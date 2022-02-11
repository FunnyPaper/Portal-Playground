using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArrayModifier))]
public class ArrayModifierEditor : Editor
{
    ArrayModifier _target;
    SerializedObject _serializedTarget;

    static bool[] Fold = new bool[3];

    private void Awake()
    {
        _target = target as ArrayModifier;
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        _serializedTarget = new SerializedObject(_target);

        SerializedProperty prop = _serializedTarget.FindProperty("ObjectToClone");
        EditorGUILayout.PropertyField(prop);

        Fold[0] = EditorGUILayout.Foldout(Fold[0], "Add new count", true);
        if (Fold[0])
        {
            prop = _serializedTarget.FindProperty("count");
            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(prop, new GUIContent("Next to add"));
            if (GUILayout.Button("Add"))
            {
                _target.Extend(_target.count);
            }

            GUILayout.EndHorizontal();
        }

        Fold[1] = EditorGUILayout.Foldout(Fold[1], "Counts", true);
        if (Fold[1])
        {
            prop = _serializedTarget.FindProperty("counts");
            for (int i = 0; i < prop.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex(i), new GUIContent($"count {i + 1} :"));
                if (GUILayout.Button("Remove"))
                {
                    _target.Shrink(i);
                }
                GUILayout.EndHorizontal();
            }
        }

        Fold[2] = EditorGUILayout.Foldout(Fold[2], "Offsets", true);
        if (Fold[2])
        {
            prop = _serializedTarget.FindProperty("offsets");
            for (int i = 0; i < prop.arraySize; i++)
            {
                EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex(i), new GUIContent($"offset {i + 1} :"));
            }
        }

        if(GUILayout.Button("Remove All"))
        {
            _target.Clear();
        }


        if(_target.ObjectToClone)
        {
            _target.ResizeInitializes();
            _target.UpdateInitializesPosition();
        }
        _serializedTarget.ApplyModifiedProperties();
    }
}
