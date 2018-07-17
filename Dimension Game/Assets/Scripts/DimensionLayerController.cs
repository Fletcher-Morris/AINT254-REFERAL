using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionLayerController : MonoBehaviour {
    
    public DimensionDef[] dimensionDefs;
}

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