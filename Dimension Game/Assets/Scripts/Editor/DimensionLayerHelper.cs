using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DimensionLayerHelper : EditorWindow {

	[MenuItem("Dimensions/Dimension Layer Helper")]
    public static void ShowWindow()
    {
        GetWindow<DimensionLayerHelper>("Layer Helper");
    }

    Dimension m_sceneDimension;

    List<GameObject> normalObjects;
    List<Camera> cameraObjects;
    List<Light> lightObjects;

    private void OnGUI()
    {
        m_sceneDimension = (Dimension)EditorGUILayout.EnumPopup("Scene's Dimension", m_sceneDimension);
        if(GUILayout.Button("Gather Objects"))
        {
            GatherObjects();
        }
    }

    private void GatherObjects()
    {
        normalObjects = new List<GameObject>();
        cameraObjects = new List<Camera>();
        lightObjects = new List<Light>();

        foreach(GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if(go.GetComponent<Camera>())
            {
                cameraObjects.Add(go.GetComponent<Camera>());
            }

            if(go.GetComponent<Light>())
            {
                lightObjects.Add(go.GetComponent<Light>());
            }

            normalObjects.Add(go);
        }

        Debug.Log(normalObjects.Count + " GameObjects gathered.");
        Debug.Log(cameraObjects.Count + " Cameras gathered.");
        Debug.Log(lightObjects.Count + " Lights gathered.");
    }

    private void ConvertLayers(Dimension dim)
    {

    }
}
