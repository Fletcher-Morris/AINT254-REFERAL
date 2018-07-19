using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSingleton : MonoBehaviour {

    private void Awake()
    {
        CheckSingleton();
    }

    private void Start()
    {
        CheckSingleton();
    }

    public void CheckSingleton()
    {
        if (Singletons.gameManager == null)
        {
            Singletons.gameManager = this;
        }
        else
        {
            if (Singletons.gameManager != this)
            {
                Destroy(gameObject);
            }
        }
    }
}