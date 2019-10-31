using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Compute;
using Rhino.Geometry;
using RhinoInsideUnity.Extensions;
using RhinoInsideUnity.Geometries.Solids;

namespace RhinoInsideUnity.Visualization
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Geometries.Solids.RhinoSphere))]
    public class SphereVisualizer : MonoBehaviour
    {
        #region Public Variables

        [Tooltip("Number of subdivisions in U direction")] [Range(1, 100)]
        public int UCount = 10;

        [Tooltip("Number of subdivisions in V direction")] [Range(1, 100)]
        public int VCount = 10;

        #endregion

        #region Private Variables

        private Rhino.Geometry.Sphere baseSphere;
        private Geometries.Solids.RhinoSphere _rhinoSphereScript;
        public UnityEngine.Mesh unityMesh;
        private MeshFilter mf;

        #endregion

        void OnEnable()
        {
            mf = transform.GetOrAddComponent<MeshFilter>();
            _rhinoSphereScript = transform.GetOrAddComponent<Geometries.Solids.RhinoSphere>();
            baseSphere = new Rhino.Geometry.Sphere(new Point3d(0, 0, 0), 1);
            RhinoComputeAuthorization.RequestAuthorization();
        }

        private void OnValidate()
        {
            ReconstructGeometry(true);
        }

        public void ReconstructGeometry(bool requiresRemeshing)
        {
            MeshCompute.CreateFromSphere(baseSphere, UCount, VCount).ToUnityMesh(unityMesh);
            unityMesh.name = "Mesh from Rhino Sphere";
            mf.mesh = unityMesh;
        }
    }
}