using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionPortal : MonoBehaviour {

    private bool initialised = false;

    [SerializeField]
    private Dimension m_showDimension;

    private Transform m_transform;
    private Transform m_playerTransform;
    [SerializeField]
    private Collider m_collider;
    [SerializeField]
    private Renderer m_renderer;

    [SerializeField]
    private float m_switchDistance = 0.1f;
    [SerializeField]
    private float m_effectDistance = 2.0f;

    public float currentRange = 100f;

    private void Start()
    {
        InitPortal();
    }

    public void InitPortal()
    {
        m_transform = GetComponent<Transform>();
        m_playerTransform = GameObject.Find("Player").transform;
        m_collider = GetComponent<Collider>();

        initialised = true;
    }

    private void Update()
    {
        if (!initialised) return;
        if (!m_playerTransform) return;

        currentRange = Vector3.Distance(m_transform.position, m_playerTransform.position);
    }
}
