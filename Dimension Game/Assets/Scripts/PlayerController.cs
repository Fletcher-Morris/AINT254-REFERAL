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
    private float m_lookSensitivity = 1f;

    //  Input variables
    private Vector2 inputRaw, inputNorm;
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

        m_body.freezeRotation = true;
    }

    private void GetInput()
    {
        inputRaw = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputNorm = inputRaw.normalized;
    }
}
