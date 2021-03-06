﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //  A sington class used for storing various misc game variables
public class GameManagerSingleton : MonoBehaviour {

    public bool debug = true;

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