using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RhinoInsideUnity.Geometries;
using RhinoInsideUnity;

[CustomEditor(typeof(InfiniteLine))]
[CanEditMultipleObjects]
public class InfiniteLineEditor : Editor
{
    public InfiniteLine script;
    private void OnEnable()
    {
        script = (InfiniteLine)target;
    }

    void OnSceneGUI()
    {
        Transform handleTransform = script.transform;

        Vector3 p0 = script.line.From.ToUnityVector();
        Vector3 p1 = script.line.To.ToUnityVector();
        bool orientInWorld = script.OrientHandlesInWorldSpace;

        EditorGUI.BeginChangeCheck();
 
        p0 = Handles.DoPositionHandle(p0, orientInWorld ? Quaternion.identity : Quaternion.LookRotation(p1-p0));

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Move Point");
            EditorUtility.SetDirty(script);
            script.From = p0;
            script.line.From = script.From.ToRhinoPoint();
        }

        EditorGUI.BeginChangeCheck();

        p1 = Handles.DoPositionHandle(p1, orientInWorld ? Quaternion.identity : Quaternion.LookRotation(p0 - p1));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Move Point");
            EditorUtility.SetDirty(script);
            script.To = p1;
            script.line.To = script.To.ToRhinoPoint();
        }

        Handles.color = script.UniformColor;
        Handles.DrawLine(p0 + (p0-p1).normalized*script.LineExtension , p1 + (p1-p0).normalized*script.LineExtension);
    }
}
