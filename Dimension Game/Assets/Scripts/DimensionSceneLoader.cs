using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//  A component used for loading scenes and their associated dimensions
public class DimensionSceneLoader : MonoBehaviour {

    [SerializeField]
    private bool m_loadOnAwake = true;  //  Should the scene be loaded on Awake()?

    //  Load the scene on awake()
    private void Awake()
    {
        if (m_loadOnAwake) { LoadMultiScene(SceneManager.GetActiveScene().name.Split('_')[0]); }
    }

    //  Load a scene, and it's associated dimensions
    public void LoadMultiScene(string sceneName)
    {
        //  Get the number of dimensions
        int numberOfScenes = Dimension.GetNames(typeof(Dimension)).Length;
        //  Loop through each dimension
        for(int i = 0; i < numberOfScenes; i++)
        {
            //  Work out the name of the scene to load
            string sceneString = (sceneName + "_" + ((Dimension)i).ToString()).ToLower();
            //  If the scene isn't loaded, load it
            if (SceneManager.GetActiveScene().name != sceneString)
            {
                Debug.Log("Loading scene '" + sceneString + "'.");
                //  Load the scene
                SceneManager.LoadScene(sceneString, LoadSceneMode.Additive);
            }
        }
    }
}