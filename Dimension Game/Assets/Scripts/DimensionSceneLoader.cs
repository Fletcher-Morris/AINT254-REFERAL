using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DimensionSceneLoader : MonoBehaviour {

    [SerializeField]
    private bool m_loadOnAwake = true;

    private void Awake()
    {
        if (m_loadOnAwake) { LoadMultiScene(SceneManager.GetActiveScene().name.Split('_')[0]); }
    }

    public void LoadMultiScene(string sceneName)
    {
        int numberOfScenes = Dimension.GetNames(typeof(Dimension)).Length;
        
        for(int i = 0; i < numberOfScenes; i++)
        {
            string sceneString = (sceneName + "_" + ((Dimension)i).ToString()).ToLower();
            if (SceneManager.GetActiveScene().name != sceneString)
            {
                Debug.Log("Loading scene '" + sceneString + "'.");
                SceneManager.LoadScene(sceneString, LoadSceneMode.Additive);
            }
        }
    }
}
