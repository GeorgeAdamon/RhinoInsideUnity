using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rhino.Geometry;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;

[CustomPropertyDrawer(typeof(Rhino.Geometry.Sphere))]
public class RhinoSphereEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        EditorGUI.LabelField(position, "Holder of a " + fieldInfo.FieldType.FullName,EditorStyles.boldLabel);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(Rhino.Geometry.Box))]
public class RhinoBoxEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        EditorGUI.LabelField(position, "Holder of a " + fieldInfo.FieldType.FullName,EditorStyles.boldLabel);

        EditorGUI.EndProperty();
    }
}