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

    private void OnGUI()
    {
        for(int i = 0; i < 3; i++)
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
        string targetPath = (scenePath + sceneGroupName + "_" + target.ToString() + ".unity").ToLower();
        if(!scenePath.StartsWith("assets"))
        {
            string newPath = "assets" + targetPath;
            targetPath = newPath;
        }
        EditorSceneManager.OpenScene(targetPath);
    }

    private void GetPaths()
    {
        sceneName = EditorSceneManager.GetActiveScene().name;
        sceneGroupName = sceneName.Split('_')[0];
        fullPath = EditorSceneManager.GetActiveScene().path;
        scenePath = fullPath.Trim((sceneName + ".unity").ToCharArray());
    }
}
