using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LayerEditor : EditorWindow {

	[MenuItem("Dimensions/Dimension Tools")]
    public static void ShowWindow()
    {
        GetWindow<LayerEditor>("Dimensions");
    }

    //  Settings
    Dimension newDimension;
    Dimension currentDimension;
    string includeLayers = "Default,PlayerSelf";
    bool setCameraCullingMasks = true;
    bool setLightCullingMask = true;
    bool setReflectionCullingMask = true;
    bool setSkyboxHolder= true;
    bool setGems = true;


    List<GameObject> normalObjects;
    List<Camera> cameraObjects;
    List<Light> lightObjects;
    List<ReflectionProbe> reflectionProbeObjects;
    List<Gem> gemObjects;
    GameObject skyboxHolder;


    string fullPath;
    string scenePath;
    string sceneGroupName;
    string sceneName;
    bool fixPath;
    string targetPath;


    private void OnGUI()
    {
        newDimension = (Dimension)EditorGUILayout.EnumPopup("Scene Dimension", newDimension);
        setCameraCullingMasks = EditorGUILayout.Toggle("Set Camera Culling", setCameraCullingMasks);
        setLightCullingMask = EditorGUILayout.Toggle("Set Light Culling", setLightCullingMask);
        setReflectionCullingMask = EditorGUILayout.Toggle("Set Reflection Culling", setReflectionCullingMask);
        setSkyboxHolder = EditorGUILayout.Toggle("Set Skybox Holder", setSkyboxHolder);
        setGems = EditorGUILayout.Toggle("Set Gems", setGems);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Gather Objects"))
        {
            GatherObjects();
        }
        if (GUILayout.Button("Convert Layers"))
        {
            ConvertLayers();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < 3; i++)
        {
            Dimension dim = ((Dimension)i);
            if (GUILayout.Button(dim.ToString()))
            {
                OpenScene(dim);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void GatherObjects()
    {
        normalObjects = new List<GameObject>();
        cameraObjects = new List<Camera>();
        lightObjects = new List<Light>();
        reflectionProbeObjects = new List<ReflectionProbe>();
        gemObjects = new List<Gem>();

        foreach(GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
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

            if (go.GetComponent<Gem>())
            {
                gemObjects.Add(go.GetComponent<Gem>());
            }

            normalObjects.Add(go);

            if (setSkyboxHolder && go.name.ToLower().Contains("skybox"))
            {
                skyboxHolder = go;
            }
        }

        Debug.Log(normalObjects.Count + " GameObjects, " + cameraObjects.Count + " Cameras, " + lightObjects.Count + " Lights, " + reflectionProbeObjects.Count + " Reflection Probes, " + gemObjects.Count + " Gems gathered.");
    }

    private void ConvertLayers()
    {
        int convertedObjects = 0;

        foreach (GameObject obj in normalObjects)
        {
            string currentLayer = LayerMask.LayerToName(obj.layer);
            currentLayer = currentLayer.Split('_')[0];

            if (newDimension != Dimension.Default)
            {
                currentLayer += "_" + newDimension.ToString();
            }
            obj.layer = LayerMask.NameToLayer(currentLayer);
            convertedObjects++;
        }

        if(setCameraCullingMasks)
        {
            foreach (Camera cam in cameraObjects)
            {
                LayerMask newCullingMask = cam.cullingMask;
                ConvertCullingMask(ref newCullingMask);
                cam.cullingMask = newCullingMask;
                convertedObjects++;
            }
        }

        if (setLightCullingMask)
        {
            foreach (Light light in lightObjects)
            {
                LayerMask newCullingMask = light.cullingMask;
                ConvertCullingMask(ref newCullingMask);
                light.cullingMask = newCullingMask;
                convertedObjects++;
            }
        }

        if (setReflectionCullingMask)
        {
            foreach (ReflectionProbe probe in reflectionProbeObjects)
            {
                LayerMask newCullingMask = probe.cullingMask;
                ConvertCullingMask(ref newCullingMask);
                probe.cullingMask = newCullingMask;
                convertedObjects++;
            }
        }

        if(setSkyboxHolder && skyboxHolder)
        {
            skyboxHolder.name = ("Skybox_" + newDimension.ToString());
        }

        if(setGems)
        {
            foreach (Gem gem in gemObjects)
            {
                gem.dimension = newDimension;
                convertedObjects++;
            }
        }

        Debug.Log("Converted " + convertedObjects + " objects to " + newDimension.ToString() + " dimension.");

        currentDimension = newDimension;
    }

    private void ConvertCullingMask(ref LayerMask mask)
    {
        if (newDimension != Dimension.Default)
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




    private void OpenScene(Dimension target)
    {
        GetPaths();
        targetPath = (scenePath + sceneGroupName + "_" + target.ToString() + ".unity").ToLower();
        if (Application.CanStreamedLevelBeLoaded(sceneGroupName + "_" + target.ToString()))
        {
            EditorSceneManager.OpenScene(targetPath);
            Debug.Log("Loaded Scene '" + targetPath + "'");
        }
        else
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            bool success = EditorSceneManager.SaveScene(newScene, targetPath);
            if (success) Debug.LogWarning("Could Not Create New Scene '" + targetPath + "'");
        }
    }

    private void GetPaths()
    {
        sceneName = EditorSceneManager.GetActiveScene().name;
        sceneGroupName = sceneName.Split('_')[0];
        fullPath = EditorSceneManager.GetActiveScene().path;
        scenePath = "assets/scenes/" + sceneGroupName + "/";
    }
}