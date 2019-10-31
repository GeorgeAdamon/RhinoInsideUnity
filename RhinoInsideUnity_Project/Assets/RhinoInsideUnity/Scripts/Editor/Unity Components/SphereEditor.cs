using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RhinoInsideUnity.Visualization;
using RhinoInsideUnity;

[CustomEditor(typeof(SphereVisualizer))]
[CanEditMultipleObjects]
public class SphereVisualizerEditor : Editor
{
    public SphereVisualizer script;

    private void OnEnable()
    {
        script = (SphereVisualizer)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        script.UCount = EditorGUILayout.IntSlider("U Count", script.UCount, 0, 200);
        script.VCount = EditorGUILayout.IntSlider("V Count", script.VCount, 0, 200);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Change Subdivision");
            EditorUtility.SetDirty(script);
            script.ReconstructGeometry(true);
        }
    }
}
