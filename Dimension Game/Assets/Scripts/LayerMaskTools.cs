using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //  A static class useful for working with layer-masks
public static class LayerMaskTools
{
    //  Check if a layer mask includes a specific layer
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

    //  Add a specific layer to a leayer mask
    public static void AddToMask(ref LayerMask mask, string layerName)
    {
        mask |= (1 << LayerMask.NameToLayer(layerName));
    }
    public static void AddToMask(ref LayerMask mask, int layer)
    {
        mask |= (1 << layer);
    }

    //  Remove a specific layer to a layer mask
    public static void RemoveFromMask(ref LayerMask mask, string layerName)
    {
        if (!IncludesLayer(mask, layerName)) return;

        mask ^= (1 << LayerMask.NameToLayer(layerName));
    }
    public static void RemoveFromMask(ref LayerMask mask, int layer)
    {
        if (!IncludesLayer(mask, layer)) return;

        mask ^= (1 << layer);
    }
}