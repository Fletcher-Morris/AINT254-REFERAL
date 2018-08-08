using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneHelper : EditorWindow
{

    [MenuItem("Dimensions/Dimension Scene Helper")]
    public static void ShowWindow()
    {
        GetWindow<SceneHelper>("Scene Helper");
    }

    string fullPath;
    string scenePath;
    string sceneGroupName;
    string sceneName;
    bool fixPath;
    string targetPath;

    private void OnGUI()
    {
        EditorGUILayout.Separator();
        for (int i = 0; i < 3; i++)
        {
            Dimension dim = ((Dimension)i);
            if (GUILayout.Button(dim.ToString()))
            {
                OpenScene(dim);
            }
        }
    }

    private void OpenScene(Dimension target)
    {
        GetPaths();
        targetPath = (scenePath + sceneGroupName + "_" + target.ToString() + ".unity").ToLower();
        if(Application.CanStreamedLevelBeLoaded(sceneGroupName + "_" + target.ToString()))
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
