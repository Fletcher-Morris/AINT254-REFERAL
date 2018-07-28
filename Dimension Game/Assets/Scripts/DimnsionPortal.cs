using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiensionPortal : MonoBehaviour {

    [SerializeField]
    private Dimension m_showDimension;

    private Transform m_transform;
    private Transform m_playerTransform;
    [SerializeField]
    private Collider m_collider;
    [SerializeField]
    private Renderer m_renderer;

    private void Start()
    {
        InitPortal();
    }

    public void InitPortal()
    {
        m_transform = GetComponent<Transform>();
        m_playerTransform = GameObject.Find("Player").transform;
        m_collider = GetComponent<Collider>();
    }
}
