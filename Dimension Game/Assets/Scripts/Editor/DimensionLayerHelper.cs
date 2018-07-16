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
        if (GUILayout.Button("Convert Layers"))
        {
            ConvertLayers();
        }
        if (GUILayout.Button("Reset Layers"))
        {
            m_sceneDimension = Dimension.Normal;
            ConvertLayers();
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

    private void ConvertLayers()
    {
        foreach(GameObject obj in normalObjects)
        {
            string currentLayer = LayerMask.LayerToName(obj.layer);

            if(m_sceneDimension == Dimension.Normal)
            {
                currentLayer = currentLayer.Split('_')[0];
            }
            else
            {
                currentLayer += "_" + m_sceneDimension.ToString();
            }
            obj.layer = LayerMask.NameToLayer(currentLayer);

            Debug.Log(currentLayer);
        }
    }
}
