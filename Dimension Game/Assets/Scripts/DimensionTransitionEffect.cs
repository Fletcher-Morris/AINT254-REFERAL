using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DimensionTransitionEffect : MonoBehaviour {

    public Shader effectShader;
    [Range(0, 1)]
    public float intensity = 0.5f;
    public float speed = 30.0f;
    private Material m_mat = null;

    [SerializeField]
    private float t;
    [SerializeField]
    private int i;

    private void Start()
    {
        if (effectShader)
        {
            m_mat = new Material(effectShader);
        }
        else
        {
            effectShader = Shader.Find("Custom/DimensionTransitionShader");
            m_mat = new Material(effectShader);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(m_mat)
        {
            t += speed * Time.deltaTime;

            if (t >= 10) t = 0;
            i = Mathf.FloorToInt(t);

            m_mat.SetInt("_Img", i);
            m_mat.SetFloat("_Intensity", intensity);

            Graphics.Blit(source, destination, m_mat);
        }

        m_mat = new Material(effectShader);
    }
}
