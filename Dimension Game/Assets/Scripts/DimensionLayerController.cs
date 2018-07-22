using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //  A singleton class used for managing dimensions
public class DimensionLayerController : MonoBehaviour {
    
    public DimensionDef[] dimensionDefs;

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
        if (Singletons.layerController == null)
        {
            Singletons.layerController = this;
        }
        else
        {
            if (Singletons.layerController != this)
            {
                Destroy(gameObject);
            }
        }
    }
}

    //  An enum class for each dimension
public enum Dimension
{
    Normal = 0,
    Other = 1
}

[System.Serializable]
public class DimensionDef
{
    public string name = "Dimension";
    public LayerMask visibleLayers;
    public LayerMask lightingLayers;
}