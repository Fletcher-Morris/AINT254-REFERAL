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

    Dimension sceneDimension;

    List<GameObject> normalObjects;
    List<Camera> cameraObjects;
    List<Light> lightObjects;

    private void OnGUI()
    {
        sceneDimension = (Dimension)EditorGUILayout.EnumPopup("Scene Dimension", sceneDimension);
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
            sceneDimension = Dimension.Normal;
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

        Debug.Log(normalObjects.Count + " GameObjects, " + cameraObjects.Count + " Cameras, " + lightObjects.Count + " Lights gathered.");
    }

    private void ConvertLayers()
    {
        int convertedObjects = 0;
        foreach (GameObject obj in normalObjects)
        {
            string currentLayer = LayerMask.LayerToName(obj.layer);
            currentLayer = currentLayer.Split('_')[0];

            if (sceneDimension != Dimension.Normal)
            {
                currentLayer += "_" + sceneDimension.ToString();
            }
            obj.layer = LayerMask.NameToLayer(currentLayer);
            convertedObjects++;
        }
        Debug.Log("Converted " + convertedObjects + " objects to " + sceneDimension.ToString() + " dimension.");
    }
}
