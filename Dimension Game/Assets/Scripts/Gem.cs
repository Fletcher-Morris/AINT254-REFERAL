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
    private float m_bobRange = 0.25f;
    [SerializeField]
    private float m_bobSpeed = 1.0f;

    private bool m_isBeingCollected = false;

    public float t = 0;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();
        m_originalPosition = m_transform.position;
        m_rotationAxis = m_transform.up;
    }

    private void Update()
    {
        Vector3 newPos = m_originalPosition;

        if(m_player && m_collectable)
        {
            m_currentRange = Vector3.Distance(m_transform.position, m_player.cameraAnchor.transform.position);

            if(m_currentRange <= m_movementRange)
            {
                newPos = Vector3.MoveTowards(m_transform.position, m_player.cameraAnchor.transform.position, 10 * Time.deltaTime);
                m_isBeingCollected = true;
                if (m_currentRange <= m_collectionRange) CollectGem();
            }
            else
            {
                m_isBeingCollected = false;
            }
        }

        if (!m_isBeingCollected && m_bob)
        {
            newPos = m_originalPosition;
            newPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * m_bobSpeed) * m_bobRange;
            m_transform.position = newPos;
        }

        m_transform.position = newPos;
    }

    public void CollectGem()
    {

        GameObject.Destroy(gameObject);
    }
}