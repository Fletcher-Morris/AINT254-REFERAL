using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LayerEditor : EditorWindow {

	[MenuItem("Dimensions/Dimension Layer Helper")]
    public static void ShowWindow()
    {
        GetWindow<LayerEditor>("Layer Helper");
    }

    //  Settings
    Dimension newDimension;
    Dimension currentDimension;
    string excludeTagsString;
    string[] excludeTagsArray;
    string includeLayers = "Default,PlayerSelf";
    bool setCameraCullingMasks = true;
    bool setLightCullingMask = true;
    bool setReflectionCullingMask = true;


    List<GameObject> normalObjects;
    List<Camera> cameraObjects;
    List<Light> lightObjects;
    List<ReflectionProbe> reflectionProbeObjects;

    private void OnGUI()
    {
        newDimension = (Dimension)EditorGUILayout.EnumPopup("Scene Dimension", newDimension);
        excludeTagsString = EditorGUILayout.TextField("Exclude Tags", excludeTagsString);
        EditorGUILayout.HelpBox("Separate tags with commas.", MessageType.None);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Tags"))
        {
            GetExcludeTags();
            PlayerPrefs.SetString("ExcludeTags", excludeTagsString);
            Debug.Log("Saving exclude tags.");
        }
        if (GUILayout.Button("Load Tags"))
        {
            excludeTagsString = PlayerPrefs.GetString("ExcludeTags");
            GetExcludeTags();
            Debug.Log("Loaded exclude tags.");
        }
        EditorGUILayout.EndHorizontal();
        setCameraCullingMasks = EditorGUILayout.Toggle("Set Camera Culling", setCameraCullingMasks);
        setLightCullingMask = EditorGUILayout.Toggle("Set Light Culling", setLightCullingMask);
        setReflectionCullingMask = EditorGUILayout.Toggle("Set Reflection Culling", setReflectionCullingMask);


        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Gather Objects"))
        {
            GetExcludeTags();
            GatherObjects();
        }
        if (GUILayout.Button("Convert Layers"))
        {
            GetExcludeTags();
            ConvertLayers();
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Reset Layers"))
        {
            GetExcludeTags();
            newDimension = Dimension.Normal;
            ConvertLayers();
        }
    }

    private void GetExcludeTags()
    {
        if(excludeTagsString != "")
        {
            string[] tempTags;
            tempTags = excludeTagsString.Split(',');
            if (tempTags[0] != "")
            {
                excludeTagsArray = tempTags;
            }
        }
    }

    private void GatherObjects()
    {
        normalObjects = new List<GameObject>();
        cameraObjects = new List<Camera>();
        lightObjects = new List<Light>();
        reflectionProbeObjects = new List<ReflectionProbe>();

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

                if (go.GetComponent<ReflectionProbe>())
                {
                    reflectionProbeObjects.Add(go.GetComponent<ReflectionProbe>());
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

            if (newDimension != Dimension.Normal)
            {
                currentLayer += "_" + newDimension.ToString();
            }
            obj.layer = LayerMask.NameToLayer(currentLayer);
            convertedObjects++;
        }
        Debug.Log("Converted " + convertedObjects + " GameObjects to " + newDimension.ToString() + " dimension.");

        if(setCameraCullingMasks)
        {
            convertedObjects = 0;
            foreach (Camera cam in cameraObjects)
            {
                LayerMask newCullingMask = cam.cullingMask;
                ConvertCullingMask(ref newCullingMask);
                cam.cullingMask = newCullingMask;
                convertedObjects++;
            }
            Debug.Log("Converted " + convertedObjects + " Cameras to " + newDimension.ToString() + " dimension.");
        }

        if (setLightCullingMask)
        {
            convertedObjects = 0;
            foreach (Light light in lightObjects)
            {
                LayerMask newCullingMask = light.cullingMask;
                ConvertCullingMask(ref newCullingMask);
                light.cullingMask = newCullingMask;
                convertedObjects++;
            }
            Debug.Log("Converted " + convertedObjects + " Lights to " + newDimension.ToString() + " dimension.");
        }

        if (setReflectionCullingMask)
        {
            convertedObjects = 0;
            foreach (ReflectionProbe probe in reflectionProbeObjects)
            {
                LayerMask newCullingMask = probe.cullingMask;
                ConvertCullingMask(ref newCullingMask);
                probe.cullingMask = newCullingMask;
                convertedObjects++;
            }
            Debug.Log("Converted " + convertedObjects + " Refleuction Probes to " + newDimension.ToString() + " dimension.");
        }

        currentDimension = newDimension;
    }

    private void ConvertCullingMask(ref LayerMask mask)
    {
        if (newDimension != Dimension.Normal)
        {
            if (LayerMaskTools.IncludesLayer(mask, "Default"))
            {
                LayerMaskTools.RemoveFromMask(ref mask, "Default");
                LayerMaskTools.AddToMask(ref mask, ("Default_" + newDimension.ToString()));
            }
            if (LayerMaskTools.IncludesLayer(mask, "PlayerSelf"))
            {
                LayerMaskTools.RemoveFromMask(ref mask, "PlayerSelf");
                LayerMaskTools.AddToMask(ref mask, ("PlayerSelf_" + newDimension.ToString()));
            }
        }
        else
        {
            if (LayerMaskTools.IncludesLayer(mask, ("Default_" + currentDimension.ToString())))
            {
                LayerMaskTools.RemoveFromMask(ref mask, ("Default_" + currentDimension.ToString()));
                LayerMaskTools.AddToMask(ref mask, "Default");
            }
            if (LayerMaskTools.IncludesLayer(mask, ("PlayerSelf_" + currentDimension.ToString())))
            {
                LayerMaskTools.RemoveFromMask(ref mask, ("PlayerSelf_" + currentDimension.ToString()));
                LayerMaskTools.AddToMask(ref mask, "PlayerSelf");
            }
        }
    }
}