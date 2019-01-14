using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino.Geometry;

namespace RhinoInsideUnity
{
    public static class MeshExtensions
    {

        #region UNITY TO RHINO CONVERSIONS
        #endregion

        #region RHINO TO UNITY CONVERSIONS

        #region To Unity Mesh

        #region Without Transform
        /// <summary>
        /// Converts a Rhino.Geometry.Mesh object to a UnityEngine.Mesh object.
        /// </summary>
        /// <param name="rhinoMesh"> The Rhino.Geometry.Mesh to convert.</param>
        /// <returns>A new instance of a UnityEngine.Mesh object.</returns>
        public static UnityEngine.Mesh ToUnityMesh(this Rhino.Geometry.Mesh rhinoMesh)
        {
            UnityEngine.Mesh unityMesh = new UnityEngine.Mesh();

            int vertexCount = rhinoMesh.Vertices.Count;
            int faceCount = rhinoMesh.Faces.Count;
            int normalCount = rhinoMesh.Normals.Count;
            int uvCount = rhinoMesh.TextureCoordinates.Count;
            int colorCount = rhinoMesh.VertexColors.Count;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();

            #region Convert Vertices

            for (int i = 0; i < vertexCount; i++)
            {
                vertices.Add(rhinoMesh.Vertices[i].ToUnityVector());
            }

            unityMesh.SetVertices(vertices);

            #endregion


            #region Convert Faces

            for (int i = 0; i < faceCount; i++)
            {
                Rhino.Geometry.MeshFace face = rhinoMesh.Faces[i];

                if (face.IsTriangle)
                {
                    triangles.Add(face.A);
                    triangles.Add(face.C);
                    triangles.Add(face.B);
                }
                else
                {
                    triangles.Add(face.A);
                    triangles.Add(face.D);
                    triangles.Add(face.B);

                    triangles.Add(face.B);
                    triangles.Add(face.D);
                    triangles.Add(face.C);
                }
            }

            unityMesh.SetTriangles(triangles, 0);

            #endregion




            /*
            
            #region Convert Normals
            if (normalCount > 0 )
            {
                for (int i = 0; i < normalCount; i++)
                {
                    normals.Add(rhinoMesh.Normals[i].ToUnityVector());
                }
            }
            else
            {
                unityMesh.RecalculateNormals();
            }
            #endregion
    */
            #region Convert UVs

            if (uvCount > 0)
            {
                for (int i = 0; i < uvCount; i++)
                {
                    uvs.Add(rhinoMesh.TextureCoordinates[i].ToUnityVector());
                }
            }

            #endregion

            #region Convert Colors
            if (colorCount > 0)
            {
                for (int i=0; i<colorCount;i++)
                {
                    colors.Add(rhinoMesh.VertexColors[i].ToUnityColor());
                }
            }

            unityMesh.SetColors(colors);

            #endregion

            unityMesh.RecalculateNormals();
            unityMesh.RecalculateTangents();
            unityMesh.RecalculateBounds();

            return unityMesh;
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Mesh object to a UnityEngine.Mesh object, and assigns it to an existing reference.
        /// </summary>
        /// <param name="rhinoMesh"> The Rhino.Geometry.Mesh to convert.</param>
        /// <param name="unityMesh"> The reference to an existing UnityEngine.Mesh instance.</param>
        /// <returns>A new instance of a UnityEngine.Mesh object.</returns>
        public static void ToUnityMesh(this Rhino.Geometry.Mesh rhinoMesh, ref UnityEngine.Mesh unityMesh)
        {
            unityMesh = new UnityEngine.Mesh();

            int vertexCount = rhinoMesh.Vertices.Count;
            int faceCount = rhinoMesh.Faces.Count;
            int normalCount = rhinoMesh.Normals.Count;
            int uvCount = rhinoMesh.TextureCoordinates.Count;
            int colorCount = rhinoMesh.VertexColors.Count;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();


            #region Convert Vertices

            for (int i = 0; i < vertexCount; i++)
            {
                vertices.Add(rhinoMesh.Vertices[i].ToUnityVector());
            }

            unityMesh.SetVertices(vertices);

            #endregion

            #region Convert Faces

            for (int i = 0; i < faceCount; i++)
            {
                Rhino.Geometry.MeshFace face = rhinoMesh.Faces[i];

                if (face.IsTriangle)
                {
                    triangles.Add(face.A);
                    triangles.Add(face.C);
                    triangles.Add(face.B);
                }
                else
                {
                    triangles.Add(face.A);
                    triangles.Add(face.D);
                    triangles.Add(face.B);

                    triangles.Add(face.B);
                    triangles.Add(face.D);
                    triangles.Add(face.C);
                }
            }

            unityMesh.SetTriangles(triangles, 0);

            #endregion
            /*
            #region Convert Normals

            if (normalCount > 0)
            {
                for (int i = 0; i < normalCount; i++)
                {
                    normals.Add(rhinoMesh.Normals[i].ToUnityVector());
                }
            }
            else
            {
                unityMesh.RecalculateNormals();
            }
           


            #endregion
     */
            #region Convert UVs

            if (uvCount > 0)
            {
                for (int i = 0; i < uvCount; i++)
                {
                    uvs.Add(rhinoMesh.TextureCoordinates[i].ToUnityVector());
                }
            }

            #endregion

            #region Convert Colors
            if (colorCount > 0)
            {
                for (int i = 0; i < colorCount; i++)
                {
                    colors.Add(rhinoMesh.VertexColors[i].ToUnityColor());
                }
            }

            unityMesh.SetColors(colors);

            #endregion

            unityMesh.RecalculateNormals();
            unityMesh.RecalculateTangents();
            unityMesh.RecalculateBounds();
        }
        #endregion

        #region With Transform
        /// <summary>
        /// Converts a Rhino.Geometry.Mesh object to a UnityEngine.Mesh object.
        /// </summary>
        /// <param name="rhinoMesh"> The Rhino.Geometry.Mesh to convert.</param>
        /// <returns>A new instance of a UnityEngine.Mesh object.</returns>
        public static UnityEngine.Mesh ToUnityMesh(this Rhino.Geometry.Mesh rhinoMesh, UnityEngine.Transform _transform)
        {
            UnityEngine.Mesh unityMesh = new UnityEngine.Mesh();

            int vertexCount = rhinoMesh.Vertices.Count;
            int faceCount = rhinoMesh.Faces.Count;
            int normalCount = rhinoMesh.Normals.Count;
            int uvCount = rhinoMesh.TextureCoordinates.Count;
            int colorCount = rhinoMesh.VertexColors.Count;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();

            #region Convert Vertices

            for (int i = 0; i < vertexCount; i++)
            {
                vertices.Add(_transform.InverseTransformPoint(rhinoMesh.Vertices[i].ToUnityVector()));
            }

            unityMesh.SetVertices(vertices);

            #endregion

            #region Convert Faces

            for (int i = 0; i < faceCount; i++)
            {
                Rhino.Geometry.MeshFace face = rhinoMesh.Faces[i];

                if (face.IsTriangle)
                {
                    triangles.Add(face.A);
                    triangles.Add(face.C);
                    triangles.Add(face.B);
                }
                else
                {
                    triangles.Add(face.A);
                    triangles.Add(face.D);
                    triangles.Add(face.B);

                    triangles.Add(face.B);
                    triangles.Add(face.D);
                    triangles.Add(face.C);
                }
            }

            unityMesh.SetTriangles(triangles, 0);

            #endregion

            /*
            
            #region Convert Normals
            if (normalCount > 0 )
            {
                for (int i = 0; i < normalCount; i++)
                {
                    normals.Add(rhinoMesh.Normals[i].ToUnityVector());
                }
            }
            else
            {
                unityMesh.RecalculateNormals();
            }
            #endregion
    */
            #region Convert UVs

            if (uvCount > 0)
            {
                for (int i = 0; i < uvCount; i++)
                {
                    uvs.Add(rhinoMesh.TextureCoordinates[i].ToUnityVector());
                }
            }

            #endregion

            #region Convert Colors
            if (colorCount > 0)
            {
                for (int i = 0; i < colorCount; i++)
                {
                    colors.Add(rhinoMesh.VertexColors[i].ToUnityColor());
                }
            }

            unityMesh.SetColors(colors);

            #endregion

            unityMesh.RecalculateNormals();
            unityMesh.RecalculateTangents();
            unityMesh.RecalculateBounds();

            return unityMesh;
        }

        /// <summary>
        /// Converts a Rhino.Geometry.Mesh object to a UnityEngine.Mesh object, and assigns it to an existing reference.
        /// </summary>
        /// <param name="rhinoMesh"> The Rhino.Geometry.Mesh to convert.</param>
        /// <param name="unityMesh"> The reference to an existing UnityEngine.Mesh instance.</param>
        /// <returns>A new instance of a UnityEngine.Mesh object.</returns>
        public static void ToUnityMesh(this Rhino.Geometry.Mesh rhinoMesh, ref UnityEngine.Mesh unityMesh, UnityEngine.Transform _transform)
        {

            int vertexCount = rhinoMesh.Vertices.Count;
            int faceCount = rhinoMesh.Faces.Count;
            int normalCount = rhinoMesh.Normals.Count;
            int uvCount = rhinoMesh.TextureCoordinates.Count;
            int colorCount = rhinoMesh.VertexColors.Count;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();

            #region Convert Vertices

            for (int i = 0; i < vertexCount; i++)
            {
                vertices.Add( _transform.InverseTransformPoint (rhinoMesh.Vertices[i].ToUnityVector()));
            }

            unityMesh.SetVertices(vertices);

            #endregion

            #region Convert Faces

            for (int i = 0; i < faceCount; i++)
            {
                Rhino.Geometry.MeshFace face = rhinoMesh.Faces[i];

                if (face.IsTriangle)
                {
                    triangles.Add(face.A);
                    triangles.Add(face.C);
                    triangles.Add(face.B);
                }
                else
                {
                    triangles.Add(face.A);
                    triangles.Add(face.D);
                    triangles.Add(face.B);

                    triangles.Add(face.B);
                    triangles.Add(face.D);
                    triangles.Add(face.C);
                }
            }

            unityMesh.SetTriangles(triangles, 0);

            #endregion

            /*
            #region Convert Normals

            if (normalCount > 0)
            {
                for (int i = 0; i < normalCount; i++)
                {
                    normals.Add(rhinoMesh.Normals[i].ToUnityVector());
                }
            }
            else
            {
                unityMesh.RecalculateNormals();
            }
           


            #endregion
     */
            #region Convert UVs

            if (uvCount > 0)
            {
                for (int i = 0; i < uvCount; i++)
                {
                    uvs.Add(rhinoMesh.TextureCoordinates[i].ToUnityVector());
                }
            }

            #endregion

            #region Convert Colors
            if (colorCount > 0)
            {
                for (int i = 0; i < colorCount; i++)
                {
                    colors.Add(rhinoMesh.VertexColors[i].ToUnityColor());
                }
            }

            unityMesh.SetColors(colors);

            #endregion

            unityMesh.RecalculateNormals();
            unityMesh.RecalculateTangents();
            unityMesh.RecalculateBounds();
        }
        #endregion

        #endregion

        #endregion
    }
}
