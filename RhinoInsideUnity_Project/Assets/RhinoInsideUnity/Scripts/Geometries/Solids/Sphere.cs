using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Geometry;
using RhinoInsideUnity.Visualization;

namespace RhinoInsideUnity.Geometries.Solids
{
    [ExecuteInEditMode]
    public class Sphere : MonoBehaviour
    {
        [HideInInspector]
        public Rhino.Geometry.Sphere sphere;

        public Vector3 Center;
        [Range(0,10.0f)] public float Radius;

        private void OnEnable()
        {
            name = "Rhino Sphere Object";
        }

        public void Update()
        {
            Center = transform.position;
            sphere.Center = Center.ToRhinoPoint();
            sphere.Radius = Radius;
            transform.localScale = Vector3.one * Radius;
        }
    }
}