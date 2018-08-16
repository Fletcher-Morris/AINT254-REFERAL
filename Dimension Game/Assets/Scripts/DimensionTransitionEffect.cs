using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DimensionTransitionEffect : MonoBehaviour {

    public Shader effectShader;         //  The shader to use
    [Range(0, 1)]
    public float intensity = 0.5f;      //  The intesity of the effect
    public float speed = 30.0f;         //  The speed at which the sprites should change
    public Material material;           //  The material to use on the camera
    private float t;                    //  The current time of the shader
    private int i;                      //  The current sprite to use in the shader

    private void Start()
    {
        //  Set the intensity to 0
        intensity = 0.0f;
    }

    //  Update the shader when the camera renders
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(material)
        {
            //  Increase the time in the shader
            t += speed * Time.deltaTime;

            if (t >= 10) t = 0;
            i = Mathf.FloorToInt(t);
            //  Set the shader to use the next sprite
            material.SetInt("_Img", i);
            material.SetFloat("_Intensity", intensity);

            Graphics.Blit(source, destination, material);
        }
    }
}
