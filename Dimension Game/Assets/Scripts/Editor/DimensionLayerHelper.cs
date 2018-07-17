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

    //  Settings
    Dimension sceneDimension;
    string excludeTagsString;
    string[] excludeTagsArray;
    string includeLayers = "Default";
    bool setCameraCullingMasks = true;
    bool setLightCullingMask;

    List<GameObject> normalObjects;
    List<Camera> cameraObjects;
    List<Light> lightObjects;

    private void OnGUI()
    {
        sceneDimension = (Dimension)EditorGUILayout.EnumPopup("Scene Dimension", sceneDimension);
        excludeTagsString = EditorGUILayout.TextField("Exclude Tags", excludeTagsString);
        EditorGUILayout.HelpBox("Separate tags with commas.", MessageType.None);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Tags"))
        {
            GetExcludeTags();
            PlayerPrefs.SetString("ExcludeTags", excludeTagsString);
            Debug.Log("Loaded exclude tags.");
        }
        if (GUILayout.Button("Load Tags"))
        {
            excludeTagsString = PlayerPrefs.GetString("ExcludeTags");
            GetExcludeTags();
            Debug.Log("Saving exclude tags.");
        }
        EditorGUILayout.EndHorizontal();
        setCameraCullingMasks = EditorGUILayout.Toggle("Set Camera Culling", setCameraCullingMasks);


        EditorGUILayout.Separator();


        if(GUILayout.Button("Gather Objects"))
        {
            GetExcludeTags();
            GatherObjects();
        }
        if (GUILayout.Button("Convert Layers"))
        {
            GetExcludeTags();
            ConvertLayers();
        }
        if (GUILayout.Button("Reset Layers"))
        {
            GetExcludeTags();
            sceneDimension = Dimension.Normal;
            ConvertLayers();
        }
    }

    private void GetExcludeTags()
    {
        string[] tempTags;
        tempTags = excludeTagsString.Split(',');
        if(tempTags[0] != "")
        {
            excludeTagsArray = tempTags;
        }
    }

    private void GatherObjects()
    {
        normalObjects = new List<GameObject>();
        cameraObjects = new List<Camera>();
        lightObjects = new List<Light>();

        foreach(GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            bool exclude = false;
            if(excludeTagsArray.Length >= 1)
            {
                foreach (string tag in excludeTagsArray)
                {
                    if(tag != "")
                    {
                        if (go.tag == tag)
                        {
                            exclude = true;
                            break;
                        }
                    }
                }
            }

            if(!exclude)
            {
                if (go.GetComponent<Camera>())
                {
                    cameraObjects.Add(go.GetComponent<Camera>());
                }

                if (go.GetComponent<Light>())
                {
                    lightObjects.Add(go.GetComponent<Light>());
                }

                normalObjects.Add(go);
            }
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

        convertedObjects = 0;
        foreach(Camera cam in cameraObjects)
        {
            LayerMask currentMask = cam.cullingMask;
            LayerMask newMask;

            Debug.Log(currentMask);
        }
    }

    private bool IncludesLayer(LayerMask mask, string layerName)
    {
        return IncludesLayer(mask, LayerMask.NameToLayer(layerName));
    }
    private bool IncludesLayer(LayerMask mask, int layer)
    {
        if (mask == (mask | (1 << layer)))
        {
            return true;
        }
        return false;
    }
}