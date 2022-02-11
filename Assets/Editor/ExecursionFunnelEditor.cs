using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExecursionFunnelField))]
public class ExecursionFunnelEditor : Editor
{
    ExecursionFunnelField _target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _target = target as ExecursionFunnelField;
        if(GUILayout.Button("Draw"))
        {
            _target.IsActiveIns = true;
        }
        if(GUILayout.Button("Clear"))
        {
            _target.IsActiveIns = false;
            _target.ClearMesh();
        }
    }
}
