using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    private bool isInitialized = false;

    //  References to various instances
    private Transform m_transform;
    private Transform m_cameraAnchor;
    private List<Camera> m_cameras;
    private Transform m_camTransform;
    private Rigidbody m_body;
    private CapsuleCollider m_col;
    private Transform m_groundCheck;
    private DimensionSceneLoader m_sceneLoader;
    private Text m_dimensionText;
    private Transform m_lookingGlass;

    //  Movement settings
    [SerializeField]
    private float m_runSpeed = 5f;
    [SerializeField]
    private float m_jumpForce = 2f;
    [SerializeField]
    private float m_groundDist = 0.5f;
    [SerializeField]
    private LayerMask m_groundMask;
    [SerializeField]
    private float m_lookSensitivity = 50f;
    [SerializeField]
    private bool m_flyMode = false;

    //  Input variables
    private Vector2 inputRaw, inputNorm;
    [SerializeField]
    private bool m_isGrounded;
    private bool m_prevGrounded;
    private bool m_jumping;
    private bool m_canJump;
    [SerializeField]
    private bool m_trySwapDimension;

    [SerializeField]
    private Dimension m_currentDimension;
    private Dimension m_switchingToDimension;
    private bool m_switchingDimensions;
    private RenderTexture m_dimensionPreviewTex;
    private Vector2 m_prevWindowSize;
    [SerializeField]
    private Vector3 m_lookingGlassInUse;
    [SerializeField]
    private Vector3 m_lookingGlassPutAway;
    [SerializeField]
    private float m_lookingGlassMoveSpeed = 1.0f;
    [SerializeField]
    private bool m_lookThroughGlass;

    private void Start()
    {
        PlayerInit();
    }

    //  Set up the player
    public void PlayerInit()
    {
        m_transform = GetComponent<Transform>();
        m_cameraAnchor = m_transform.Find("CameraAnchor");
        CreatePlayerCameras();
        m_body = GetComponent<Rigidbody>();
        m_col = m_transform.Find("Collider").GetComponent<CapsuleCollider>();
        m_groundCheck = m_transform.Find("GroundCheck");
        m_sceneLoader = GameObject.Find("GM").GetComponent<DimensionSceneLoader>();
        m_dimensionText = GameObject.Find("CurrentDimensionText").GetComponent<Text>();
        m_lookingGlass = m_cameraAnchor.Find("LookingGlass");

        m_body.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        CreateNewDimensionPrevewTex();
        SwitchDimension(Dimension.Normal, Dimension.Other);
        isInitialized = true;
    }
    //  Generate the cameras needed for each dimension
    private void CreatePlayerCameras()
    {
        m_cameras = new List<Camera>();
        int numberOfDimensions = Dimension.GetNames(typeof(Dimension)).Length;
        for (int i = 0; i < numberOfDimensions; i++)
        {
            string camName = ("Camera_" + ((Dimension)i).ToString());
            Camera newCam = new GameObject(camName).AddComponent<Camera>();
            newCam.transform.SetParent(m_cameraAnchor);
            newCam.transform.localPosition = Vector3.zero;
            newCam.transform.localEulerAngles = Vector3.zero;

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
            }
            newCam.cullingMask = newMask;
            m_cameras.Add(newCam);
            Debug.Log("Added camera '" + camName + "'.");
        }
    }
    public void CreateNewDimensionPrevewTex()
    {
        m_dimensionPreviewTex = new RenderTexture(Screen.width, Screen.height, 24);
        Shader.SetGlobalTexture("_DimensionPrevewTex", m_dimensionPreviewTex);
        m_prevWindowSize = new Vector2(Screen.width, Screen.height);
    }

    private void GetInput()
    {
        inputRaw = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputNorm = inputRaw.normalized;
        m_trySwapDimension = Input.GetKey(KeyCode.E);

        if(Input.GetKeyDown(KeyCode.E))
        {
            switch (m_currentDimension)
            {
                case Dimension.Normal:
                    SwitchDimension(Dimension.Other);
                    break;
                case Dimension.Other:
                    SwitchDimension(Dimension.Normal);
                    break;
                default:
                    break;
            }
        }
        m_lookThroughGlass = Input.GetMouseButton(0);
        if (Input.GetKeyDown(KeyCode.Return)) m_transform.position = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.V)) ToggleFlyMode();

        if (new Vector2(Screen.width, Screen.height) != m_prevWindowSize) CreateNewDimensionPrevewTex();
    }

    private void GroundCheck()
    {
        m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_groundDist, m_groundMask, QueryTriggerInteraction.Ignore);
        if (m_isGrounded && !m_prevGrounded) OnGrounded();
        else if (!m_isGrounded && m_prevGrounded) OnUngrounded();
        m_prevGrounded = m_isGrounded;
    }

    private void OnGrounded()
    {
        m_canJump = true;
    }
    private void OnUngrounded()
    {
        m_canJump = false;
    }

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

    private void Movement()
    {
        if(m_flyMode)
        {
            float upDown = 0.0f;
            if (Input.GetKey(KeyCode.Space)) upDown = 1.0f;
            if (Input.GetKey(KeyCode.LeftShift)) upDown = -1.0f;

            m_body.AddForce(m_transform.forward * inputRaw.y * m_runSpeed * 25);
            m_body.AddForce(m_transform.up * upDown * m_runSpeed * 25);
            m_body.AddForce(m_transform.right * inputRaw.x * m_runSpeed * 25);
        }
        else
        {
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

        if(m_lookThroughGlass)
        {
            m_lookingGlass.transform.localPosition = Vector3.Lerp(m_lookingGlass.transform.localPosition, m_lookingGlassInUse, m_lookingGlassMoveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            m_lookingGlass.transform.localPosition = Vector3.Lerp(m_lookingGlass.transform.localPosition, m_lookingGlassPutAway, m_lookingGlassMoveSpeed * Time.fixedDeltaTime);
        }
    }

    private void CamMovement()
    {
        Vector3 newY = m_transform.localEulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * m_lookSensitivity, 0);
        m_body.MoveRotation(Quaternion.Euler(newY));

        Vector3 newX = m_cameraAnchor.localEulerAngles - new Vector3(Input.GetAxis("Mouse Y") * Time.deltaTime * m_lookSensitivity, 0, 0);

        if (newX.x >= 89f && newX.x <= 180f) newX.x = 89f;
        if (newX.x <= 271f && newX.x >= 180f) newX.x = 271f;

        m_cameraAnchor.localEulerAngles = newX;
    }

    private void Update()
    {
        if (!isInitialized) return;

        GetInput();
        GroundCheck();
        CamMovement();

        Debug.DrawLine((m_cameraAnchor.position + (m_cameraAnchor.right * -0.2f)), m_cameraAnchor.forward * 5f, Color.blue);
        Debug.DrawLine((m_cameraAnchor.position + (m_cameraAnchor.right * 0.2f)), m_cameraAnchor.forward * 5f, Color.blue);
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        Movement();
    }

    //  Switch to a different dimension
    public void SwitchDimension(Dimension newDimension)
    {
        if(!m_switchingDimensions)
        {
            StartCoroutine(SwitchDimensionCoroutine(newDimension, m_currentDimension));
        }
    }
    public void SwitchDimension(Dimension newDimension, Dimension fromDimesion)
    {
        if (!m_switchingDimensions)
        {
            StartCoroutine(SwitchDimensionCoroutine(newDimension, fromDimesion));
        }
    }
    private IEnumerator SwitchDimensionCoroutine(Dimension newDimension, Dimension fromDimesion)
    {
        m_switchingDimensions = true;
        m_switchingToDimension = newDimension;

        //  Switch cameras
        for (int i = 0; i < Singletons.layerController.dimensionDefs.Length; i++)
        {
            //m_cameras[i].enabled = (i == (int)newDimension);
            if (i == (int)newDimension)
            {
                m_cameras[i].targetTexture = null;
            }
            else if(i == (int)fromDimesion)
            {
                m_cameras[i].targetTexture = m_dimensionPreviewTex;
            }
        }

        //  Switch the collider layer
        if(newDimension == Dimension.Normal)
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
            m_lookingGlass.gameObject.layer = m_col.gameObject.layer;
            m_lookingGlass.GetChild(0).gameObject.layer = m_col.gameObject.layer;
            m_lookingGlass.GetChild(1).gameObject.layer = m_col.gameObject.layer;

            LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + fromDimesion.ToString()));
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
        }
        else
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf_" + newDimension.ToString());
            m_lookingGlass.gameObject.layer = m_col.gameObject.layer;
            m_lookingGlass.GetChild(0).gameObject.layer = m_col.gameObject.layer;
            m_lookingGlass.GetChild(1).gameObject.layer = m_col.gameObject.layer;

            LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + newDimension.ToString()));
        }

        m_switchingDimensions = false;
        m_currentDimension = newDimension;
        m_dimensionText.text = "Dimension: " + m_currentDimension.ToString();

        yield return null;
    }
}