using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        m_body.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        SwitchDimension(Dimension.Normal);
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

    private void Movement()
    {
        if(m_isGrounded)
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

            if(Input.GetKey(KeyCode.Space) && m_canJump)
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
            StartCoroutine(SwitchDimensionCoroutine(newDimension));
        }
    }
    private IEnumerator SwitchDimensionCoroutine(Dimension newDimension)
    {
        m_switchingDimensions = true;
        m_switchingToDimension = newDimension;

        //  Switch cameras
        for (int i = 0; i < Singletons.layerController.dimensionDefs.Length; i++)
        {
            m_cameras[i].enabled = (i == (int)newDimension);
        }

        //  Switch the collider layer
        if(newDimension == Dimension.Normal)
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
            LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + m_currentDimension.ToString()));
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
        }
        else
        {
            m_col.gameObject.layer = LayerMask.NameToLayer("PlayerSelf_" + newDimension.ToString());
            LayerMaskTools.RemoveFromMask(ref m_groundMask, LayerMask.NameToLayer("Default"));
            LayerMaskTools.AddToMask(ref m_groundMask, LayerMask.NameToLayer("Default_" + newDimension.ToString()));
        }

        m_switchingDimensions = false;
        m_currentDimension = newDimension;
        yield return null;
    }
}