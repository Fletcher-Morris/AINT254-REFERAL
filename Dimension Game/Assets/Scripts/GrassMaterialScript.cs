using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassMaterialScript : MonoBehaviour {

    public Material grassMaterial;
    private string property = "_PlayerPos";
    [SerializeField]
    private Transform m_transform;

    private void Start()
    {
        m_transform = transform;
    }

    private void LateUpdate()
    {
        if (grassMaterial == null) return;
        if (m_transform == null) return;

        grassMaterial.SetVector(property, m_transform.position);
        Debug.Log(gameObject.name + " : " + m_transform.position);
    }
}
