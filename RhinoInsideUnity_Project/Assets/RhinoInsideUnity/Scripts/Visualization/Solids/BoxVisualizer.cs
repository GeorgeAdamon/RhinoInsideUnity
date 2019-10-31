using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Compute;
using Rhino.Geometry;
using RhinoInsideUnity.Extensions;
using RhinoInsideUnity.Geometries.Solids;
using Mesh = UnityEngine.Mesh;

namespace RhinoInsideUnity.Visualization
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Geometries.Solids.RhinoBox))]
    public class BoxVisualizer : MonoBehaviour
    {
        #region Public Variables

        [Tooltip("Number of subdivisions in U direction")] [Range(1, 100)]
        public int UCount = 10;

        [Tooltip("Number of subdivisions in V direction")] [Range(1, 100)]
        public int VCount = 10;

        [Tooltip("Number of subdivisions in W direction")] [Range(1, 100)]
        public int WCount = 10;

        #endregion

        #region Private Variables

        private Rhino.Geometry.Box baseBox;
        private Geometries.Solids.RhinoBox boxScript;
        public UnityEngine.Mesh unityMesh;
        private MeshFilter mf;
        private bool isDirty;
        #endregion

        void OnEnable()
        {
            mf = transform.GetOrAddComponent<MeshFilter>();
            unityMesh = new Mesh();
            
            boxScript = transform.GetOrAddComponent<Geometries.Solids.RhinoBox>();
            baseBox = new Rhino.Geometry.Box(Rhino.Geometry.Plane.WorldXY, new Interval(-0.5, 0.5),
                new Interval(-0.5, 0.5), new Interval(-0.5, 0.5));
        }

        private void Update()
        {
            if (isDirty)
            {
                ReconstructGeometry(true);
                isDirty = false;
            }
        }

        private void OnValidate()
        {
            isDirty = true;
        }

        private void ReconstructGeometry(bool requiresRemeshing)
        {
            RhinoComputeAuthorization.RequestAuthorization();
            MeshCompute.CreateFromBox(baseBox, UCount, VCount, WCount ).ToUnityMesh(unityMesh);
            unityMesh.name = "Mesh from Rhino Box";
            mf.mesh = unityMesh;
        }
    }
}