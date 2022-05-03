using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexMap))]
public class HexMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HexMap hexMap = target as HexMap;
        if (GUILayout.Button("Generate"))
        {
            hexMap.Init();
        }
    }
}
