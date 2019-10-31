using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Geometry;
using RhinoInsideUnity.Extensions;
using RhinoInsideUnity.Visualization;

namespace RhinoInsideUnity.Geometries.Solids
{
    [ExecuteInEditMode]
    public class RhinoSphere : RhinoPrimitiveBase
    {
        [Range(0,10.0f)] public float Radius;
        
        public Rhino.Geometry.Sphere rhGeo;
        
        public override void OnEnable()
        {
            name = "Rhino Sphere Object";
        }

        public override void Update()
        {
            base.Update();
            
            rhGeo.Center = center.ToRhinoPoint();
            rhGeo.Radius = Radius;
            transform.localScale = Vector3.one * Radius;
        }
    }
}