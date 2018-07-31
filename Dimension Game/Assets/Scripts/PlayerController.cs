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
    public Transform cameraAnchor;               //  A reference to the camera anchor
    private Camera[] m_cameras;                     //  A reference to each of the player's cameras
    private Rigidbody m_body;                       //  A reference to the player's Rigidbody
    private CapsuleCollider m_col;                  //  A reference to the player's capsule collider
    private Transform m_groundCheck;                //  A reference to the ground-check object's Transform
    private DimensionSceneLoader m_sceneLoader;     //  A reference to the scene-loader instance
    private Text m_dimensionText;                   //  A reference to the 'CurrentDimension' UI text
    private Transform m_knife;               //  A reference to the Looking-Glass' Transform

    [Space]
    [Space]

    //  Movement settings
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
    private bool m_flyMode = false;                 //  Is the player superman?

    [Space]
    [Space]

    //  Input variables
    private Vector2 inputRaw, inputNorm;            //  Raw and normalised movement input
    [SerializeField]
    private bool m_isGrounded;                      //  Is the player currently grounded?
    private bool m_prevGrounded;                    //  Was the player grounded during the previous frame?
    private bool m_jumping;                         //  Is the player currently jumping?
    private bool m_canJump;                         //  Should the player be able to jump?
    [SerializeField]
    private bool m_trySwapDimension;                //  Is the player trying to switch dimension?

    [Space]
    [Space]

    [SerializeField]
    private Dimension m_currentDimension;           //  The dimension in which the player currently exists
    private Dimension m_switchingToDimension;       //  The dimension the player is currently switching to
    private bool m_switchingDimensions;             //  Is the player currently switching dimensions?
    [SerializeField]
    private float m_transitionTime = 1.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_transitionSwitchPoint = 0.8f;
    [SerializeField]
    private bool m_autoSwitchPoint = false;
    [SerializeField]
    private AnimationCurve m_curve;
    [SerializeField]
    private float m_startFov = 60f;
    [SerializeField]
    private float m_endFov = 80f;
    private RenderTexture m_dimensionPreviewTex;    //  A reference to the dimension preview RenderTexture
    private Vector2 m_prevWindowSize;               //  The window size during the previous frame

    private bool m_lookThroughGlass;                //  Is the player currently using the Looking-Glass?

    public Material speedEffectMaterial;

    public bool stopCamMovement = false;

    public GameObject dimensionPortalPrefab;
    [SerializeField]
    private GameObject m_currentPortal;


    private Animator m_knifeAnim;
    private FloatHolder m_knifeFloat;

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
        CreatePlayerCameras();                      //  Create thee cameras needed for each dimension
        m_body = GetComponent<Rigidbody>();
        m_col = m_transform.Find("Collider").GetComponent<CapsuleCollider>();
        m_groundCheck = m_transform.Find("GroundCheck");
        m_sceneLoader = GameObject.Find("GM").GetComponent<DimensionSceneLoader>();
        m_dimensionText = GameObject.Find("CurrentDimensionText").GetComponent<Text>();
        m_knife = cameraAnchor.Find("Knife");
        m_knifeAnim = m_knife.GetComponent<Animator>();
        m_knifeFloat = m_knife.GetComponent<FloatHolder>();

        //  Prevent the player's rigidbody from rotating
        m_body.freezeRotation = true;
        //  Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        //  Create the RenderTexture used for dimension previews
        CreateNewDimensionPrevewTex();
        //  Switch to the normal dimension
        SwitchDimensionImmediate(Dimension.Normal, Dimension.Dark);

        //  Declare that the player is all set up
        isInitialized = true;
    }

    //  Generate the cameras needed for each dimension
    private void CreatePlayerCameras()
    {
        //  Get the number of dimension cameras needed
        int numberOfDimensions = Dimension.GetNames(typeof(Dimension)).Length;
        //  Create a new array the size of the number of dimenions, plus the self-camera
        m_cameras = new Camera[numberOfDimensions + 1];

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
        for (int i = 0; i < numberOfDimensions; i++)
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
            if(((Dimension)i) == Dimension.Normal)
            {
                for (int j = 0; j < numberOfDimensions; j++)
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
                for (int j = 0; j < numberOfDimensions; j++)
                {
                    if (((Dimension)j) != Dimension.Normal && i != j)
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
        m_cameras[numberOfDimensions] = selfCam;
    }

    //  Create the RenderTexture needed for dimension preview
    public void CreateNewDimensionPrevewTex()
    {
        //  Set the texture to a new instance
        m_dimensionPreviewTex = new RenderTexture(Screen.width, Screen.height, 24);
        //  Set the global shader variable
        Shader.SetGlobalTexture("_DimensionPrevewTex", m_dimensionPreviewTex);
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

        m_trySwapDimension = Input.GetKey(KeyCode.E);

        if(Input.GetKeyDown(KeyCode.E))
        {
            switch (m_currentDimension)
            {
                case Dimension.Normal:
                    SwitchDimension(Dimension.Dark);
                    break;
                case Dimension.Dark:
                    SwitchDimension(Dimension.Normal);
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (m_currentDimension)
            {
                case Dimension.Normal:
                    CreateDimensionalPortal(Dimension.Dark);
                    break;
                case Dimension.Dark:
                    CreateDimensionalPortal(Dimension.Normal);
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) stopCamMovement = !stopCamMovement;

        m_lookThroughGlass = Input.GetMouseButton(0);

        if (Input.GetKeyDown(KeyCode.Return)) m_transform.position = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.V)) ToggleFlyMode();

        if (new Vector2(Screen.width, Screen.height) != m_prevWindowSize) CreateNewDimensionPrevewTex();
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
    private void ToggleFlyMode()
    {
        m_flyMode = !m_flyMode;

        if(m_flyMode)
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
        if(m_flyMode)
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
        }

        //  Move the Looking-Glass to the required position
        m_knifeAnim.SetBool("Viewing", m_lookThroughGlass);
    }

    //  Control the player's camera
    private void CamMovement()
    {
        if (stopCamMovement) return;

        Vector3 newY = m_transform.localEulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * m_lookSensitivity, 0);
        m_body.MoveRotation(Quaternion.Euler(newY));

        Vector3 newX = cameraAnchor.localEulerAngles - new Vector3(Input.GetAxis("Mouse Y") * Time.deltaTime * m_lookSensitivity, 0, 0);

        if (newX.x >= 89f && newX.x <= 180f) newX.x = 89f;
        if (newX.x <= 271f && newX.x >= 180f) newX.x = 271f;

        cameraAnchor.localEulerAngles = newX;
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
        if(!m_switchingDimensions)
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

        int fromI = (int)fromDimension;
        int toI = (int)newDimension;

        bool halfWay = false;
        float prevE = 0.0f;

        while(transitioning)
        {
            t += Time.deltaTime;
            completion = Mathf.Clamp01(t / m_transitionTime);
            float e = m_curve.Evaluate(completion);

            effect.intensity = e;
            m_cameras[fromI].fieldOfView = Mathf.Lerp(m_startFov, m_endFov, e);
            m_cameras[toI].fieldOfView = Mathf.Lerp(m_startFov, m_endFov, e);
            m_cameras[numberOfDimensions].fieldOfView = Mathf.Lerp(m_startFov, m_endFov, e);

            if (((completion >= m_transitionSwitchPoint && !m_autoSwitchPoint) || (prevE > e && m_autoSwitchPoint)) && !halfWay)
            {
                halfWay = true;

                //  Switch cameras
                for (int i = 0; i < numberOfDimensions; i++)
                {

                    if (i == (int)newDimension)
                    {
                        m_cameras[i].targetTexture = null;
                    }
                    else if (i == (int)fromDimension)
                    {
                        m_cameras[i].targetTexture = m_dimensionPreviewTex;
                    }
                }

                //  Switch the collider's layers
                if (newDimension == Dimension.Normal)
                {
                    m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
                    m_knife.gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(0).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(1).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(2).gameObject.layer = m_col.gameObject.layer;
                    m_knife.GetChild(3).gameObject.layer = m_col.gameObject.layer;

                    LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + fromDimension.ToString()));
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

                    LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
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
        m_dimensionText.text = "Dimension: " + m_currentDimension.ToString();

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

        int fromI = (int)fromDimension;
        int toI = (int)m_switchingToDimension;

        //  Switch cameras
        for (int i = 0; i < numberOfDimensions; i++)
        {

            if (i == (int)m_switchingToDimension)
            {
                m_cameras[i].targetTexture = null;
            }
            else if (i == (int)fromDimension)
            {
                m_cameras[i].targetTexture = m_dimensionPreviewTex;
            }
        }

        //  Switch the collider's layers
        if (m_switchingToDimension == Dimension.Normal)
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
            m_knife.gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(0).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(1).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(2).gameObject.layer = m_col.gameObject.layer;
            m_knife.GetChild(3).gameObject.layer = m_col.gameObject.layer;

            LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + fromDimension.ToString()));
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

            LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + m_switchingToDimension.ToString()));
        }

        m_switchingDimensions = false;
        m_currentDimension = m_switchingToDimension;
        m_dimensionText.text = "Dimension: " + m_currentDimension.ToString();

        yield return null;
    }

    //  Create a dimensional portal in front of the player
    private void CreateDimensionalPortal(Dimension destination)
    {
        if (m_currentPortal) GameObject.Destroy(m_currentPortal);

        m_knifeAnim.SetTrigger("Slash");

        m_currentPortal = GameObject.Instantiate(dimensionPortalPrefab, m_transform.position + new Vector3(0, 1.2f, 0) + (m_transform.forward * 1.5f), Quaternion.identity);
        m_currentPortal.GetComponent<DimensionPortal>().OpenPortal(destination, m_knifeFloat);
    }
}