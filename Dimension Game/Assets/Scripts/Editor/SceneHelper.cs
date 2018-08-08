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
        EditorSceneManager.OpenScene(targetPath);
        Debug.Log("Loaded Scene '" + targetPath + "'");
        if (!EditorSceneManager.GetSceneByName(sceneGroupName + "_" + target.ToString()).IsValid())
        {
            EditorSceneManager.CreateScene(sceneGroupName + "_" + target.ToString());
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
