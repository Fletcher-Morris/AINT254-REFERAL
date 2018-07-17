﻿using System.Collections;
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
        m_sceneLoader = GameObject.Find("GM").GetComponent<DimensionSceneLoader>();

        m_body.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void GetInput()
    {
        inputRaw = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputNorm = inputRaw.normalized;
        m_trySwapDimension = Input.GetKey(KeyCode.E);
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

        Vector3 newX = m_camTransform.localEulerAngles - new Vector3(Input.GetAxis("Mouse Y") * Time.deltaTime * m_lookSensitivity, 0, 0);

        if (newX.x >= 89f && newX.x <= 180f) newX.x = 89f;
        if (newX.x <= 271f && newX.x >= 180f) newX.x = 271f;

        m_camTransform.localEulerAngles = newX;
    }

    private void Update()
    {
        GetInput();
        GroundCheck();
        CamMovement();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public void SwapDimension(Dimension newDimension)
    {

    }
}