using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DimensionSceneLoader : MonoBehaviour {

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
