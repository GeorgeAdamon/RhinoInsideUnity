using UnityEngine;
using Rhino.Geometry;
using RhinoInsideUnity.Extensions;

namespace RhinoInsideUnity.Visualization
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class InfiniteLineVisualizer : MonoBehaviour
    {
        public Line line;
        private LineRenderer lr;

        [Range(0, 2)] public float IntersectionVisualizationWidth = 0.1f;
        [Range(0, 25)] public float IntersectionVisualizationLength = 10;

        void OnEnable()
        {
            lr = transform.GetOrAddComponent<LineRenderer>();
            lr.positionCount = 2;
        }

        void LateUpdate()
        {
            if (lr == null) lr = transform.GetOrAddComponent<LineRenderer>();

            if (line != null)
            {
                float len = IntersectionVisualizationLength;
                float wid = IntersectionVisualizationWidth;
                lr.SetPositions(new Vector3[] { line.From.ToUnityVector() + line.ToUnityVector() * -len, line.To.ToUnityVector() + line.ToUnityVector() * len });
                lr.startWidth = wid;
                lr.endWidth = wid;
            }
        }
    }
}