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

    public float openTime = 0.5f;

    private void Start()
    {
        InitPortal();
    }

    private void InitPortal()
    {
        m_transform = GetComponent<Transform>();
        m_playerTransform = GameObject.Find("Player").transform;
        m_collider = GetComponent<Collider>();

        initialised = true;
    }

    public void OpenPortal(Dimension toDimension)
    {

    }

    private IEnumerator OpenPortalCoroutine(Dimension toDimension)
    {
        float t = 0.0f;
        float completion = 0.0f;
        bool open = false;

        while(!open)
        {
            t += Time.deltaTime;
            completion = Mathf.Clamp01(t / openTime);

            m_renderer.material.SetFloat("_Completion", completion);

            if (completion >= 1.0f) open = true;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private void Update()
    {
        if (!initialised) return;
        if (!m_playerTransform) return;

        currentRange = Vector3.Distance(m_transform.position, m_playerTransform.position);
    }
}
