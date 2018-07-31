using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionPortal : MonoBehaviour {

    private bool initialised = false;

    [SerializeField]
    private Dimension destination;

    private Transform m_transform;
    private PlayerController m_player;
    private FloatHolder m_knifeFloat;
    [SerializeField]
    private Collider m_collider;
    [SerializeField]
    private Renderer m_renderer;

    [SerializeField]
    private float m_switchDistance = 0.1f;
    [SerializeField]
    private float m_effectDistance = 1.0f;

    public float currentRange = 100f;

    public float openTime = 0.5f;

    private Vector3 startScale, endScale;

    public float effectFactor = 0f;

    private void InitPortal(FloatHolder knifeFloat)
    {
        m_transform = GetComponent<Transform>();
        m_renderer = GetComponent<Renderer>();
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();
        m_collider = GetComponent<Collider>();
        m_knifeFloat = knifeFloat;
        startScale = m_transform.localScale;
        endScale = startScale * 10;

        initialised = true;
    }

    public void OpenPortal(Dimension dest, FloatHolder knifeFloat)
    {
        if (!initialised) InitPortal(knifeFloat);
        destination = dest;
        StartCoroutine(OpenPortalCoroutine());
    }

    private IEnumerator OpenPortalCoroutine()
    {
        float completion = 0.0f;
        bool open = false;

        while(!open)
        {
            completion = Mathf.Clamp01(m_knifeFloat.value);

            m_renderer.material.SetFloat("_Completion", completion);

            if (completion >= 1.0f) open = true;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private void Update()
    {
        if (!initialised) return;

        currentRange = Vector3.Distance(m_transform.position, m_player.cameraAnchor.position);

        if(currentRange <= m_effectDistance)
        {

            effectFactor = Mathf.Lerp(0, 1, 1 - currentRange);

            m_transform.localScale = Vector3.Lerp(startScale, endScale, effectFactor);
        }
        else
        {
            m_transform.localScale = startScale;
        }

        if (effectFactor >= 0.5) m_player.SwitchDimensionImmediate(destination);
    }
}