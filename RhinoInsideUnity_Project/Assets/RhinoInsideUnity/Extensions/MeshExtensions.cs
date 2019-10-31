using System.Collections.Generic;
using UnityEngine;

namespace RhinoInsideUnity.Extensions
{
    public static class MeshExtensions
    {

        #region UNITY TO RHINO CONVERSIONS
        #endregion

        #region RHINO TO UNITY CONVERSIONS

        #region To Unity Mesh
        /// <summary>
        /// Converts a Rhino.Geometry.Mesh object to a UnityEngine.Mesh object.
        /// </summary>
        /// <param name="rhinoMesh"> The Rhino.Geometry.Mesh to convert.</param>
        /// <param name="unityMesh"> Optional mesh to pass, to avoid new allocations.</param>
        /// <param name="transform"> Optional transformation matrix.</param>
        /// <returns>A new instance of a UnityEngine.Mesh object.</returns>
        public static void ToUnityMesh(this Rhino.Geometry.Mesh rhinoMesh, UnityEngine.Mesh unityMesh = null, UnityEngine.Transform transform = null)
        {
            if (unityMesh == null)
            {
                unityMesh = new UnityEngine.Mesh();
            }
            else
            {
                unityMesh.Clear();
            }

            var vertexCount = rhinoMesh.Vertices.Count;
            var faceCount = rhinoMesh.Faces.Count;
            var normalCount = rhinoMesh.Normals.Count;
            var uvCount = rhinoMesh.TextureCoordinates.Count;
            var colorCount = rhinoMesh.VertexColors.Count;
            
            // Lists for passing to the Unity.Mesh API
            var vertices = new List<Vector3>(vertexCount);
            var triangles = new List<int>(faceCount*4);
            var normals = new List<Vector3>(normalCount);
            var uvs = new List<Vector2>(uvCount);
            var colors = new List<Color>(colorCount);

            // Temporary allocations to avoid calling List<T>.Add
            var tempVerts = new Vector3[vertexCount];
            var tempNorms = new Vector3[normalCount];
            var threePack = new int[3];
            var sixPack = new int[6];
            var tempUv = new Vector2[uvCount];
            var tempCol = new Color[colorCount];
            
            #region Convert Vertices
            if (transform != null)
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    tempVerts[i] = transform.InverseTransformPoint(rhinoMesh.Vertices[i].ToUnityVector());
                }
            }
            else
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    tempVerts[i] = rhinoMesh.Vertices[i].ToUnityVector();
                }
            }
            
            vertices.AddRange(tempVerts);
            unityMesh.SetVertices(vertices);
            #endregion


            #region Convert Faces
            
            for (var i = 0; i < faceCount; i++)
            {
                var face = rhinoMesh.Faces[i];

                if (face.IsTriangle)
                {
                    threePack[0] = face.A;
                    threePack[1] = face.C;
                    threePack[2] = face.B;
                    
                    triangles.AddRange(threePack);
                }
                else
                {
                    sixPack[0] = face.A;
                    sixPack[1] = face.D;
                    sixPack[2] = face.B;
                    sixPack[3] = face.B;
                    sixPack[4] = face.D;
                    sixPack[5] = face.C;
                    
                    triangles.AddRange(sixPack);
                }
            }
            unityMesh.SetTriangles(triangles, 0);
            #endregion
            
            #region Convert Normals
            if (normalCount > 0 )
            {
                for (int i = 0; i < normalCount; i++)
                {
                    tempNorms[i] = rhinoMesh.Normals[i].ToUnityVector();
                }
                normals.AddRange(tempNorms);
                unityMesh.SetNormals(normals);
            }
            else
            {
                unityMesh.RecalculateNormals();
            }
            #endregion
            
            
            #region Convert UVs
            if (uvCount > 0)
            {
                for (var i = 0; i < uvCount; i++)
                {
                    tempUv[i] = rhinoMesh.TextureCoordinates[i].ToUnityVector();
                }
                
                uvs.AddRange(tempUv);
                unityMesh.SetUVs(0, uvs);
            }
            #endregion

            #region Convert Colors
            if (colorCount > 0)
            {
                for (var i=0; i<colorCount;i++)
                {
                    tempCol[i] = rhinoMesh.VertexColors[i].ToUnityColor();
                }
                
                colors.AddRange(tempCol);
                unityMesh.SetColors(colors);
            }
            #endregion
            
            unityMesh.RecalculateTangents();
            unityMesh.RecalculateBounds();

        }
        #endregion
        
        #endregion

    }
}
