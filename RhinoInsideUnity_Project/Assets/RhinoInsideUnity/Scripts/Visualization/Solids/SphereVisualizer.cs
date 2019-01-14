using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Compute;
using Rhino.Geometry;
using RhinoInsideUnity.Geometries.Solids;

namespace RhinoInsideUnity.Visualization
{
    [ExecuteInEditMode]
    [RequireComponent( typeof(MeshFilter), typeof(MeshRenderer), typeof(Geometries.Solids.Sphere))]
    public class SphereVisualizer : MonoBehaviour
    {
        #region Public Variables
        [Tooltip("Number of subdivisions in U direction")] [Range(1, 100)] public int UCount = 10;
        [Tooltip("Number of subdivisions in V direction")] [Range(1, 100)] public int VCount = 10;
        #endregion

        #region Private Variables
        private Geometries.Solids.Sphere sphereScript;
        public UnityEngine.Mesh unityMesh;
        private MeshFilter mf;

        #endregion

        void OnEnable()
        {
            mf = transform.GetOrAddComponent<MeshFilter>();
            sphereScript = transform.GetOrAddComponent<Geometries.Solids.Sphere>();
        }

        public void ReconstructGeometry(bool requiresRemeshing)
        {
                RhinoComputeAuthorization.RequestAuthorization();

                if (unityMesh != null)
                {
                    Rhino.Compute.MeshCompute.CreateFromSphere(new Rhino.Geometry.Sphere(new Point3d(0,0,0), 1), UCount, VCount).ToUnityMesh(ref unityMesh);
                }

                else
                {
                    unityMesh = Rhino.Compute.MeshCompute.CreateFromSphere(new Rhino.Geometry.Sphere(new Point3d(0, 0, 0), 1), UCount, VCount).ToUnityMesh();
                }

            unityMesh.name = "Mesh from Rhino Sphere";
            mf.sharedMesh = unityMesh;
        }
    }
}
