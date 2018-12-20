using Rhino.Geometry.Intersect;
using RhinoInsideUnity;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(LineRenderer))]
public class PlanePlaneIntersectionExample : MonoBehaviour
{
    public Transform ObjectA;
    public Transform ObjectB;

    private Rhino.Geometry.Plane PlaneA;
    private Rhino.Geometry.Plane PlaneB;
    private Rhino.Geometry.Line IntersectionLine;

    [Range(0, 2)] public float IntersectionVisualizationWidth = 100;
    [Range(0, 25)] public float IntersectionVisualizationLength = 100;

    private LineRenderer lr;

    private void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;

        PlaneA = new Rhino.Geometry.Plane();
        PlaneB = new Rhino.Geometry.Plane();
    }

    private void Update()
    {
        float l = IntersectionVisualizationLength;
        float w = IntersectionVisualizationWidth;

        // Usage of RhinoCommon
        PlaneA.Origin = ObjectA.position.ToRhinoPoint();
        PlaneA.ZAxis = ObjectA.up.ToRhinoVector();

        PlaneB.Origin = ObjectB.position.ToRhinoPoint();
        PlaneB.ZAxis = ObjectB.up.ToRhinoVector();

        Intersection.PlanePlane(PlaneA, PlaneB, out Rhino.Geometry.Line IntersectionLine);

        // Update the line renderer with the intersection information
        lr.SetPositions(new Vector3[] { IntersectionLine.From.ToUnityVector() + IntersectionLine.ToUnityVector() * -l, IntersectionLine.To.ToUnityVector() + IntersectionLine.ToUnityVector() * l });
        lr.startWidth = w;
        lr.endWidth = w;
    }

}
