using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Transform m_transform;
    private Camera m_cam;
    private Transform m_camTransform;
    private Rigidbody m_body;



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
    }
}
