using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DimensionTransitionEffect : MonoBehaviour {

    public Shader effectShader;
    [Range(0, 1)]
    public float intensity = 0.5f;
    public float speed = 30.0f;
    public Material material;

    [SerializeField]
    private float t;
    [SerializeField]
    private int i;

    private void Start()
    {
        //if (effectShader)
        //{
        //    material = new Material(effectShader);
        //}
        //else
        //{
        //    effectShader = Shader.Find("Custom/DimensionTransitionShader");
        //    material = new Material(effectShader);
        //}

        intensity = 0.0f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(material)
        {
            t += speed * Time.deltaTime;

            if (t >= 10) t = 0;
            i = Mathf.FloorToInt(t);

            material.SetInt("_Img", i);
            material.SetFloat("_Intensity", intensity);

            Graphics.Blit(source, destination, material);
        }

        //material = new Material(effectShader);
    }
}
