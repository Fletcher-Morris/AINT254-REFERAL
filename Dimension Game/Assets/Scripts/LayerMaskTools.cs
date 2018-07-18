using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskTools
{
    public static bool IncludesLayer(LayerMask mask, string layerName)
    {
        return IncludesLayer(mask, LayerMask.NameToLayer(layerName));
    }
    public static bool IncludesLayer(LayerMask mask, int layer)
    {
        if (mask == (mask | (1 << layer)))
        {
            return true;
        }
        return false;
    }

    public static void AddToMask(ref LayerMask mask, string layerName)
    {
        mask |= (1 << LayerMask.NameToLayer(layerName));
    }
    public static void AddToMask(ref LayerMask mask, int layer)
    {
        mask |= (1 << layer);
    }

    public static void RemoveFromMask(ref LayerMask mask, string layerName)
    {
        mask ^= (1 << LayerMask.NameToLayer(layerName));
    }
    public static void RemoveFromMask(ref LayerMask mask, int layer)
    {
        mask ^= (1 << layer);
    }
}