using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DimensionLayerHelper : EditorWindow {

	[MenuItem("Dimensions/Dimension Layer Helper")]
    public static void ShowWindow()
    {
        GetWindow<DimensionLayerHelper>("Layer Helper");
    }

    Dimension m_sceneDimension;

    private void OnGUI()
    {
        m_sceneDimension = (Dimension)EditorGUILayout.EnumPopup("Scene's Dimension", m_sceneDimension);
    }
}
