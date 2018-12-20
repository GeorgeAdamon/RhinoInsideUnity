using Rhino.Geometry.Intersect;
using RhinoInsideUnity;
using UnityEngine;
using RhinoInsideUnity.Visualization;

namespace RhinoInsideUnity.Intersections
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(InfiniteLineVisualizer))]
    public class PlanePlane : MonoBehaviour
    {
        public Transform PlaneA;
        public Transform PlaneB;
        public bool VisualizeResult = true;

        private Rhino.Geometry.Plane _PlaneA;
        private Rhino.Geometry.Plane _PlaneB;
        public Rhino.Geometry.Line IntersectionLine;

        InfiniteLineVisualizer viz;

        private void OnEnable()
        {
            _PlaneA = new Rhino.Geometry.Plane();
            _PlaneB = new Rhino.Geometry.Plane();
            viz = GetComponent<InfiniteLineVisualizer>();
        }

        private void Update()
        {
            if (PlaneA != null && PlaneB != null)
            {
                PlaneA.ToRhinoPlaneNonAlloc(ref _PlaneA);
                PlaneB.ToRhinoPlaneNonAlloc(ref _PlaneB);
                Intersection.PlanePlane(_PlaneA, _PlaneB, out Rhino.Geometry.Line IntersectionLine);


                if (VisualizeResult)
                {
                    Visualize(ref IntersectionLine);
                }
                else
                {
                    if (viz.enabled) viz.enabled = false;
                }
            }
        }

        private void Visualize(ref Rhino.Geometry.Line line)
        {
            if (!viz.enabled) viz.enabled = true;
            viz.line = line;

        }
    }
}
