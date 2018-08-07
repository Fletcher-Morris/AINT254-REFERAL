using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    [SerializeField]
    private int m_value;
    public Dimension dimension;
    [SerializeField]
    private bool m_bob = true;
    [SerializeField]
    private bool m_collectable = true;

    private Transform m_transform;
    private PlayerController m_player;
    private Vector3 m_originalPosition;
    private float m_movementRange = 2f;
    private float m_collectionRange = 0.25f;
    private float m_currentRange;

    private Vector3 m_rotationAxis;
    [SerializeField]
    private float m_bobRange = 0.25f;
    [SerializeField]
    private float m_bobSpeed = 0.5f;
    private float m_bobRdm;

    [SerializeField]
    private bool m_randomColour = true;

    private bool m_isBeingCollected = false;

    public float t = 0;

    private Transform m_endPos;
    private Vector3 m_endScale;

    private Color m_color;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();
        m_transform.position += new Vector3(0, m_bobRange, 0);
        m_originalPosition = m_transform.position;
        m_rotationAxis = m_transform.up;
        m_bobRdm = Random.Range(0f, 1f);
        m_endPos = GameObject.Find("GemPosition").transform;
        m_endScale = new Vector3(0.4f, 0.4f, 0.4f);

        if(m_randomColour)
        {
            float hue = Random.Range(0f, 1f);
            m_color = Color.HSVToRGB(hue, 1, 1);
            GetComponent<Renderer>().material.color = m_color;
        }
    }

    private void Update()
    {
        Vector3 newPos = m_originalPosition;

        if(m_player && m_collectable)
        {
            m_currentRange = Vector3.Distance(m_transform.position, m_player.transform.position);

            if(m_currentRange <= m_movementRange)
            {
                if(!m_isBeingCollected)
                {
                    gameObject.layer = LayerMask.NameToLayer("PlayerSelf");
                    m_transform.parent = m_endPos.parent;
                }

                m_isBeingCollected = true;                
            }

            if(m_isBeingCollected)
            {
                m_currentRange = Vector3.Distance(m_transform.localPosition, m_endPos.localPosition);
                newPos = Vector3.MoveTowards(m_transform.position, m_endPos.position, 20 * Time.deltaTime);
                m_transform.localScale = Vector3.MoveTowards(m_transform.localScale, m_endScale, 20 * Time.deltaTime);
                if (m_currentRange <= m_collectionRange) CollectGem();
            }
        }

        Vector3 bobPos = m_originalPosition;
        bobPos.y += Mathf.Sin((Time.fixedTime * Mathf.PI * m_bobSpeed) + m_bobRdm) * m_bobRange;

        if (!m_isBeingCollected && m_bob)
        {
            newPos = bobPos;
        }

        m_transform.position = newPos;
    }

    public void CollectGem()
    {
        Singletons.score.AddGem(m_color);
        GameObject.Destroy(gameObject);
    }
}