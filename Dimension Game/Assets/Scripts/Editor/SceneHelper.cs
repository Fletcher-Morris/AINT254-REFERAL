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

    Dimension m_dimension;
    string m_path;

    private void OnGUI()
    {
        m_dimension = (Dimension)EditorGUILayout.EnumPopup("Scene Dimension", m_dimension);

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Scene"))
        {
            OpenScene(m_dimension);
        }
        if (GUILayout.Button("Save Scene"))
        {

        }
    }

    private void OpenScene(Dimension dimension)
    {
        string currentScene = EditorSceneManager.GetActiveScene().name.Split('_')[0];
        if(StringToDimension(currentScene) != dimension)
        {
            string sceneName = GetSceneName(currentScene, dimension);

            if(!EditorSceneManager.GetSceneByName(sceneName).IsValid())
            {
                CreateScene(sceneName, dimension);
            }

            EditorSceneManager.LoadScene(sceneName);
        }
    }

    private string GetScenePath()
    {
        string result = EditorSceneManager.GetActiveScene().path;
        result = result.Split('_')[0];
        return result;
    }

    private void CreateScene(string sceneName, Dimension dimension)
    {
        UnityEngine.SceneManagement.Scene newScene = EditorSceneManager.CreateScene(sceneName);
        EditorSceneManager.SaveScene(newScene, GetScenePath() + "_" + dimension.ToString());
    }

    private string GetSceneName(string name, Dimension dimension)
    {
        string result = name;
        name += "_";
        name += dimension.ToString();
        return result;
    }

    private Dimension StringToDimension(string name)
    {
        Dimension result = Dimension.Default;
        for(int i = 0; i < 3; i++)
        {
            if (((Dimension)i).ToString() == name) return (Dimension)i;
        }

        return result;
    }
}
