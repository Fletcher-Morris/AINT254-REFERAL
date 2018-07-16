using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    //  References to various instances
    private Transform m_transform;
    private Camera m_cam;
    private Transform m_camTransform;
    private Rigidbody m_body;
    private CapsuleCollider m_col;
    private Transform m_groundCheck;

    //  Movement settings
    [SerializeField]
    private float m_runSpeed = 5f;
    [SerializeField]
    private float m_jumpForce = 2f;
    [SerializeField]
    private float m_groundDist = 0.05f;
    [SerializeField]
    private LayerMask m_groundMask;
    [SerializeField]
    private float m_lookSensitivity = 1f;

    //  Input variables
    private Vector2 inputRaw, inputNorm;
    [SerializeField]
    private bool m_isGrounded;

    private void Start()
    {
        PlayerInit();
    }

    public void PlayerInit()
    {
        m_transform = GetComponent<Transform>();
        if (!m_cam) m_cam = GetComponentInChildren<Camera>();
        m_camTransform = m_cam.transform;
        m_body = GetComponent<Rigidbody>();
        m_col = GetComponent<CapsuleCollider>();
        m_groundCheck = m_transform.Find("GroundCheck");

        m_body.freezeRotation = true;
    }

    private void GetInput()
    {
        inputRaw = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputNorm = inputRaw.normalized;
    }

    private void GroundCheck()
    {
        m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_groundDist, m_groundMask, QueryTriggerInteraction.Ignore);
    }

    private void Movement()
    {
        if(m_isGrounded)
        {
            Vector3 targetVelocity = new Vector3(inputRaw.x, 0, inputRaw.y);
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= m_runSpeed;

            Vector3 velocity = m_body.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -10f, 10f);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -10f, 10f);
            velocityChange.y = 0;
            m_body.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {

        }
    }


    private void Update()
    {
        GetInput();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        Movement();
    }
}
