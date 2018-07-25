using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassMaterialScript : MonoBehaviour {

    public Material[] grassMaterials;
    private string property = "_PlayerPos";
    [SerializeField]
    private Transform m_transform;

    private void Start()
    {
        m_transform = transform;
    }

    private void LateUpdate()
    {
        if (grassMaterials.Length <= 0) return;
        if (m_transform == null) return;

        foreach(Material mat in grassMaterials)
        {
            mat.SetVector(property, m_transform.position);
        }
    }
}
