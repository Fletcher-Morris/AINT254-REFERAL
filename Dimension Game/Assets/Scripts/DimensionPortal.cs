using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionPortal : MonoBehaviour {

    private bool initialised = false;

    [SerializeField]
    private Dimension m_destination;
    private Dimension m_origin;

    [SerializeField]
    private PortalState m_state;

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

    private Vector3 startScale, endScale;

    public float effectFactor = 0f;
    public AnimationCurve effectCurve = AnimationCurve.EaseInOut(0.0f,0.0f,1.0f,1.0f);

    private Vector3 m_originalPosition;

    private void InitPortal(FloatHolder knifeFloat)
    {
        m_transform = GetComponent<Transform>();
        m_originalPosition = m_transform.position;
        m_renderer = GetComponent<Renderer>();
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();
        m_collider = GetComponent<Collider>();
        m_knifeFloat = knifeFloat;
        startScale = m_transform.localScale;
        endScale = new Vector3(30f, 30f, 1f);
        m_state = PortalState.Opening;

        initialised = true;
    }

    public void OpenPortal(Dimension destination, Dimension origin, FloatHolder knifeFloat)
    {
        if (!initialised) InitPortal(knifeFloat);
        m_destination = destination;
        m_origin = origin;
        if(m_origin == Dimension.Default)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default_" + m_origin.ToString());
        }
        StartCoroutine(OpenPortalCoroutine());
    }

    private IEnumerator OpenPortalCoroutine()
    {
        float completion = 0.0f;

        while(m_state == PortalState.Opening)
        {
            completion = Mathf.Clamp01(m_knifeFloat.value);

            m_renderer.material.SetFloat("_Completion", completion);

            if (completion >= 1.0f) m_state = PortalState.Open;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private void Update()
    {
        if (!initialised) return;

        RotateToPlayer();

        currentRange = Vector2.Distance(new Vector2(m_transform.position.x, m_transform.position.z), new Vector2(m_player.cameraAnchor.position.x, m_player.cameraAnchor.position.z));

        if(currentRange <= m_effectDistance)
        {

            effectFactor = Mathf.Lerp(0, 1, 1 - currentRange);

            float t = effectCurve.Evaluate(effectFactor);

            m_transform.localScale = Vector3.Lerp(startScale, endScale, t);

            if (currentRange <= m_switchDistance && m_state == PortalState.Open && m_origin == m_player.GetDimension())
            {
                m_player.SwitchDimensionImmediate(m_destination);
                GameObject.Destroy(gameObject);
            }
        }
        else
        {
            m_transform.localScale = startScale;
        }
    }

    public void RotateToPlayer()
    {
        Vector3 a = m_transform.position;
        a.y = 0.0f;
        Vector3 b = m_player.cameraAnchor.position;
        b.y = 0.0f;

        Vector3 relativePos = a - b;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        m_transform.rotation = rotation;
    }

    public PortalState GetState()
    {
        return m_state;
    }
}

public enum PortalState
{
    Opening,
    Open,
    Closing
}