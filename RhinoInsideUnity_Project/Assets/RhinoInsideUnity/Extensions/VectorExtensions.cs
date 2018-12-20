using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Geometry;

namespace RhinoInsideUnity
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
        #endregion

        #endregion

        #region RHINO TO UNITY CONVERSIONS

        #region To Vector3
        /// <summary>
        /// Converts a Rhino.Geometry.Point3d object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoPoint"> The Rhino.Geometry.Vector3d to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Point3d rhinoPoint)
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
        /// Converts a Rhino.Geometry.Line object to a UnityEngine.Vector3 struct .
        /// </summary>
        /// <param name="rhinoVector"> The Rhino.Geometry.Line to convert.</param>
        /// <returns>A new UnityEngine.Vector3.</returns>
        public static Vector3 ToUnityVector(this Rhino.Geometry.Line rhinoLine, bool normalize = true)
        {
            return normalize ? (rhinoLine.To.ToUnityVector() - rhinoLine.From.ToUnityVector()).normalized : (rhinoLine.To.ToUnityVector() - rhinoLine.From.ToUnityVector());
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
