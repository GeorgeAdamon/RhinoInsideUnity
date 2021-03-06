﻿using Rhino.Geometry;
using UnityEngine;

namespace RhinoInsideUnity.Extensions
{
    public static class VectorExtensions
    {
        #region UNITY TO RHINO CONVERSIONS

        #region To Point3d
        /// <summary>
        /// Converts a UnityEngine.Vector3 struct to a Rhino.Geometry.Point3d object.
        /// </summary>
        /// <param name="unityVector"> The Unity Vector to convert.</param>
        /// <returns>A new instance of a Rhino.Geometry.Point3d object.</returns>
        public static Rhino.Geometry.Point3d ToRhinoPoint(this Vector3 unityVector)
        {
            return new Point3d(unityVector.x, unityVector.z, unityVector.y);
        }
        #endregion

        #region To Vector3d
        /// <summary>
        /// Converts a UnityEngine.Vector3 struct to a Rhino.Geometry.Vector3d object.
        /// </summary>
        /// <param name="unityVector"> The Unity Vector to convert.</param>
        /// <returns>A new instance of a Rhino.Geometry.Vector3d object.</returns>
        public static Rhino.Geometry.Vector3d ToRhinoVector(this Vector3 unityVector)
        {
            return new Vector3d(unityVector.x, unityVector.z, unityVector.y);
        }
        #endregion

        #region To Plane
        /// <summary>
        /// Converts a UnityEngine.Plane object to a Rhino.Geometry.Plane object.
        /// </summary>
        /// <param name="unityVector"> The UnityEngine.Plane to convert.</param>
        /// <returns>A new instance of a Rhino.Geometry.Plane object.</returns>
        public static Rhino.Geometry.Plane ToRhinoPlane(this UnityEngine.Plane unityPlane)
        {
            Vector3d direction = unityPlane.normal.ToRhinoVector();
            Point3d origin = Vector3.zero.ToRhinoPoint() + direction * unityPlane.distance;

            return new Rhino.Geometry.Plane(origin, direction);
        }

        /// <summary>
        /// Converts a UnityEngine.Transform object to a Rhino.Geometry.Plane object.
        /// </summary>
        /// <param name="unityVector"> The UnityEngine.Transform to convert.</param>
        /// <returns>A new instance of a Rhino.Geometry.Plane object.</returns>
        public static Rhino.Geometry.Plane ToRhinoPlane(this UnityEngine.Transform unityTransform)
        {
            Rhino.Geometry.Plane rhinoPlane = new Rhino.Geometry.Plane();
            rhinoPlane.Origin = unityTransform.position.ToRhinoPoint();
            rhinoPlane.ZAxis = unityTransform.up.ToRhinoVector();
            rhinoPlane.XAxis = unityTransform.right.ToRhinoVector();
            return rhinoPlane;
        }

        /// <summary>
        /// Converts a UnityEngine.Transform object to a Rhino.Geometry.Plane object, and passes it to an existing Plane object (no memory allocations).
        /// </summary>
        /// <param name="unityVector"> The UnityEngine.Transform to convert.</param>
        /// <returns> True on success.</returns>
        public static bool ToRhinoPlaneNonAlloc(this UnityEngine.Transform unityTransform, ref  Rhino.Geometry.Plane rhinoPlane)
        {
            rhinoPlane.Origin = unityTransform.position.ToRhinoPoint();
            rhinoPlane.ZAxis = unityTransform.up.ToRhinoVector();
            rhinoPlane.XAxis = unityTransform.right.ToRhinoVector();
            return true;
        }

        #endregion

        #endregion

        #region RHINO TO UNITY CONVERSIONS

        #region To Vector3
        /// <summary>
        /// Converts a Rhino.Geometry.Point3d object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Point3d to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Point3d rhinoPoint)
        {
            return new Vector3((float)rhinoPoint.X, (float)rhinoPoint.Z, (float)rhinoPoint.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Point3f object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Point3f  to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Point3f rhinoPoint)
        {
            return new Vector3((float)rhinoPoint.X, (float)rhinoPoint.Z, (float)rhinoPoint.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Vector3d object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoVector"> The Rhino.Geometry.Vector3d to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Vector3d rhinoVector)
        {
            return new Vector3((float)rhinoVector.X, (float)rhinoVector.Z, (float)rhinoVector.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Vector3f object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Vector3f  to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Vector3f rhinoVector)
        {
            return new Vector3((float)rhinoVector.X, (float)rhinoVector.Z, (float)rhinoVector.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Line object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoVector"> The Rhino.Geometry.Line to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Line rhinoLine, bool normalize = true)
        {
            return normalize ? (rhinoLine.To.ToUnityVector() - rhinoLine.From.ToUnityVector()).normalized : (rhinoLine.To.ToUnityVector() - rhinoLine.From.ToUnityVector());
        }
        #endregion

        #region To Vector2
        /// <summary>
        /// Converts a Rhino.Geometry.Point2d object to a UnityEngine.Vector2 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Point2d to convert.</param>
        /// <returns>A new UnityEngine.Vector2.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Point2d rhinoPoint)
        {
            return new Vector2((float)rhinoPoint.X, (float)rhinoPoint.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Point2f object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Point2f  to convert.</param>
        /// <returns>A new UnityEngine.Vector2.</returns>
        public static Vector2 ToUnityVector(this Rhino.Geometry.Point2f rhinoPoint)
        {
            return new Vector2((float)rhinoPoint.X, (float)rhinoPoint.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Vector2d object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoVector"> The Rhino.Geometry.Vector2d to convert.</param>
        /// <returns>A new UnityEngine.Vector2.</returns>
        public static Vector2 ToUnityVector(this Rhino.Geometry.Vector2d rhinoVector)
        {
            return new Vector2((float)rhinoVector.X, (float)rhinoVector.Y);
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Vector2f object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Vector2f  to convert.</param>
        /// <returns>A new UnityEngine.Vector2.</returns>
        public static Vector2 ToUnityVector(this Rhino.Geometry.Vector2f rhinoVector)
        {
            return new Vector2((float)rhinoVector.X, (float)rhinoVector.Y);
        }
        #endregion

        #region To Plane
        /// <summary>
        /// Converts a Rhino.Geometry.Plane object to a UnityEngine.Plane object.
        /// </summary>
        /// <param name="rhinoPlane"> The Rhino.Geometry.Plane to convert.</param>
        /// <returns>A new instance of a UnityEngine.Plane object.</returns>
        public static UnityEngine.Plane ToUnityPlane(this Rhino.Geometry.Plane rhinoPlane)
        {
            Vector3 direction = rhinoPlane.Normal.ToUnityVector();
            Vector3 origin = rhinoPlane.Origin.ToUnityVector();

            return new UnityEngine.Plane(direction, origin);
        }
        #endregion

        #endregion
    }

}
