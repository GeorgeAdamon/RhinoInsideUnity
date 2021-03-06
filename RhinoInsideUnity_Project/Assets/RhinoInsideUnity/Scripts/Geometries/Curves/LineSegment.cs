﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Geometry;

namespace RhinoInsideUnity.Geometries
{
    [ExecuteInEditMode]
    public class LineSegment : MonoBehaviour
    {
        public Rhino.Geometry.Line line;
        public Vector3 From, To;
        public Color UniformColor;
        
        public bool OrientHandlesInWorldSpace = true;
        
    }
}
