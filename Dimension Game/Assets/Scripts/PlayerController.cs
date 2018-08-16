using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    //  Has the player been fully set up?
    [SerializeField]
    private bool isInitialized = false;

    //  References to various instances
    private Transform m_transform;                  //  A reference to the player's Transform
    [HideInInspector]
    public Transform cameraAnchor;                  //  A reference to the camera anchor
    private Camera[] m_cameras;                     //  A reference to each of the player's cameras
    private Rigidbody m_body;                       //  A reference to the player's Rigidbody
    private CapsuleCollider m_col;                  //  A reference to the player's capsule collider
    private Transform m_groundCheck;                //  A reference to the ground-check object's Transform
    private DimensionSceneLoader m_sceneLoader;     //  A reference to the scene-loader instance
    private Transform m_knife;                       //  A reference to the Looking-Glass' Transform
    private int m_numberOfDimensions = 0;

    [Space]
    [Space]

    //  Movement variables
    [SerializeField]
    private float m_runSpeed = 5f;                  //  The player's maximum movement speed
    [SerializeField]
    private float m_jumpForce = 2f;                 //  The upwards force applied to jump
    [SerializeField]
    private float m_groundDist = 0.5f;              // The radius of the sphere-check ground-detection
    [SerializeField]
    private LayerMask m_groundMask;                 //  The layers detected as ground
    [SerializeField]
    private float m_lookSensitivity = 50f;          //  The mouse ensitivity used for looking around
    [SerializeField]
    private bool m_debug = false;                   //  Is the player superman?
    private Vector2 inputRaw, inputNorm;            //  Raw and normalised movement input
    [SerializeField]
    private bool m_isGrounded;                      //  Is the player currently grounded?
    private bool m_prevGrounded;                    //  Was the player grounded during the previous frame?
    private bool m_jumping;                         //  Is the player currently jumping?
    private bool m_canJump;                         //  Should the player be able to jump?

    [Space]
    [Space]

    [SerializeField]
    private Dimension m_currentDimension;           //  The dimension in which the player currently exists
    private Dimension m_switchingToDimension;       //  The dimension the player is currently switching to
    private bool m_switchingDimensions;             //  Is the player currently switching dimensions?
    [SerializeField]
    private float m_transitionTime = 1.0f;          //  How long should the transition last>
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_transitionSwitchPoint = 0.8f;   //  At what point in the transition should the dimensions be switched?
    [SerializeField]
    private bool m_autoSwitchPoint = false;         //  Should the transiotn point be automatically detected?
    [SerializeField]
    private AnimationCurve m_curve;                 //  The animation curve for the transion
    [SerializeField]
    private float m_startFov = 60f;                 //  The normal field of view
    [SerializeField]
    private float m_endFov = 80f;                   //  The fov at the transition point
    [HideInInspector]
    public RenderTexture[] m_renderTextures;        //  An array of the render textures used for previews
    private Vector2 m_prevWindowSize;               //  The window size during the previous frame
    private bool m_lookThroughGlass;                //  Is the player currently using the Looking-Glass?
    public Material speedEffectMaterial;            //  A reference to the material used for the effect
    public bool stopCamMovement = false;            //  Should camera movement be halted?
    public GameObject dimensionPortalPrefab;        //  The prefab for the portals
    [SerializeField]
    private GameObject m_currentPortal;             //  The currently active portal
    private DimensionPortal m_portalControler;      //  The controller on the currently active portal
    [HideInInspector]
    public bool isCreatingPortal;                   //  Is the player currently creating a portal?
    private Animator m_knifeAnim;                   //  The animator for the knife
    private FloatHolder m_knifeFloat;               //  The 'position' of the knife during the slicing animation

    [Space]
    [Space]

    //  Audio stuff
    [SerializeField]
    private AudioClip[] m_dimensionAudio;           //  An array holding the background audio for each dimension
    private AudioSource[] m_audioSources;           //  An array holding the audiosources for each dimension background audio


    private Text m_controlsText;                    //  The text component of the controls ui
    private Text m_debugText;
    private int m_fps;

    private void Start()
    {
        PlayerInit();   //  Initialise the player
    }

    //  Initialise up the player
    public void PlayerInit()
    {
        //  Find and assign various references
        m_transform = GetComponent<Transform>();
        cameraAnchor = m_transform.Find("CameraAnchor");
        //  Get the number of dimension cameras needed
        m_numberOfDimensions = Dimension.GetNames(typeof(Dimension)).Length;
        //  Create the cameras needed for each dimension
        CreatePlayerCameras();

        //  Get references to various objects
        m_body = GetComponent<Rigidbody>();
        m_col = m_transform.Find("Collider").GetComponent<CapsuleCollider>();
        m_groundCheck = m_transform.Find("GroundCheck");
        m_sceneLoader = GameObject.Find("GM").GetComponent<DimensionSceneLoader>();
        m_knife = cameraAnchor.Find("Knife");
        m_knifeAnim = m_knife.GetComponent<Animator>();
        m_knifeFloat = m_knife.GetComponent<FloatHolder>();

        //  For each dimension, add a background soundtrack
        m_audioSources = new AudioSource[m_numberOfDimensions];
        for(int i = 0; i<m_numberOfDimensions; i++)
        {
            m_audioSources[i] = gameObject.AddComponent<AudioSource>();
            m_audioSources[i].loop = true;
            m_audioSources[i].clip = m_dimensionAudio[i];
            m_audioSources[i].volume = 0;
            m_audioSources[i].Play();
        }

        //  Get the controls and debug ui
        m_controlsText = GameObject.Find("ControlsText").GetComponent<Text>();
        m_debugText = GameObject.Find("DebugText").GetComponent<Text>();

        //  Prevent the player's rigidbody from rotating
        m_body.freezeRotation = true;

        //  Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //  Create the render textures used for dimension previews
        CreateRenderTextures();

        //  Switch to the normal dimension
        SwitchDimensionImmediate(Dimension.Default, Dimension.Dark);

        //  Start the FPS counter
        StartCoroutine(FpsCoroutine());

        //  Declare that the player is all set up and ready to go!
        isInitialized = true;
    }

    //  Generate the cameras and render textures needed for each dimension
    private void CreatePlayerCameras()
    {        
        //  Create a new array the size of the number of dimenions, plus the self-camera
        m_cameras = new Camera[m_numberOfDimensions + 1];
        //  Create a new array the size of the number of dimensions
        m_renderTextures = new RenderTexture[m_numberOfDimensions];

        //  Create and set up the self-camera
        Camera selfCam = new GameObject("Camera_self").AddComponent<Camera>();
        selfCam.fieldOfView = m_startFov;
        selfCam.cullingMask = 0 << 0;
        selfCam.depth = 1;
        selfCam.clearFlags = CameraClearFlags.Depth;
        selfCam.transform.SetParent(cameraAnchor);
        selfCam.transform.localPosition = Vector3.zero;
        selfCam.transform.localEulerAngles = Vector3.zero;
        selfCam.gameObject.AddComponent<DimensionTransitionEffect>();
        selfCam.gameObject.GetComponent<DimensionTransitionEffect>().material = speedEffectMaterial;
        LayerMask selfMask = selfCam.cullingMask;

        //  Loop through each dimension
        for (int i = 0; i < m_numberOfDimensions; i++)
        {
            //  Create a new camera for the dimension
            string camName = ("Camera_" + ((Dimension)i).ToString());
            Camera newCam = new GameObject(camName).AddComponent<Camera>();
            newCam.fieldOfView = m_startFov;
            newCam.transform.SetParent(cameraAnchor);
            newCam.transform.localPosition = Vector3.zero;
            newCam.transform.localEulerAngles = Vector3.zero;

            //  Set the new camera's skybox to match the dimension
            Skybox sky = newCam.gameObject.AddComponent<Skybox>();
            SkyboxHolder holder = GameObject.Find("Skybox_" + ((Dimension)i).ToString()).GetComponent<SkyboxHolder>();
            sky.material = holder.skybox;

            //  Add and remove the appropriate layers from the camera's culling mask
            LayerMask newMask = newCam.cullingMask;
            if(((Dimension)i) == Dimension.Default)
            {
                for (int j = 0; j < m_numberOfDimensions; j++)
                {
                    if(i != j)
                    {
                        LayerMaskTools.RemoveFromMask(ref newMask, ("Default_" + ((Dimension)j).ToString()));
                        LayerMaskTools.RemoveFromMask(ref newMask, ("PlayerSelf_" + ((Dimension)j).ToString()));
                    }
                }

                LayerMaskTools.AddToMask(ref selfMask, "PlayerSelf");
            }
            else
            {
                LayerMaskTools.RemoveFromMask(ref newMask, "Default");
                LayerMaskTools.RemoveFromMask(ref newMask, "PlayerSelf");
                for (int j = 0; j < m_numberOfDimensions; j++)
                {
                    if (((Dimension)j) != Dimension.Default && i != j)
                    {
                        LayerMaskTools.RemoveFromMask(ref newMask, ("Default_" + ((Dimension)j).ToString()));
                        LayerMaskTools.RemoveFromMask(ref newMask, ("PlayerSelf_" + ((Dimension)j).ToString()));
                    }
                }

                LayerMaskTools.AddToMask(ref selfMask, ("PlayerSelf_" + ((Dimension)i).ToString()));
            }
            newCam.cullingMask = newMask;

            //  Add the new camera to the array
            m_cameras[i] = newCam;
            if(Singletons.gameManager.debug) Debug.Log("Added camera '" + camName + "'.");
        }

        //  Set the self-camera's culling mask
        selfCam.cullingMask = selfMask;
        //  Add the self-camera to the array
        m_cameras[m_numberOfDimensions] = selfCam;
    }

    //  Create the RenderTexture needed for dimension preview
    public void CreateRenderTextures()
    {
        //  Create the render textures
        for (int i = 0; i < m_numberOfDimensions; i++)
        {
            m_renderTextures[i] = new RenderTexture(Screen.width, Screen.height, 24);
        }


        //  Set the previous window sise value
        m_prevWindowSize = new Vector2(Screen.width, Screen.height);
    }

    //  Get input from the mouse & keyboard
    private void GetInput()
    {
        //  Create a Vector2 from the appropriate Input axis
        inputRaw = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //  Normalise the Vector2
        inputNorm = inputRaw.normalized;
        
        //  Should the player tryy to swap dimensions?
        if(Input.GetKeyDown(KeyCode.E))
        {
            SwitchDimension(GetNextDimension());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchDimension(GetPrevDimension());
        }

        //  Should the player try to create a portal?
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateDimensionalPortal(GetNextDimension());
        }

        //  Should the player try to look through the knife?
        if (Input.GetMouseButtonDown(0))
        {
            m_lookThroughGlass = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_lookThroughGlass = false;
        }

        //  Resets the player's position to 0,0,0
        if (Input.GetKeyDown(KeyCode.Return)) ResetPosition();
        //  Toggles debug mode
        if (Input.GetKeyDown(KeyCode.F3)) ToggleDebug();
        //  Shows the controls
        if (!Input.GetKey(KeyCode.Tab)) m_controlsText.text = "Controls: Tab";
        else { m_controlsText.text = "Movement: WASD\nJump: Space\nSwitch Dimension: Q/E\nOpen Portal: R\nDebug: F3"; }

        //  Should the render textures be re-made?
        if (new Vector2(Screen.width, Screen.height) != m_prevWindowSize) CreateRenderTextures();
    }

    public void ResetPosition()
    {
        m_transform.position = Vector3.zero;
    }

    //  Return the current dimension
    public Dimension GetDimension()
    {
        return m_currentDimension;
    }

    //  Return the next dimension
    private Dimension GetNextDimension()
    {
        return GetNextDimension(m_currentDimension);
    }
    //  Return the previous dimension
    private Dimension GetPrevDimension()
    {
        return GetPrevDimension(m_currentDimension);
    }
    //  Return the next dimension
    private Dimension GetNextDimension(Dimension dim)
    {
        switch (dim)
        {
            case Dimension.Default:
                return Dimension.Dark;
            case Dimension.Dark:
                return Dimension.Light;
            case Dimension.Light:
                return Dimension.Default;
            default:
                return Dimension.Default;
        }
    }
    //  Return the previous dimension
    private Dimension GetPrevDimension(Dimension dim)
    {
        switch (dim)
        {
            case Dimension.Default:
                return Dimension.Light;
            case Dimension.Dark:
                return Dimension.Default;
            case Dimension.Light:
                return Dimension.Dark;
            default:
                return Dimension.Default;
        }
    }

    //  Check if the player is grounded
    private void GroundCheck()
    {
        //  Use a CheckSphere to set the grounded state
        m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_groundDist, m_groundMask, QueryTriggerInteraction.Ignore);

        //  If the new grounded state is different from the previous, call the appropriate method
        if (m_isGrounded && !m_prevGrounded) OnGrounded();
        else if (!m_isGrounded && m_prevGrounded) OnUngrounded();

        //  Set the previous grounded state
        m_prevGrounded = m_isGrounded;
    }

    //  What to do when the player becomes grounded
    private void OnGrounded()
    {
        m_canJump = true;
    }
    //  What to do when the player becomes un-grounded
    private void OnUngrounded()
    {
        m_canJump = false;
    }

    //  Allow the player to fly (for debuging)
    private void ToggleDebug()
    {
        m_debug = !m_debug;

        if(m_debug)
        {
            m_body.useGravity = false;
            m_body.drag = 10.0f;
        }
        else
        {
            m_body.useGravity = true;
            m_body.drag = 1.0f;
        }
    }

    //  Control the player's movement
    private void Movement()
    {
        if(m_debug)
        {
            //  The movement to use if flying

            float upDown = 0.0f;
            if (Input.GetKey(KeyCode.Space)) upDown = 1.0f;
            if (Input.GetKey(KeyCode.LeftShift)) upDown = -1.0f;

            m_body.AddForce(m_transform.forward * inputRaw.y * m_runSpeed * 25);
            m_body.AddForce(m_transform.up * upDown * m_runSpeed * 25);
            m_body.AddForce(m_transform.right * inputRaw.x * m_runSpeed * 25);
        }
        else
        {
            //  The movement to use if not flying

            if (m_isGrounded)
            {
                //  MOVEMENT
                Vector3 targetVelocity = new Vector3(inputRaw.x, 0, inputRaw.y);
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= m_runSpeed;

                Vector3 velocity = m_body.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -10f, 10f);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -10f, 10f);
                velocityChange.y = 0;
                m_body.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Input.GetKey(KeyCode.Space) && m_canJump)
                {
                    //  JUMP
                    m_body.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
                }
            }
            else
            {
                //  AIR CONTROL
            }

            if (m_transform.position.y <= -20f) ResetPosition();
        }

        //  Move the Looking-Glass to the required position
        m_knifeAnim.SetBool("Viewing", m_lookThroughGlass);
    }

    //  Control the player's camera
    private void CamMovement()
    {
        if (stopCamMovement) return;

        if(!m_currentPortal || m_portalControler.GetState() == PortalState.Open)
        {
            Vector3 newY = m_transform.localEulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * m_lookSensitivity, 0);
            m_body.MoveRotation(Quaternion.Euler(newY));

            Vector3 newX = cameraAnchor.localEulerAngles - new Vector3(Input.GetAxis("Mouse Y") * Time.deltaTime * m_lookSensitivity, 0, 0);

            if (newX.x >= 89f && newX.x <= 180f) newX.x = 89f;
            if (newX.x <= 271f && newX.x >= 180f) newX.x = 271f;

            cameraAnchor.localEulerAngles = newX;
        }
        else
        {
            float newX = cameraAnchor.localEulerAngles.x;
            float s = 100f;

            if (newX >= 45f && newX <= 315f) s = 200f;
            //if (newX <= 271f && newX >= 180f) s = 100f;

            newX = Mathf.MoveTowardsAngle(newX, 0f, s * Time.deltaTime);
            cameraAnchor.localEulerAngles = new Vector3(newX, 0, 0);
        }
    }

    private void Update()
    {
        //  If the player is not yet initialised, return early
        if (!isInitialized) return;

        //  Get the mouse & keyboard input
        GetInput();
        //  Check the grounded state of the player
        GroundCheck();
        //  Control the player's camera
        CamMovement();

        //  Draw two lines to show where the player is looking (for debugging)
        Debug.DrawLine(cameraAnchor.position, cameraAnchor.forward * 5f, Color.blue);

        if (m_debug) DebugStuff();
        else { m_debugText.text = ""; }
    }

    //  Set the debug text
    private void DebugStuff()
    {
        string debugString = "DEBUG\n\n";

        debugString += "Dimension : ";
        debugString += m_currentDimension;

        debugString += "\nSwitching Dimension : ";
        debugString += m_switchingDimensions;

        debugString += "\nPortal Open : ";
        debugString += m_currentPortal != null;

        debugString += "\nOpening Portal : ";
        debugString += isCreatingPortal;

        debugString += "\nPosition : ";
        debugString += m_transform.position;

        debugString += "\nFPS : ";
        debugString += m_fps;

        m_debugText.text = debugString;
    }

    private void FixedUpdate()
    {
        //  If the player is not yet initialised, return early
        if (!isInitialized) return;

        //  Control the player's movement
        Movement();
    }

    //  Switch to a different dimension
    public void SwitchDimension(Dimension newDimension)
    {
        if(!m_switchingDimensions && (!m_currentPortal || m_portalControler.GetState() == PortalState.Open))
        {
            //  If the player is not currently switching dimensions, start the co-routine
            StartCoroutine(SwitchDimensionCoroutine(newDimension, m_currentDimension));
        }
    }
    //  Switch to a different dimension, from a specific dimension
    public void SwitchDimension(Dimension newDimension, Dimension fromDimesion)
    {
        if (!m_switchingDimensions)
        {
            //  If the player is not currently switching dimensions, start the co-routine
            StartCoroutine(SwitchDimensionCoroutine(newDimension, fromDimesion));
        }
    }
    //  The co-routine that switches the player to a different dimension
    private IEnumerator SwitchDimensionCoroutine(Dimension newDimension, Dimension fromDimension)
    {
        m_switchingDimensions = true;
        m_switchingToDimension = newDimension;
        int numberOfDimensions = Dimension.GetNames(typeof(Dimension)).Length;

        bool transitioning = true;
        float t = 0.0f;
        float completion = 0.0f;

        DimensionTransitionEffect effect = m_cameras[numberOfDimensions].GetComponent<DimensionTransitionEffect>();
        effect.intensity = 1.0f;

        bool halfWay = false;
        float prevE = 0.0f;

        while(transitioning)
        {
            t += Time.deltaTime;
            completion = Mathf.Clamp01(t / m_transitionTime);
            float e = m_curve.Evaluate(completion);

            effect.intensity = e;
            float newFov = Mathf.Lerp(m_startFov, m_endFov, e);

            for (int i = 0; i < numberOfDimensions; i++)
            {
                m_cameras[i].fieldOfView = newFov;

                if(i == (int)m_switchingToDimension)
                {
                    m_audioSources[i].volume = e;
                }
                else if (i == (int)fromDimension)
                {
                    m_audioSources[i].volume = 1 - e;
                }
                else
                {
                    m_audioSources[i].volume = 0;
                }
            }

            if (((completion >= m_transitionSwitchPoint && !m_autoSwitchPoint) || (prevE > e && m_autoSwitchPoint)) && !halfWay)
            {
                halfWay = true;

                //  Switch render textures
                for (int i = 0; i < numberOfDimensions; i++)
                {
                    Camera cam = m_cameras[i];

                    if (i == (int)m_switchingToDimension)
                    {
                        cam.targetTexture = null;
                        cam.depth = 0;
                    }
                    else
                    {
                        m_cameras[i].targetTexture = m_renderTextures[i];
                        cam.depth = -1;
                    }
                }

                //  Set the new render texture
                Shader.SetGlobalTexture("_DimensionPrevewTex", m_renderTextures[(int)GetNextDimension(m_switchingToDimension)]);

                //  Switch the collider's layers
                m_groundMask = 0;
                if (newDimension == Dimension.Default)
                {
                    m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
                    m_knife.gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(0).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(1).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(2).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(3).gameObject.layer = m_col.gameObject.layer;
                    LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
                }
                else
                {
                    m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf_" + newDimension.ToString());
                    m_knife.gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(0).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(1).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(2).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(3).gameObject.layer = m_col.gameObject.layer;
                    LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + newDimension.ToString()));
                }
            }

            prevE =  e;

            if (completion >= 1.0f) transitioning = false;
            yield return new WaitForEndOfFrame();
        }

        effect.intensity = 0.0f;

        m_switchingDimensions = false;
        m_currentDimension = newDimension;

        yield return null;
    }
    //  Switch to a different dimension immediately, with no transition
    public void SwitchDimensionImmediate(Dimension newDimension)
    {
        if (!m_switchingDimensions)
        {
            //  If the player is not currently switching dimensions, start the co-routine
            StartCoroutine(SwitchDimensionImmediateCoroutine(newDimension, m_currentDimension));
        }
    }
    //  Switch to a different dimension, from a specific dimension, with no transition
    public void SwitchDimensionImmediate(Dimension newDimension, Dimension fromDimesion)
    {
        if (!m_switchingDimensions)
        {
            //  If the player is not currently switching dimensions, start the co-routine
            StartCoroutine(SwitchDimensionImmediateCoroutine(newDimension, fromDimesion));
        }
    }
    //  Switch dimensions without the transition
    private IEnumerator SwitchDimensionImmediateCoroutine(Dimension newDimension, Dimension fromDimension)
    {
        m_switchingDimensions = true;
        m_switchingToDimension = newDimension;
        int numberOfDimensions = Dimension.GetNames(typeof(Dimension)).Length;

        DimensionTransitionEffect effect = m_cameras[numberOfDimensions].GetComponent<DimensionTransitionEffect>();
        effect.intensity = 0.0f;

        //  Switch render textures
        for (int i = 0; i < numberOfDimensions; i++)
        {
            Camera cam = m_cameras[i];

            if(i == (int)m_switchingToDimension)
            {
                cam.targetTexture = null;
                cam.depth = 0;
            }
            else
            {
                m_cameras[i].targetTexture = m_renderTextures[i];
                cam.depth = -1;
            }

            if (i == (int)m_switchingToDimension)
            {
                m_audioSources[i].volume = 1;
            }
            else
            {
                m_audioSources[i].volume = 0;
            }
        }

        //  Set the new render texture
        Shader.SetGlobalTexture("_DimensionPrevewTex", m_renderTextures[(int)GetNextDimension(m_switchingToDimension)]);

        //  Switch the collider's layers
        m_groundMask = 0;
        if (m_switchingToDimension == Dimension.Default)
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
            m_knife.gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(0).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(1).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(2).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(3).gameObject.layer = m_col.gameObject.layer;
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
        }
        else
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf_" + m_switchingToDimension.ToString());
            m_knife.gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(0).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(1).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(2).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(3).gameObject.layer = m_col.gameObject.layer;
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + m_switchingToDimension.ToString()));
        }

        m_switchingDimensions = false;
        m_currentDimension = m_switchingToDimension;

        yield return null;
    }

    //  Create a dimensional portal in front of the player
    private void CreateDimensionalPortal(Dimension destination)
    {
        //  Check that the player should be able to create a new portal
        if (m_switchingDimensions) return;
        if (isCreatingPortal) return;

        if (m_currentPortal)
        {
            //  Close the current portal
            m_currentPortal.GetComponent<DimensionPortal>().Close();
        }

        //  Start the animation
        m_knifeAnim.SetTrigger("Slash");

        //  Create the new portal
        m_currentPortal = GameObject.Instantiate(dimensionPortalPrefab, m_transform.position + new Vector3(0, 1.2f, 0) + (m_transform.forward * 1.5f), Quaternion.identity);
        m_portalControler = m_currentPortal.GetComponent<DimensionPortal>();
        Shader.SetGlobalTexture("_PortalPrevewTex", m_renderTextures[(int)destination]);
        m_portalControler.OpenPortal(destination, m_currentDimension, m_knifeFloat);
    }

    private IEnumerator FpsCoroutine()
    {
        bool dewit = true;
        float t = 0f;
        int count = 0;

        while(dewit)
        {
            t += Time.deltaTime;
            count++;
            if(t >= 0.5f)
            {
                m_fps = Mathf.RoundToInt((float)count/t);
                t = 0f;
                count = 0;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}