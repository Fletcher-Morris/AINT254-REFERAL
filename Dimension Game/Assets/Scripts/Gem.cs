using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    [SerializeField]
    private int m_value;
    public Dimension dimension;
    [SerializeField]
    private bool m_rotate = true;
    [SerializeField]
    private bool m_bob = true;

    private Transform m_transform;
    private Transform m_player;
    private float m_movementRange = 2f;
    private float m_collectionRange = 0.25f;
    private float m_currentRange;

    private Vector3 m_rotationAxis;
    private float m_rotateSpeed = 10.0f;
    private float m_bobRange = 0.5f;
    private float m_bobSpeed = 1.0f;

    public float t = 0;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
        m_player = GameObject.Find("Player").transform;
        m_rotationAxis = m_transform.up;
    }

    private void Update()
    {
        if(m_rotate)
        {
            m_transform.Rotate(m_rotationAxis, m_rotateSpeed * Time.deltaTime);
        }

        Vector3 newPos = m_transform.position;

        if (m_bob)
        {
            t += m_bobRange * Mathf.Sin(m_bobSpeed * Time.deltaTime);
            newPos.y = t;
        }

        if(m_player)
        {
            m_currentRange = Vector3.Distance(m_transform.position, m_player.position);

            if(m_currentRange <= m_movementRange)
            {
                newPos = Vector3.MoveTowards(m_transform.position, m_player.position, 10 * Time.deltaTime);
                if (m_currentRange <= m_collectionRange) CollectGem();
            }
        }

        m_transform.position = newPos;
    }

    public void CollectGem()
    {

        GameObject.Destroy(gameObject);
    }
}
