using Rhino.Geometry;
using RhinoInsideUnity.Extensions;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace RhinoInsideUnity.Geometries.Solids
{
    [ExecuteInEditMode]
    public class RhinoBox : RhinoPrimitiveBase
    {
        public Vector3 Size;
        
        public Rhino.Geometry.Box rhGeo;
        
        public override void OnEnable()
        {
            name = "Rhino Box Object";
            rhGeo = new Rhino.Geometry.Box(transform.ToRhinoPlane(), new Interval(-Size.x * 0.5, Size.x * 0.5),
                new Interval(-Size.y * 0.5, Size.y * 0.5), new Interval(-Size.z * 0.5, Size.z * 0.5));
        }

        public override void Update()
        {
            base.Update();
            Transform transform1;
            rhGeo.Plane = (transform1 = transform).ToRhinoPlane();
            rhGeo.X = new Interval(-Size.x * 0.5, Size.x * 0.5);
            rhGeo.Y = new Interval(-Size.y * 0.5, Size.y * 0.5);
            rhGeo.Z = new Interval(-Size.z * 0.5, Size.z * 0.5);
            transform1.localScale =  Size;
        }
    }
}
