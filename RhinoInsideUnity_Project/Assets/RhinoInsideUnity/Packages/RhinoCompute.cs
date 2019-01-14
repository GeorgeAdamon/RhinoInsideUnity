using System;
using System.IO;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rhino.Compute
{
    public static class ComputeServer
    {
        public static string WebAddress { get; set; } = "https://compute.rhino3d.com";
        public static string AuthToken { get; set; }

        public static T Post<T>(string function, params object[] postData)
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                throw new UnauthorizedAccessException("AuthToken must be set");
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            if (!function.StartsWith("/"))
                function = "/" + function;
            string uri = (WebAddress + function).ToLower();
            var request = System.Net.WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + AuthToken);
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var rc = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
                return rc;
            }
        }

        public static T0 Post<T0, T1>(string function, out T1 out1, params object[] postData)
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                throw new UnauthorizedAccessException("AuthToken must be set");
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            if (!function.StartsWith("/"))
                function = "/" + function;
            string uri = (WebAddress + function).ToLower();
            var request = System.Net.WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + AuthToken);
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                object data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
                var ja = data as Newtonsoft.Json.Linq.JArray;
                out1 = ja[1].ToObject<T1>();
                return ja[0].ToObject<T0>();
            }
        }

        public static T0 Post<T0, T1, T2>(string function, out T1 out1, out T2 out2, params object[] postData)
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                throw new UnauthorizedAccessException("AuthToken must be set");
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            if (!function.StartsWith("/"))
                function = "/" + function;
            string uri = (WebAddress + function).ToLower();
            var request = System.Net.WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + AuthToken);
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                object data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
                var ja = data as Newtonsoft.Json.Linq.JArray;
                out1 = ja[1].ToObject<T1>();
                out2 = ja[2].ToObject<T2>();
                return ja[0].ToObject<T0>();
            }
        }

        public static string ApiAddress(Type t, string function)
        {
            string s = t.ToString().Replace('.', '/');
            return s + "/" + function;
        }
    }

    public static class ExtrusionCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(Extrusion), caller);
        }
        /// <summary>
        /// Constructs all the Wireframe curves for this Extrusion.
        /// </summary>
        /// <returns>An array of Wireframe curves.</returns>
        public static Curve[] GetWireframe(this Extrusion extrusion)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), extrusion);
        }
    }

    public static class BezierCurveCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(BezierCurve), caller);
        }
        /// <summary>
        /// Constructs an array of cubic, non-rational beziers that fit a curve to a tolerance.
        /// </summary>
        /// <param name="sourceCurve">A curve to approximate.</param>
        /// <param name="distanceTolerance">
        /// The max fitting error. Use RhinoMath.SqrtEpsilon as a minimum.
        /// </param>
        /// <param name="kinkTolerance">
        /// If the input curve has a g1-discontinuity with angle radian measure
        /// greater than kinkTolerance at some point P, the list of beziers will
        /// also have a kink at P.
        /// </param>
        /// <returns>A new array of bezier curves. The array can be empty and might contain null items.</returns>
        public static BezierCurve[] CreateCubicBeziers(Curve sourceCurve, double distanceTolerance, double kinkTolerance)
        {
            return ComputeServer.Post<BezierCurve[]>(ApiAddress(), sourceCurve, distanceTolerance, kinkTolerance);
        }
        /// <summary>
        /// Create an array of Bezier curves that fit to an existing curve. Please note, these
        /// Beziers can be of any order and may be rational.
        /// </summary>
        /// <param name="sourceCurve">The curve to fit Beziers to</param>
        /// <returns>A new array of Bezier curves</returns>
        public static BezierCurve[] CreateBeziers(Curve sourceCurve)
        {
            return ComputeServer.Post<BezierCurve[]>(ApiAddress(), sourceCurve);
        }
    }

    public static class BrepCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(Brep), caller);
        }
        /// <summary>
        /// Change the seam of a closed trimmed surface.
        /// </summary>
        /// <param name="face">A Brep face with a closed underlying surface.</param>
        /// <param name="direction">The parameter direction (0 = U, 1 = V). The face's underlying surface must be closed in this direction.</param>
        /// <param name="parameter">The parameter at which to place the seam.</param>
        /// <param name="tolerance">Tolerance used to cut up surface.</param>
        /// <returns>A new Brep that has the same geoemtry as the face with a relocated seam if successful, or null on failure.</returns>
        public static Brep ChangeSeam(BrepFace face, int direction, double parameter, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), face, direction, parameter, tolerance);
        }
        /// <summary>
        /// Copy all trims from a Brep face onto a surface.
        /// </summary>
        /// <param name="trimSource">Brep face which defines the trimming curves.</param>
        /// <param name="surfaceSource">The surface to trim.</param>
        /// <param name="tolerance">Tolerance to use for rebuilding 3D trim curves.</param>
        /// <returns>A brep with the shape of surfaceSource and the trims of trimSource or null on failure.</returns>
        public static Brep CopyTrimCurves(BrepFace trimSource, Surface surfaceSource, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), trimSource, surfaceSource, tolerance);
        }
        /// <summary>
        /// Creates a brep representation of the sphere with two similar trimmed NURBS surfaces, and no singularities.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="tolerance">
        /// Used in computing 2d trimming curves. If &gt;= 0.0, then the max of 
        /// ON_0.0001 * radius and RhinoMath.ZeroTolerance will be used.
        /// </param>
        /// <returns>A new brep, or null on error.</returns>
        public static Brep CreateBaseballSphere(Point3d center, double radius, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), center, radius, tolerance);
        }
        /// <summary>
        /// Creates a single developable surface between two curves.
        /// </summary>
        /// <param name="crv0">The first rail curve.</param>
        /// <param name="crv1">The second rail curve.</param>
        /// <param name="reverse0">Reverse the first rail curve.</param>
        /// <param name="reverse1">Reverse the second rail curve</param>
        /// <param name="density">The number of rulings across the surface.</param>
        /// <returns>The output Breps if successful, otherwise an empty array.</returns>
        public static Brep[] CreateDevelopableLoft(Curve crv0, Curve crv1, bool reverse0, bool reverse1, int density)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), crv0, crv1, reverse0, reverse1, density);
        }
        /// <summary>
        /// Creates a single developable surface between two curves.
        /// </summary>
        /// <param name="rail0">The first rail curve.</param>
        /// <param name="rail1">The second rail curve.</param>
        /// <param name="fixedRulings">
        /// Rulings define lines across the surface that define the straight sections on the developable surface,
        /// where rulings[i].X = parameter on first rail curve, and rulings[i].Y = parameter on second rail curve.
        /// Note, rulings will be automatically adjusted to minimum twist.
        /// </param>
        /// <returns>The output Breps if successful, otherwise an empty array.</returns>
        public static Brep[] CreateDevelopableLoft(NurbsCurve rail0, NurbsCurve rail1, IEnumerable<Point2d> fixedRulings)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail0, rail1, fixedRulings);
        }
        /// <summary>
        /// Constructs a set of planar breps as outlines by the loops.
        /// </summary>
        /// <param name="inputLoops">Curve loops that delineate the planar boundaries.</param>
        /// <returns>An array of Planar Breps.</returns>
        public static Brep[] CreatePlanarBreps(IEnumerable<Curve> inputLoops)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), inputLoops);
        }
        /// <summary>
        /// Constructs a set of planar breps as outlines by the loops.
        /// </summary>
        /// <param name="inputLoops">Curve loops that delineate the planar boundaries.</param>
        /// <param name="tolerance"></param>
        /// <returns>An array of Planar Breps.</returns>
        public static Brep[] CreatePlanarBreps(IEnumerable<Curve> inputLoops, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), inputLoops, tolerance);
        }
        /// <summary>
        /// Constructs a set of planar breps as outlines by the loops.
        /// </summary>
        /// <param name="inputLoop">A curve that should form the boundaries of the surfaces or polysurfaces.</param>
        /// <returns>An array of Planar Breps.</returns>
        public static Brep[] CreatePlanarBreps(Curve inputLoop)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), inputLoop);
        }
        /// <summary>
        /// Constructs a set of planar breps as outlines by the loops.
        /// </summary>
        /// <param name="inputLoop">A curve that should form the boundaries of the surfaces or polysurfaces.</param>
        /// <param name="tolerance"></param>
        /// <returns>An array of Planar Breps.</returns>
        public static Brep[] CreatePlanarBreps(Curve inputLoop, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), inputLoop, tolerance);
        }
        /// <summary>
        /// Constructs a Brep using the trimming information of a brep face and a surface. 
        /// Surface must be roughly the same shape and in the same location as the trimming brep face.
        /// </summary>
        /// <param name="trimSource">BrepFace which contains trimmingSource brep.</param>
        /// <param name="surfaceSource">Surface that trims of BrepFace will be applied to.</param>
        /// <returns>A brep with the shape of surfaceSource and the trims of trimSource or null on failure.</returns>
        public static Brep CreateTrimmedSurface(BrepFace trimSource, Surface surfaceSource)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), trimSource, surfaceSource);
        }
        /// <summary>
        /// Constructs a Brep using the trimming information of a brep face and a surface. 
        /// Surface must be roughly the same shape and in the same location as the trimming brep face.
        /// </summary>
        /// <param name="trimSource">BrepFace which contains trimmingSource brep.</param>
        /// <param name="surfaceSource">Surface that trims of BrepFace will be applied to.</param>
        /// <param name="tolerance"></param>
        /// <returns>A brep with the shape of surfaceSource and the trims of trimSource or null on failure.</returns>
        public static Brep CreateTrimmedSurface(BrepFace trimSource, Surface surfaceSource, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), trimSource, surfaceSource, tolerance);
        }
        /// <summary>
        /// Makes a brep with one face.
        /// </summary>
        /// <param name="corner1">A first corner.</param>
        /// <param name="corner2">A second corner.</param>
        /// <param name="corner3">A third corner.</param>
        /// <param name="tolerance">
        /// Minimum edge length without collapsing to a singularity.
        /// </param>
        /// <returns>A boundary representation, or null on error.</returns>
        public static Brep CreateFromCornerPoints(Point3d corner1, Point3d corner2, Point3d corner3, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), corner1, corner2, corner3, tolerance);
        }
        /// <summary>
        /// make a Brep with one face.
        /// </summary>
        /// <param name="corner1">A first corner.</param>
        /// <param name="corner2">A second corner.</param>
        /// <param name="corner3">A third corner.</param>
        /// <param name="corner4">A fourth corner.</param>
        /// <param name="tolerance">
        /// Minimum edge length allowed before collapsing the side into a singularity.
        /// </param>
        /// <returns>A boundary representation, or null on error.</returns>
        public static Brep CreateFromCornerPoints(Point3d corner1, Point3d corner2, Point3d corner3, Point3d corner4, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), corner1, corner2, corner3, corner4, tolerance);
        }
        /// <summary>
        /// Constructs a coons patch from 2, 3, or 4 curves.
        /// </summary>
        /// <param name="curves">A list, an array or any enumerable set of curves.</param>
        /// <returns>resulting brep or null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_edgesrf.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_edgesrf.cs' lang='cs'/>
        /// <code source='examples\py\ex_edgesrf.py' lang='py'/>
        /// </example>
        public static Brep CreateEdgeSurface(IEnumerable<Curve> curves)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), curves);
        }
        /// <summary>
        /// Constructs a set of planar Breps as outlines by the loops.
        /// </summary>
        /// <param name="inputLoops">Curve loops that delineate the planar boundaries.</param>
        /// <returns>An array of Planar Breps or null on error.</returns>
        public static Brep[] CreatePlanarBreps(Rhino.Collections.CurveList inputLoops)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), inputLoops);
        }
        /// <summary>
        /// Constructs a set of planar Breps as outlines by the loops.
        /// </summary>
        /// <param name="inputLoops">Curve loops that delineate the planar boundaries.</param>
        /// <param name="tolerance"></param>
        /// <returns>An array of Planar Breps.</returns>
        public static Brep[] CreatePlanarBreps(Rhino.Collections.CurveList inputLoops, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), inputLoops, tolerance);
        }
        /// <summary>
        /// Offsets a face including trim information to create a new brep.
        /// </summary>
        /// <param name="face">the face to offset.</param>
        /// <param name="offsetDistance">An offset distance.</param>
        /// <param name="offsetTolerance">
        ///  Use 0.0 to make a loose offset. Otherwise, the document's absolute tolerance is usually sufficient.
        /// </param>
        /// <param name="bothSides">When true, offset to both sides of the input face.</param>
        /// <param name="createSolid">When true, make a solid object.</param>
        /// <returns>
        /// A new brep if successful. The brep can be disjoint if bothSides is true and createSolid is false,
        /// or if createSolid is true and connecting the offsets with side surfaces fails.
        /// null if unsuccessful.
        /// </returns>
        public static Brep CreateFromOffsetFace(BrepFace face, double offsetDistance, double offsetTolerance, bool bothSides, bool createSolid)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), face, offsetDistance, offsetTolerance, bothSides, createSolid);
        }
        /// <summary>
        /// Constructs closed polysurfaces from surfaces and polysurfaces that bound a region in space.
        /// </summary>
        /// <param name="breps">
        /// The intersecting surfaces and polysurfaces to automatically trim and join into closed polysurfaces.
        /// </param>
        /// <param name="tolerance">
        /// The trim and join tolerance. If set to RhinoMath.UnsetValue, Rhino's global absolute tolerance is used.
        /// </param>
        /// <returns>The resulting polysurfaces on success or null on failure.</returns>
        public static Brep[] CreateSolid(IEnumerable<Brep> breps, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), breps, tolerance);
        }
        /// <summary>
        /// Merges two surfaces into one surface at untrimmed edges.
        /// </summary>
        /// <param name="surface0">The first surface to merge.</param>
        /// <param name="surface1">The second surface to merge.</param>
        /// <param name="tolerance">Surface edges must be within this tolerance for the two surfaces to merge.</param>
        /// <param name="angleToleranceRadians">Edge must be within this angle tolerance in order for contiguous edges to be combined into a single edge.</param>
        /// <returns>The merged surfaces as a Brep if successful, null if not successful.</returns>
        public static Brep MergeSurfaces(Surface surface0, Surface surface1, double tolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), surface0, surface1, tolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Merges two surfaces into one surface at untrimmed edges. Both surfaces must be untrimmed and share an edge.
        /// </summary>
        /// <param name="brep0">The first single-face Brep to merge.</param>
        /// <param name="brep1">The second single-face Brep to merge.</param>
        /// <param name="tolerance">Surface edges must be within this tolerance for the two surfaces to merge.</param>
        /// <param name="angleToleranceRadians">Edge must be within this angle tolerance in order for contiguous edges to be combined into a single edge.</param>
        /// <returns>The merged Brep if successful, null if not successful.</returns>
        public static Brep MergeSurfaces(Brep brep0, Brep brep1, double tolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep0, brep1, tolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Merges two surfaces into one surface at untrimmed edges. Both surfaces must be untrimmed and share an edge.
        /// </summary>
        /// <param name="brep0">The first single-face Brep to merge.</param>
        /// <param name="brep1">The second single-face Brep to merge.</param>
        /// <param name="tolerance">Surface edges must be within this tolerance for the two surfaces to merge.</param>
        /// <param name="angleToleranceRadians">Edge must be within this angle tolerance in order for contiguous edges to be combined into a single edge.</param>
        /// <param name="point0">2D pick point on the first single-face Brep. The value can be unset.</param>
        /// <param name="point1">2D pick point on the second single-face Brep. The value can be unset.</param>
        /// <param name="roundness">Defines the roundness of the merge. Acceptable values are between 0.0 (sharp) and 1.0 (smooth).</param>
        /// <param name="smooth">The surface will be smooth. This makes the surface behave better for control point editing, but may alter the shape of both surfaces.</param>
        /// <returns>The merged Brep if successful, null if not successful.</returns>
        public static Brep MergeSurfaces(Brep brep0, Brep brep1, double tolerance, double angleToleranceRadians, Point2d point0, Point2d point1, double roundness, bool smooth)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep0, brep1, tolerance, angleToleranceRadians, point0, point1, roundness, smooth);
        }
        /// <summary>
        /// Constructs a brep patch.
        /// <para>This is the simple version of fit that uses a specified starting surface.</para>
        /// </summary>
        /// <param name="geometry">
        /// Combination of Curves, BrepTrims, Points, PointClouds or Meshes.
        /// Curves and trims are sampled to get points. Trims are sampled for
        /// points and normals.
        /// </param>
        /// <param name="startingSurface">A starting surface (can be null).</param>
        /// <param name="tolerance">
        /// Tolerance used by input analysis functions for loop finding, trimming, etc.
        /// </param>
        /// <returns>
        /// Brep fit through input on success, or null on error.
        /// </returns>
        public static Brep CreatePatch(IEnumerable<GeometryBase> geometry, Surface startingSurface, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), geometry, startingSurface, tolerance);
        }
        /// <summary>
        /// Constructs a brep patch.
        /// <para>This is the simple version of fit that uses a plane with u x v spans.
        /// It makes a plane by fitting to the points from the input geometry to use as the starting surface.
        /// The surface has the specified u and v span count.</para>
        /// </summary>
        /// <param name="geometry">
        /// A combination of <see cref="Curve">curves</see>, brep trims,
        /// <see cref="Point">points</see>, <see cref="PointCloud">point clouds</see> or <see cref="Mesh">meshes</see>.
        /// Curves and trims are sampled to get points. Trims are sampled for
        /// points and normals.
        /// </param>
        /// <param name="uSpans">The number of spans in the U direction.</param>
        /// <param name="vSpans">The number of spans in the V direction.</param>
        /// <param name="tolerance">
        /// Tolerance used by input analysis functions for loop finding, trimming, etc.
        /// </param>
        /// <returns>
        /// A brep fit through input on success, or null on error.
        /// </returns>
        public static Brep CreatePatch(IEnumerable<GeometryBase> geometry, int uSpans, int vSpans, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), geometry, uSpans, vSpans, tolerance);
        }
        /// <summary>
        /// Constructs a brep patch using all controls
        /// </summary>
        /// <param name="geometry">
        /// A combination of <see cref="Curve">curves</see>, brep trims,
        /// <see cref="Point">points</see>, <see cref="PointCloud">point clouds</see> or <see cref="Mesh">meshes</see>.
        /// Curves and trims are sampled to get points. Trims are sampled for
        /// points and normals.
        /// </param>
        /// <param name="startingSurface">A starting surface (can be null).</param>
        /// <param name="uSpans">
        /// Number of surface spans used when a plane is fit through points to start in the U direction.
        /// </param>
        /// <param name="vSpans">
        /// Number of surface spans used when a plane is fit through points to start in the U direction.
        /// </param>
        /// <param name="trim">
        /// If true, try to find an outer loop from among the input curves and trim the result to that loop
        /// </param>
        /// <param name="tangency">
        /// If true, try to find brep trims in the outer loop of curves and try to
        /// fit to the normal direction of the trim's surface at those locations.
        /// </param>
        /// <param name="pointSpacing">
        /// Basic distance between points sampled from input curves.
        /// </param>
        /// <param name="flexibility">
        /// Determines the behavior of the surface in areas where its not otherwise
        /// controlled by the input.  Lower numbers make the surface behave more
        /// like a stiff material; higher, less like a stiff material.  That is,
        /// each span is made to more closely match the spans adjacent to it if there
        /// is no input geometry mapping to that area of the surface when the
        /// flexibility value is low.  The scale is logrithmic. Numbers around 0.001
        /// or 0.1 make the patch pretty stiff and numbers around 10 or 100 make the
        /// surface flexible.
        /// </param>
        /// <param name="surfacePull">
        /// Tends to keep the result surface where it was before the fit in areas where
        /// there is on influence from the input geometry
        /// </param>
        /// <param name="fixEdges">
        /// Array of four elements. Flags to keep the edges of a starting (untrimmed)
        /// surface in place while fitting the interior of the surface.  Order of
        /// flags is left, bottom, right, top
        ///</param>
        /// <param name="tolerance">
        /// Tolerance used by input analysis functions for loop finding, trimming, etc.
        /// </param>
        /// <returns>
        /// A brep fit through input on success, or null on error.
        /// </returns>
        public static Brep CreatePatch(IEnumerable<GeometryBase> geometry, Surface startingSurface, int uSpans, int vSpans, bool trim, bool tangency, double pointSpacing, double flexibility, double surfacePull, bool[] fixEdges, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), geometry, startingSurface, uSpans, vSpans, trim, tangency, pointSpacing, flexibility, surfacePull, fixEdges, tolerance);
        }
        /// <summary>
        /// Creates a single walled pipe
        /// </summary>
        /// <param name="rail">the path curve for the pipe</param>
        /// <param name="radius">radius of the pipe</param>
        /// <param name="localBlending">
        /// If True, Local (pipe radius stays constant at the ends and changes more rapidly in the middle) is applied.
        /// If False, Global (radius is linearly blended from one end to the other, creating pipes that taper from one radius to the other) is applied
        /// </param>
        /// <param name="cap">end cap mode</param>
        /// <param name="fitRail">
        /// If the curve is a polycurve of lines and arcs, the curve is fit and a single surface is created;
        /// otherwise the result is a polysurface with joined surfaces created from the polycurve segments.
        /// </param>
        /// <param name="absoluteTolerance">
        /// The sweeping and fitting tolerance. If you are unsure what to use, then use the document's absolute tolerance
        /// </param>
        /// <param name="angleToleranceRadians">
        /// The angle tolerance. If you are unsure what to use, then either use the document's angle tolerance in radians
        /// </param>
        /// <returns>Array of created pipes on success</returns>
        public static Brep[] CreatePipe(Curve rail, double radius, bool localBlending, PipeCapMode cap, bool fitRail, double absoluteTolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail, radius, localBlending, cap, fitRail, absoluteTolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Creates a single walled pipe
        /// </summary>
        /// <param name="rail">the path curve for the pipe</param>
        /// <param name="railRadiiParameters">
        /// one or more normalized curve parameters where changes in radius occur.
        /// Important: curve parameters must be normalized - ranging between 0.0 and 1.0.
        /// </param>
        /// <param name="radii">An array of radii - one at each normalized curve parameter in railRadiiParameters.</param>
        /// <param name="localBlending">
        /// If True, Local (pipe radius stays constant at the ends and changes more rapidly in the middle) is applied.
        /// If False, Global (radius is linearly blended from one end to the other, creating pipes that taper from one radius to the other) is applied
        /// </param>
        /// <param name="cap">end cap mode</param>
        /// <param name="fitRail">
        /// If the curve is a polycurve of lines and arcs, the curve is fit and a single surface is created;
        /// otherwise the result is a polysurface with joined surfaces created from the polycurve segments.
        /// </param>
        /// <param name="absoluteTolerance">
        /// The sweeping and fitting tolerance. If you are unsure what to use, then use the document's absolute tolerance
        /// </param>
        /// <param name="angleToleranceRadians">
        /// The angle tolerance. If you are unsure what to use, then either use the document's angle tolerance in radians
        /// </param>
        /// <returns>Array of created pipes on success</returns>
        public static Brep[] CreatePipe(Curve rail, IEnumerable<double> railRadiiParameters, IEnumerable<double> radii, bool localBlending, PipeCapMode cap, bool fitRail, double absoluteTolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail, railRadiiParameters, radii, localBlending, cap, fitRail, absoluteTolerance, angleToleranceRadians);
        }
        /// <summary>
        /// General 1 rail sweep. If you are not producing the sweep results that you are after, then
        /// use the SweepOneRail class with options to generate the swept geometry
        /// </summary>
        /// <param name="rail">Rail to sweep shapes along</param>
        /// <param name="shape">Shape curve</param>
        /// <param name="closed">Only matters if shape is closed</param>
        /// <param name="tolerance">Tolerance for fitting surface and rails</param>
        /// <returns>Array of Brep sweep results</returns>
        public static Brep[] CreateFromSweep(Curve rail, Curve shape, bool closed, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail, shape, closed, tolerance);
        }
        /// <summary>
        /// General 1 rail sweep. If you are not producing the sweep results that you are after, then
        /// use the SweepOneRail class with options to generate the swept geometry
        /// </summary>
        /// <param name="rail">Rail to sweep shapes along</param>
        /// <param name="shapes">Shape curves</param>
        /// <param name="closed">Only matters if shapes are closed</param>
        /// <param name="tolerance">Tolerance for fitting surface and rails</param>
        /// <returns>Array of Brep sweep results</returns>
        public static Brep[] CreateFromSweep(Curve rail, IEnumerable<Curve> shapes, bool closed, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail, shapes, closed, tolerance);
        }
        /// <summary>
        /// General 2 rail sweep. If you are not producing the sweep results that you are after, then
        /// use the SweepTwoRail class with options to generate the swept geometry
        /// </summary>
        /// <param name="rail1">Rail to sweep shapes along</param>
        /// <param name="rail2">Rail to sweep shapes along</param>
        /// <param name="shape">Shape curve</param>
        /// <param name="closed">Only matters if shape is closed</param>
        /// <param name="tolerance">Tolerance for fitting surface and rails</param>
        /// <returns>Array of Brep sweep results</returns>
        public static Brep[] CreateFromSweep(Curve rail1, Curve rail2, Curve shape, bool closed, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail1, rail2, shape, closed, tolerance);
        }
        /// <summary>
        /// General 2 rail sweep. If you are not producing the sweep results that you are after, then
        /// use the SweepTwoRail class with options to generate the swept geometry
        /// </summary>
        /// <param name="rail1">Rail to sweep shapes along</param>
        /// <param name="rail2">Rail to sweep shapes along</param>
        /// <param name="shapes">Shape curves</param>
        /// <param name="closed">Only matters if shapes are closed</param>
        /// <param name="tolerance">Tolerance for fitting surface and rails</param>
        /// <returns>Array of Brep sweep results</returns>
        public static Brep[] CreateFromSweep(Curve rail1, Curve rail2, IEnumerable<Curve> shapes, bool closed, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail1, rail2, shapes, closed, tolerance);
        }
        /// <summary>
        /// Makes a 2 rail sweep.  Like CreateFromSweep but the result is split where parameterization along a rail changes abruptly
        /// </summary>
        /// <param name="rail1">Rail to sweep shapes along</param>
        /// <param name="rail2">Rail to sweep shapes along</param>
        /// <param name="shapes">Shape curves</param>
        /// <param name="rail_params">Shape parameters</param>
        /// <param name="closed">Only matters if shapes are closed</param>
        /// <param name="tolerance">Tolerance for fitting surface and rails</param>
        /// <returns>Array of Brep sweep results</returns>
        public static Brep[] CreateFromSweepInParts(Curve rail1, Curve rail2, IEnumerable<Curve> shapes, IEnumerable<Point2d> rail_params, bool closed, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), rail1, rail2, shapes, rail_params, closed, tolerance);
        }
        /// <summary>
        /// Extrude a curve to a taper making a brep (potentially more than 1)
        /// </summary>
        /// <param name="curveToExtrude">the curve to extrude</param>
        /// <param name="distance">the distance to extrude</param>
        /// <param name="direction">the direction of the extrusion</param>
        /// <param name="basePoint">the basepoint of the extrusion</param>
        /// <param name="draftAngleRadians">angle of the extrusion</param>
        /// <param name="cornerType"></param>
        /// <param name="tolerance">tolerance to use for the extrusion</param>
        /// <param name="angleToleranceRadians">angle tolerance to use for the extrusion</param>
        /// <returns>array of breps on success</returns>
        public static Brep[] CreateFromTaperedExtrude(Curve curveToExtrude, double distance, Vector3d direction, Point3d basePoint, double draftAngleRadians, ExtrudeCornerType cornerType, double tolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), curveToExtrude, distance, direction, basePoint, draftAngleRadians, cornerType, tolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Extrude a curve to a taper making a brep (potentially more than 1)
        /// </summary>
        /// <param name="curveToExtrude">the curve to extrude</param>
        /// <param name="distance">the distance to extrude</param>
        /// <param name="direction">the direction of the extrusion</param>
        /// <param name="basePoint">the basepoint of the extrusion</param>
        /// <param name="draftAngleRadians">angle of the extrusion</param>
        /// <param name="cornerType"></param>
        /// <returns>array of breps on success</returns>
        /// <remarks>tolerances used are based on the active doc tolerance</remarks>
        public static Brep[] CreateFromTaperedExtrude(Curve curveToExtrude, double distance, Vector3d direction, Point3d basePoint, double draftAngleRadians, ExtrudeCornerType cornerType)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), curveToExtrude, distance, direction, basePoint, draftAngleRadians, cornerType);
        }
        /// <summary>
        /// Makes a surface blend between two surface edges.
        /// </summary>
        /// <param name="face0">First face to blend from.</param>
        /// <param name="edge0">First edge to blend from.</param>
        /// <param name="domain0">The domain of edge0 to use.</param>
        /// <param name="rev0">If false, edge0 will be used in its natural direction. If true, edge0 will be used in the reversed direction.</param>
        /// <param name="continuity0">Continuity for the blend at the start.</param>
        /// <param name="face1">Second face to blend from.</param>
        /// <param name="edge1">Second edge to blend from.</param>
        /// <param name="domain1">The domain of edge1 to use.</param>
        /// <param name="rev1">If false, edge1 will be used in its natural direction. If true, edge1 will be used in the reversed direction.</param>
        /// <param name="continuity1">Continuity for the blend at the end.</param>
        /// <returns>Array of Breps if successful.</returns>
        public static Brep[] CreateBlendSurface(BrepFace face0, BrepEdge edge0, Interval domain0, bool rev0, BlendContinuity continuity0, BrepFace face1, BrepEdge edge1, Interval domain1, bool rev1, BlendContinuity continuity1)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), face0, edge0, domain0, rev0, continuity0, face1, edge1, domain1, rev1, continuity1);
        }
        /// <summary>
        /// Makes a curve blend between points on two surface edges. The blend will be tangent to the surfaces and perpendicular to the edges.
        /// </summary>
        /// <param name="face0">First face to blend from.</param>
        /// <param name="edge0">First edge to blend from.</param>
        /// <param name="t0">Location on first edge for first end of blend curve.</param>
        /// <param name="rev0">If false, edge0 will be used in its natural direction. If true, edge0 will be used in the reversed direction.</param>
        /// <param name="continuity0">Continuity for the blend at the start.</param>
        /// <param name="face1">Second face to blend from.</param>
        /// <param name="edge1">Second edge to blend from.</param>
        /// <param name="t1">Location on second edge for second end of blend curve.</param>
        /// <param name="rev1">If false, edge1 will be used in its natural direction. If true, edge1 will be used in the reversed direction.</param>
        /// <param name="continuity1">>Continuity for the blend at the end.</param>
        /// <returns>The blend curve on success. null on failure</returns>
        public static Curve CreateBlendShape(BrepFace face0, BrepEdge edge0, double t0, bool rev0, BlendContinuity continuity0, BrepFace face1, BrepEdge edge1, double t1, bool rev1, BlendContinuity continuity1)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), face0, edge0, t0, rev0, continuity0, face1, edge1, t1, rev1, continuity1);
        }
        /// <summary>
        /// Creates a constant-radius round surface between two surfaces.
        /// </summary>
        /// <param name="face0">First face to fillet from.</param>
        /// <param name="uv0">A parameter face0 at the side you want to keep after filleting.</param>
        /// <param name="face1">Second face to fillet from.</param>
        /// <param name="uv1">A parameter face1 at the side you want to keep after filleting.</param>
        /// <param name="radius">The fillet radius.</param>
        /// <param name="extend">If true, then when one input surface is longer than the other, the fillet surface is extended to the input surface edges.</param>
        /// <param name="tolerance">The tolerance. In in doubt, the the document's model absolute tolerance.</param>
        /// <returns>Array of Breps if successful.</returns>
        public static Brep[] CreateFilletSurface(BrepFace face0, Point2d uv0, BrepFace face1, Point2d uv1, double radius, bool extend, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), face0, uv0, face1, uv1, radius, extend, tolerance);
        }
        /// <summary>
        /// Creates a ruled surface as a bevel between two input surface edges.
        /// </summary>
        /// <param name="face0">First face to chamfer from.</param>
        /// <param name="uv0">A parameter face0 at the side you want to keep after chamfering.</param>
        /// <param name="radius0">The distance from the intersection of face0 to the edge of the chamfer.</param>
        /// <param name="face1">Second face to chamfer from.</param>
        /// <param name="uv1">A parameter face1 at the side you want to keep after chamfering.</param>
        /// <param name="radius1">The distance from the intersection of face1 to the edge of the chamfer.</param>
        /// <param name="extend">If true, then when one input surface is longer than the other, the chamfer surface is extended to the input surface edges.</param>
        /// <param name="tolerance">The tolerance. In in doubt, the the document's model absolute tolerance.</param>
        /// <returns>Array of Breps if successful.</returns>
        public static Brep[] CreateChamferSurface(BrepFace face0, Point2d uv0, double radius0, BrepFace face1, Point2d uv1, double radius1, bool extend, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), face0, uv0, radius0, face1, uv1, radius1, extend, tolerance);
        }
        /// <summary>
        /// Fillets, chamfers, or blends the edges of a brep.
        /// </summary>
        /// <param name="brep">The brep to fillet, chamfer, or blend edges.</param>
        /// <param name="edgeIndices">An array of one or more edge indices where the fillet, chamfer, or blend will occur.</param>
        /// <param name="startRadii">An array of starting fillet, chamfer, or blend radaii, one for each edge index.</param>
        /// <param name="endRadii">An array of ending fillet, chamfer, or blend radaii, one for each edge index.</param>
        /// <param name="blendType">The blend type.</param>
        /// <param name="railType">The rail type.</param>
        /// <param name="tolerance">The tolerance to be used to perform calculations.</param>
        /// <returns>Array of Breps if successful.</returns>
        public static Brep[] CreateFilletEdges(Brep brep, IEnumerable<int> edgeIndices, IEnumerable<double> startRadii, IEnumerable<double> endRadii, BlendType blendType, RailType railType, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brep, edgeIndices, startRadii, endRadii, blendType, railType, tolerance);
        }
        /// <summary>
        /// Offsets a Brep.
        /// </summary>
        /// <param name="brep">The Brep to offset.</param>
        /// <param name="distance">
        /// The distance to offset. This is a signed distance value with respect to
        /// face normals and flipped faces.
        /// </param>
        /// <param name="solid">
        /// If true, then the function makes a closed solid from the input and offset
        /// surfaces by lofting a ruled surface between all of the matching edges.
        /// </param>
        /// <param name="extend">
        /// If true, then the function maintains the sharp corners when the original
        /// surfaces have sharps corner. If False, then the function creates fillets
        /// at sharp corners in the original surfaces.
        /// </param>
        /// <param name="tolerance">The offset tolerance.</param>
        /// <param name="outBlends">The results of the calculation.</param>
        /// <param name="outWalls">The results of the calculation.</param>
        /// <returns>
        /// Array of Breps if successful. If the function succeeds in offsetting, a
        /// single Brep will be returned. Otherwise, the array will contain the 
        /// offset surfaces, outBlends will contain the set of blends used to fill
        /// in gaps (if extend is false), and outWalls will contain the set of wall
        /// surfaces that was supposed to join the offset to the original (if solid
        /// is true).
        /// </returns>
        public static Brep[] CreateOffsetBrep(Brep brep, double distance, bool solid, bool extend, double tolerance, out Brep[] outBlends, out Brep[] outWalls)
        {
            return ComputeServer.Post<Brep[], Brep[], Brep[]>(ApiAddress(), out outBlends, out outWalls, brep, distance, solid, extend, tolerance);
        }
        /// <summary>
        /// Joins two naked edges, or edges that are coincident or close together, from two Breps.
        /// </summary>
        /// <param name="brep0">The first Brep.</param>
        /// <param name="edgeIndex0">The edge index on the first Brep.</param>
        /// <param name="brep1">The second Brep.</param>
        /// <param name="edgeIndex1">The edge index on the second Brep.</param>
        /// <param name="joinTolerance">The join tolerance.</param>
        /// <returns>The resulting Brep if successful, null on failure.</returns>
        public static Brep CreateFromJoinedEdges(Brep brep0, int edgeIndex0, Brep brep1, int edgeIndex1, double joinTolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep0, edgeIndex0, brep1, edgeIndex1, joinTolerance);
        }
        /// <summary>
        /// Constructs one or more Breps by lofting through a set of curves.
        /// </summary>
        /// <param name="curves">
        /// The curves to loft through. This function will not perform any curve sorting. You must pass in
        /// curves in the order you want them lofted. This function will not adjust the directions of open
        /// curves. Use Curve.DoDirectionsMatch and Curve.Reverse to adjust the directions of open curves.
        /// This function will not adjust the seams of closed curves. Use Curve.ChangeClosedCurveSeam to
        /// adjust the seam of closed curves.
        /// </param>
        /// <param name="start">
        /// Optional starting point of loft. Use Point3d.Unset if you do not want to include a start point.
        /// </param>
        /// <param name="end">
        /// Optional ending point of loft. Use Point3d.Unset if you do not want to include an end point.
        /// </param>
        /// <param name="loftType">type of loft to perform.</param>
        /// <param name="closed">true if the last curve in this loft should be connected back to the first one.</param>
        /// <returns>
        /// Constructs a closed surface, continuing the surface past the last curve around to the
        /// first curve. Available when you have selected three shape curves.
        /// </returns>
        /// <example>
        /// <code source='examples\vbnet\ex_loft.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_loft.cs' lang='cs'/>
        /// <code source='examples\py\ex_loft.py' lang='py'/>
        /// </example>
        public static Brep[] CreateFromLoft(IEnumerable<Curve> curves, Point3d start, Point3d end, LoftType loftType, bool closed)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), curves, start, end, loftType, closed);
        }
        /// <summary>
        /// Constructs one or more Breps by lofting through a set of curves. Input for the loft is simplified by
        /// rebuilding to a specified number of control points.
        /// </summary>
        /// <param name="curves">
        /// The curves to loft through. This function will not perform any curve sorting. You must pass in
        /// curves in the order you want them lofted. This function will not adjust the directions of open
        /// curves. Use Curve.DoDirectionsMatch and Curve.Reverse to adjust the directions of open curves.
        /// This function will not adjust the seams of closed curves. Use Curve.ChangeClosedCurveSeam to
        /// adjust the seam of closed curves.
        /// </param>
        /// <param name="start">
        /// Optional starting point of loft. Use Point3d.Unset if you do not want to include a start point.
        /// </param>
        /// <param name="end">
        /// Optional ending point of lost. Use Point3d.Unset if you do not want to include an end point.
        /// </param>
        /// <param name="loftType">type of loft to perform.</param>
        /// <param name="closed">true if the last curve in this loft should be connected back to the first one.</param>
        /// <param name="rebuildPointCount">A number of points to use while rebuilding the curves. 0 leaves turns this parameter off.</param>
        /// <returns>
        /// Constructs a closed surface, continuing the surface past the last curve around to the
        /// first curve. Available when you have selected three shape curves.
        /// </returns>
        public static Brep[] CreateFromLoftRebuild(IEnumerable<Curve> curves, Point3d start, Point3d end, LoftType loftType, bool closed, int rebuildPointCount)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), curves, start, end, loftType, closed, rebuildPointCount);
        }
        /// <summary>
        /// Constructs one or more Breps by lofting through a set of curves. Input for the loft is simplified by
        /// refitting to a specified tolerance.
        /// </summary>
        /// <param name="curves">
        /// The curves to loft through. This function will not perform any curve sorting. You must pass in
        /// curves in the order you want them lofted. This function will not adjust the directions of open
        /// curves. Use Curve.DoDirectionsMatch and Curve.Reverse to adjust the directions of open curves.
        /// This function will not adjust the seams of closed curves. Use Curve.ChangeClosedCurveSeam to
        /// adjust the seam of closed curves.
        /// </param>
        /// <param name="start">
        /// Optional starting point of loft. Use Point3d.Unset if you do not want to include a start point.
        /// </param>
        /// <param name="end">
        /// Optional ending point of lost. Use Point3d.Unset if you do not want to include an end point.
        /// </param>
        /// <param name="loftType">type of loft to perform.</param>
        /// <param name="closed">true if the last curve in this loft should be connected back to the first one.</param>
        /// <param name="refitTolerance">A distance to use in refitting, or 0 if you want to turn this parameter off.</param>
        /// <returns>
        /// Constructs a closed surface, continuing the surface past the last curve around to the
        /// first curve. Available when you have selected three shape curves.
        /// </returns>
        public static Brep[] CreateFromLoftRefit(IEnumerable<Curve> curves, Point3d start, Point3d end, LoftType loftType, bool closed, double refitTolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), curves, start, end, loftType, closed, refitTolerance);
        }
        /// <summary>
        /// Compute the Boolean Union of a set of Breps.
        /// </summary>
        /// <param name="breps">Breps to union.</param>
        /// <param name="tolerance">Tolerance to use for union operation.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        public static Brep[] CreateBooleanUnion(IEnumerable<Brep> breps, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), breps, tolerance);
        }
        /// <summary>
        /// Compute the Boolean Union of a set of Breps.
        /// </summary>
        /// <param name="breps">Breps to union.</param>
        /// <param name="tolerance">Tolerance to use for union operation.</param>
        /// <param name="manifoldOnly">If true, non-manifold input breps are ignored.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        public static Brep[] CreateBooleanUnion(IEnumerable<Brep> breps, double tolerance, bool manifoldOnly)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), breps, tolerance, manifoldOnly);
        }
        /// <summary>
        /// Compute the Solid Intersection of two sets of Breps.
        /// </summary>
        /// <param name="firstSet">First set of Breps.</param>
        /// <param name="secondSet">Second set of Breps.</param>
        /// <param name="tolerance">Tolerance to use for intersection operation.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        public static Brep[] CreateBooleanIntersection(IEnumerable<Brep> firstSet, IEnumerable<Brep> secondSet, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstSet, secondSet, tolerance);
        }
        /// <summary>
        /// Compute the Solid Intersection of two sets of Breps.
        /// </summary>
        /// <param name="firstSet">First set of Breps.</param>
        /// <param name="secondSet">Second set of Breps.</param>
        /// <param name="tolerance">Tolerance to use for intersection operation.</param>
        /// <param name="manifoldOnly">If true, non-manifold input breps are ignored.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        public static Brep[] CreateBooleanIntersection(IEnumerable<Brep> firstSet, IEnumerable<Brep> secondSet, double tolerance, bool manifoldOnly)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstSet, secondSet, tolerance, manifoldOnly);
        }
        /// <summary>
        /// Compute the Solid Intersection of two Breps.
        /// </summary>
        /// <param name="firstBrep">First Brep for boolean intersection.</param>
        /// <param name="secondBrep">Second Brep for boolean intersection.</param>
        /// <param name="tolerance">Tolerance to use for intersection operation.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        public static Brep[] CreateBooleanIntersection(Brep firstBrep, Brep secondBrep, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstBrep, secondBrep, tolerance);
        }
        /// <summary>
        /// Compute the Solid Intersection of two Breps.
        /// </summary>
        /// <param name="firstBrep">First Brep for boolean intersection.</param>
        /// <param name="secondBrep">Second Brep for boolean intersection.</param>
        /// <param name="tolerance">Tolerance to use for intersection operation.</param>
        /// <param name="manifoldOnly">If true, non-manifold input breps are ignored.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        public static Brep[] CreateBooleanIntersection(Brep firstBrep, Brep secondBrep, double tolerance, bool manifoldOnly)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstBrep, secondBrep, tolerance, manifoldOnly);
        }
        /// <summary>
        /// Compute the Solid Difference of two sets of Breps.
        /// </summary>
        /// <param name="firstSet">First set of Breps (the set to subtract from).</param>
        /// <param name="secondSet">Second set of Breps (the set to subtract).</param>
        /// <param name="tolerance">Tolerance to use for difference operation.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        /// <example>
        /// <code source='examples\vbnet\ex_booleandifference.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_booleandifference.cs' lang='cs'/>
        /// <code source='examples\py\ex_booleandifference.py' lang='py'/>
        /// </example>
        public static Brep[] CreateBooleanDifference(IEnumerable<Brep> firstSet, IEnumerable<Brep> secondSet, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstSet, secondSet, tolerance);
        }
        /// <summary>
        /// Compute the Solid Difference of two sets of Breps.
        /// </summary>
        /// <param name="firstSet">First set of Breps (the set to subtract from).</param>
        /// <param name="secondSet">Second set of Breps (the set to subtract).</param>
        /// <param name="tolerance">Tolerance to use for difference operation.</param>
        /// <param name="manifoldOnly">If true, non-manifold input breps are ignored.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        /// <example>
        /// <code source='examples\vbnet\ex_booleandifference.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_booleandifference.cs' lang='cs'/>
        /// <code source='examples\py\ex_booleandifference.py' lang='py'/>
        /// </example>
        public static Brep[] CreateBooleanDifference(IEnumerable<Brep> firstSet, IEnumerable<Brep> secondSet, double tolerance, bool manifoldOnly)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstSet, secondSet, tolerance, manifoldOnly);
        }
        /// <summary>
        /// Compute the Solid Difference of two Breps.
        /// </summary>
        /// <param name="firstBrep">First Brep for boolean difference.</param>
        /// <param name="secondBrep">Second Brep for boolean difference.</param>
        /// <param name="tolerance">Tolerance to use for difference operation.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        public static Brep[] CreateBooleanDifference(Brep firstBrep, Brep secondBrep, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstBrep, secondBrep, tolerance);
        }
        /// <summary>
        /// Compute the Solid Difference of two Breps.
        /// </summary>
        /// <param name="firstBrep">First Brep for boolean difference.</param>
        /// <param name="secondBrep">Second Brep for boolean difference.</param>
        /// <param name="tolerance">Tolerance to use for difference operation.</param>
        /// <param name="manifoldOnly">If true, non-manifold input breps are ignored.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        /// <remarks>The solid orientation of the breps make a difference when calling this function</remarks>
        public static Brep[] CreateBooleanDifference(Brep firstBrep, Brep secondBrep, double tolerance, bool manifoldOnly)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), firstBrep, secondBrep, tolerance, manifoldOnly);
        }
        /// <summary>
        /// Creates a hollowed out shell from a solid Brep. Function only operates on simple, solid, manifold Breps.
        /// </summary>
        /// <param name="brep">The solid Brep to shell.</param>
        /// <param name="facesToRemove">The indices of the Brep faces to remove. These surfaces are removed and the remainder is offset inward, using the outer parts of the removed surfaces to join the inner and outer parts.</param>
        /// <param name="distance">The distance, or thickness, for the shell. This is a signed distance value with respect to face normals and flipped faces.</param>
        /// <param name="tolerance">The offset tolerane. When in doubt, use the document's absolute tolerance.</param>
        /// <returns>An array of Brep results or null on failure.</returns>
        public static Brep[] CreateShell(Brep brep, IEnumerable<int> facesToRemove, double distance, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brep, facesToRemove, distance, tolerance);
        }
        /// <summary>
        /// Joins the breps in the input array at any overlapping edges to form
        /// as few as possible resulting breps. There may be more than one brep in the result array.
        /// </summary>
        /// <param name="brepsToJoin">A list, an array or any enumerable set of breps to join.</param>
        /// <param name="tolerance">3d distance tolerance for detecting overlapping edges.</param>
        /// <returns>new joined breps on success, null on failure.</returns>
        public static Brep[] JoinBreps(IEnumerable<Brep> brepsToJoin, double tolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brepsToJoin, tolerance);
        }
        /// <summary>
        /// Combines two or more breps into one. A merge is like a boolean union that keeps the inside pieces. This
        /// function creates non-manifold Breps which in general are unusual in Rhino. You may want to consider using
        /// JoinBreps or CreateBooleanUnion functions instead.
        /// </summary>
        /// <param name="brepsToMerge">must contain more than one Brep.</param>
        /// <param name="tolerance">the tolerance to use when merging.</param>
        /// <returns>Single merged Brep on success. Null on error.</returns>
        /// <seealso cref="JoinBreps"/>
        public static Brep MergeBreps(IEnumerable<Brep> brepsToMerge, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brepsToMerge, tolerance);
        }
        /// <summary>
        /// Constructs the contour curves for a brep at a specified interval.
        /// </summary>
        /// <param name="brepToContour">A brep or polysurface.</param>
        /// <param name="contourStart">A point to start.</param>
        /// <param name="contourEnd">A point to use as the end.</param>
        /// <param name="interval">The interaxial offset in world units.</param>
        /// <returns>An array with intersected curves. This array can be empty.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_makerhinocontours.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_makerhinocontours.cs' lang='cs'/>
        /// <code source='examples\py\ex_makerhinocontours.py' lang='py'/>
        /// </example>
        public static Curve[] CreateContourCurves(Brep brepToContour, Point3d contourStart, Point3d contourEnd, double interval)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), brepToContour, contourStart, contourEnd, interval);
        }
        /// <summary>
        /// Constructs the contour curves for a brep, using a slicing plane.
        /// </summary>
        /// <param name="brepToContour">A brep or polysurface.</param>
        /// <param name="sectionPlane">A plane.</param>
        /// <returns>An array with intersected curves. This array can be empty.</returns>
        public static Curve[] CreateContourCurves(Brep brepToContour, Plane sectionPlane)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), brepToContour, sectionPlane);
        }
        /// <summary>
        /// Constructs all the Wireframe curves for this Brep.
        /// </summary>
        /// <param name="density">Wireframe density. Valid values range between -1 and 99.</param>
        /// <returns>An array of Wireframe curves or null on failure.</returns>
        public static Curve[] GetWireframe(this Brep brep, int density)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), brep, density);
        }
        /// <summary>
        /// Finds a point on the brep that is closest to testPoint.
        /// </summary>
        /// <param name="testPoint">Base point to project to brep.</param>
        /// <returns>The point on the Brep closest to testPoint or Point3d.Unset if the operation failed.</returns>
        public static Point3d ClosestPoint(this Brep brep, Point3d testPoint)
        {
            return ComputeServer.Post<Point3d>(ApiAddress(), brep, testPoint);
        }
        /// <summary>
        /// Determines if point is inside Brep.  This question only makes sense when
        /// the brep is a closed manifold.  This function does not not check for
        /// closed or manifold, so result is not valid in those cases.  Intersects
        /// a line through point with brep, finds the intersection point Q closest
        /// to point, and looks at face normal at Q.  If the point Q is on an edge
        /// or the intersection is not transverse at Q, then another line is used.
        /// </summary>
        /// <param name="point">3d point to test.</param>
        /// <param name="tolerance">
        /// 3d distance tolerance used for intersection and determining strict inclusion.
        /// A good default is RhinoMath.SqrtEpsilon.
        /// </param>
        /// <param name="strictlyIn">
        /// if true, point is in if inside brep by at least tolerance.
        /// if false, point is in if truly in or within tolerance of boundary.
        /// </param>
        /// <returns>
        /// true if point is in, false if not.
        /// </returns>
        public static bool IsPointInside(this Brep brep, Point3d point, double tolerance, bool strictlyIn)
        {
            return ComputeServer.Post<bool>(ApiAddress(), brep, point, tolerance, strictlyIn);
        }
        /// <summary>
        /// Returns a new Brep that is equivalent to this Brep with all planar holes capped.
        /// </summary>
        /// <param name="tolerance">Tolerance to use for capping.</param>
        /// <returns>New brep on success. null on error.</returns>
        public static Brep CapPlanarHoles(this Brep brep, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep, tolerance);
        }
        /// <summary>
        /// If any edges of this brep overlap edges of otherBrep, merge a copy of otherBrep into this
        /// brep joining all edges that overlap within tolerance.
        /// </summary>
        /// <param name="otherBrep">Brep to be added to this brep.</param>
        /// <param name="tolerance">3d distance tolerance for detecting overlapping edges.</param>
        /// <param name="compact">if true, set brep flags and tolerances, remove unused faces and edges.</param>
        /// <returns>true if any edges were joined.</returns>
        /// <remarks>
        /// if no edges overlap, this brep is unchanged.
        /// otherBrep is copied if it is merged with this, and otherBrep is always unchanged
        /// Use this to join a list of breps in a series.
        /// When joining multiple breps in series, compact should be set to false.
        /// Call compact on the last Join.
        /// </remarks>
        public static bool Join(this Brep brep, out Brep updatedInstance, Brep otherBrep, double tolerance, bool compact)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep, otherBrep, tolerance, compact);
        }
        /// <summary>
        /// Joins naked edge pairs within the same brep that overlap within tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance value.</param>
        /// <returns>number of joins made.</returns>
        public static int JoinNakedEdges(this Brep brep, out Brep updatedInstance, double tolerance)
        {
            return ComputeServer.Post<int, Brep>(ApiAddress(), out updatedInstance, brep, tolerance);
        }
        /// <summary>
        /// Merges adjacent coplanar faces into single faces.
        /// </summary>
        /// <param name="tolerance">
        /// Tolerance for determining when edges are adjacent.
        /// When in doubt, use the document's ModelAbsoluteTolerance property.
        /// </param>
        /// <returns>true if faces were merged, false if no faces were merged.</returns>
        public static bool MergeCoplanarFaces(this Brep brep, out Brep updatedInstance, double tolerance)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep, tolerance);
        }
        /// <summary>
        /// Merges adjacent coplanar faces into single faces.
        /// </summary>
        /// <param name="tolerance">
        /// Tolerance for determining when edges are adjacent.
        /// When in doubt, use the document's ModelAbsoluteTolerance property.
        /// </param>
        /// <param name="angleTolerance">
        /// Angle tolerance, in radians, for determining when faces are parallel.
        /// When in doubt, use the document's ModelAngleToleranceRadians property.
        /// </param>
        /// <returns>true if faces were merged, false if no faces were merged.</returns>
        public static bool MergeCoplanarFaces(this Brep brep, out Brep updatedInstance, double tolerance, double angleTolerance)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep, tolerance, angleTolerance);
        }
        /// <summary>
        /// Splits a Brep into pieces.
        /// </summary>
        /// <param name="splitter">A splitting surface or polysurface.</param>
        /// <param name="intersectionTolerance">The tolerance with which to compute intersections.</param>
        /// <returns>A new array of breps. This array can be empty.</returns>
        public static Brep[] Split(this Brep brep, Brep splitter, double intersectionTolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brep, splitter, intersectionTolerance);
        }
        /// <summary>
        /// Splits a Brep into pieces.
        /// </summary>
        /// <param name="splitter">The splitting polysurface.</param>
        /// <param name="intersectionTolerance">The tolerance with which to compute intersections.</param>
        /// <param name="toleranceWasRaised">
        /// set to true if the split failed at intersectionTolerance but succeeded
        /// when the tolerance was increased to twice intersectionTolerance.
        /// </param>
        /// <returns>A new array of breps. This array can be empty.</returns>
        public static Brep[] Split(this Brep brep, Brep splitter, double intersectionTolerance, out bool toleranceWasRaised)
        {
            return ComputeServer.Post<Brep[], bool>(ApiAddress(), out toleranceWasRaised, brep, splitter, intersectionTolerance);
        }
        /// <summary>
        /// Trims a brep with an oriented cutter. The parts of the brep that lie inside
        /// (opposite the normal) of the cutter are retained while the parts to the
        /// outside (in the direction of the normal) are discarded.  If the Cutter is
        /// closed, then a connected component of the Brep that does not intersect the
        /// cutter is kept if and only if it is contained in the inside of cutter.
        /// That is the region bounded by cutter opposite from the normal of cutter,
        /// If cutter is not closed all these components are kept.
        /// </summary>
        /// <param name="cutter">A cutting brep.</param>
        /// <param name="intersectionTolerance">A tolerance value with which to compute intersections.</param>
        /// <returns>This Brep is not modified, the trim results are returned in an array.</returns>
        public static Brep[] Trim(this Brep brep, Brep cutter, double intersectionTolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brep, cutter, intersectionTolerance);
        }
        /// <summary>
        /// Trims a Brep with an oriented cutter.  The parts of Brep that lie inside
        /// (opposite the normal) of the cutter are retained while the parts to the
        /// outside ( in the direction of the normal ) are discarded. A connected
        /// component of Brep that does not intersect the cutter is kept if and only
        /// if it is contained in the inside of Cutter.  That is the region bounded by
        /// cutter opposite from the normal of cutter, or in the case of a Plane cutter
        /// the halfspace opposite from the plane normal.
        /// </summary>
        /// <param name="cutter">A cutting plane.</param>
        /// <param name="intersectionTolerance">A tolerance value with which to compute intersections.</param>
        /// <returns>This Brep is not modified, the trim results are returned in an array.</returns>
        public static Brep[] Trim(this Brep brep, Plane cutter, double intersectionTolerance)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brep, cutter, intersectionTolerance);
        }
        /// <summary>
        /// Unjoins, or separates, edges within the Brep. Note, seams in closed surfaces will not separate.
        /// </summary>
        /// <param name="edgesToUnjoin">The indices of the Brep edges to unjoin.</param>
        /// <returns>This Brep is not modified, the trim results are returned in an array.</returns>
        public static Brep[] UnjoinEdges(this Brep brep, IEnumerable<int> edgesToUnjoin)
        {
            return ComputeServer.Post<Brep[]>(ApiAddress(), brep, edgesToUnjoin);
        }
        /// <summary>
        /// Joins two naked edges, or edges that are coincident or close together.
        /// </summary>
        /// <param name="edgeIndex0">The first edge index.</param>
        /// <param name="edgeIndex1">The second edge index.</param>
        /// <param name="joinTolerance">The join tolerance.</param>
        /// <param name="compact">
        /// If joining more than one edge pair and want the edge indices of subsequent pairs to remain valid, 
        /// set to false. But then call Brep.Compact() on the final result.
        /// </param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool JoinEdges(this Brep brep, out Brep updatedInstance, int edgeIndex0, int edgeIndex1, double joinTolerance, bool compact)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep, edgeIndex0, edgeIndex1, joinTolerance, compact);
        }
        /// <summary>
        /// Transform an array of Brep components, bend neighbors to match, and leave the rest fixed.
        /// </summary>
        /// <param name="components">The Brep components to transform.</param>
        /// <param name="xform">The transformation to apply.</param>
        /// <param name="tolerance">The desired fitting tolerance to use when bending faces that share edges with both fixed and transformed components.</param>
        /// <param name="timeLimit">
        /// If the deformation is extreme, it can take a long time to calculate the result.
        /// If time_limit > 0, then the value specifies the maximum amount of time in seconds you want to spend before giving up.
        /// </param>
        /// <param name="useMultipleThreads">True if multiple threads can be used.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool TransformComponent(this Brep brep, out Brep updatedInstance, IEnumerable<ComponentIndex> components, Transform xform, double tolerance, double timeLimit, bool useMultipleThreads)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep, components, xform, tolerance, timeLimit, useMultipleThreads);
        }
        /// <summary>
        /// Compute the Area of the Brep. If you want proper Area data with moments 
        /// and error information, use the AreaMassProperties class.
        /// </summary>
        /// <returns>The area of the Brep.</returns>
        public static double GetArea(this Brep brep)
        {
            return ComputeServer.Post<double>(ApiAddress(), brep);
        }
        /// <summary>
        /// Compute the Area of the Brep. If you want proper Area data with moments 
        /// and error information, use the AreaMassProperties class.
        /// </summary>
        /// <param name="relativeTolerance">Relative tolerance to use for area calculation.</param>
        /// <param name="absoluteTolerance">Absolute tolerance to use for area calculation.</param>
        /// <returns>The area of the Brep.</returns>
        public static double GetArea(this Brep brep, double relativeTolerance, double absoluteTolerance)
        {
            return ComputeServer.Post<double>(ApiAddress(), brep, relativeTolerance, absoluteTolerance);
        }
        /// <summary>
        /// Compute the Volume of the Brep. If you want proper Volume data with moments 
        /// and error information, use the VolumeMassProperties class.
        /// </summary>
        /// <returns>The volume of the Brep.</returns>
        public static double GetVolume(this Brep brep)
        {
            return ComputeServer.Post<double>(ApiAddress(), brep);
        }
        /// <summary>
        /// Compute the Volume of the Brep. If you want proper Volume data with moments 
        /// and error information, use the VolumeMassProperties class.
        /// </summary>
        /// <param name="relativeTolerance">Relative tolerance to use for area calculation.</param>
        /// <param name="absoluteTolerance">Absolute tolerance to use for area calculation.</param>
        /// <returns>The volume of the Brep.</returns>
        public static double GetVolume(this Brep brep, double relativeTolerance, double absoluteTolerance)
        {
            return ComputeServer.Post<double>(ApiAddress(), brep, relativeTolerance, absoluteTolerance);
        }
        /// <summary>
        /// No support is available for this function.
        /// <para>Expert user function used by MakeValidForV2 to convert trim
        /// curves from one surface to its NURBS form. After calling this function,
        /// you need to change the surface of the face to a NurbsSurface.</para>
        /// </summary>
        /// <param name="face">
        /// Face whose underlying surface has a parameterization that is different
        /// from its NURBS form.
        /// </param>
        /// <param name="nurbsSurface">NURBS form of the face's underlying surface.</param>
        /// <remarks>
        /// Don't call this function unless you know exactly what you are doing.
        /// </remarks>
        public static Brep RebuildTrimsForV2(this Brep brep, BrepFace face, NurbsSurface nurbsSurface)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep, face, nurbsSurface);
        }
        /// <summary>
        /// No support is available for this function.
        /// <para>Expert user function that converts all geometry in brep to nurbs form.</para>
        /// </summary>
        /// <remarks>
        /// Don't call this function unless you know exactly what you are doing.
        /// </remarks>
        public static bool MakeValidForV2(this Brep brep, out Brep updatedInstance)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep);
        }
        /// <summary>
        /// Fills in missing or fixes incorrect component information from a Brep. 
        /// Useful when reading Brep information from other file formats that do not 
        /// provide as complete of a Brep definition as requried by Rhino.
        /// </summary>
        /// <param name="tolerance">The repair tolerance. When in doubt, use the document's model absolute tolerance.</param>
        /// <returns>True on success.</returns>
        public static bool Repair(this Brep brep, out Brep updatedInstance, double tolerance)
        {
            return ComputeServer.Post<bool, Brep>(ApiAddress(), out updatedInstance, brep, tolerance);
        }
        /// <summary>
        /// Remove all inner loops, or holes, in a Brep.
        /// </summary>
        /// <param name="tolerance">The tolerance. When in doubt, use the document's model absolute tolerance.</param>
        /// <returns>The Brep without holes if successful, null otherwise.</returns>
        public static Brep RemoveHoles(this Brep brep, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep, tolerance);
        }
        /// <summary>
        /// Removes inner loops, or holes, in a Brep.
        /// </summary>
        /// <param name="loops">A list of BrepLoop component indexes, where BrepLoop.LoopType == Rhino.Geometry.BrepLoopType.Inner.</param>
        /// <param name="tolerance">The tolerance. When in doubt, use the document's model absolute tolerance.</param>
        /// <returns>The Brep without holes if successful, null otherwise.</returns>
        public static Brep RemoveHoles(this Brep brep, IEnumerable<ComponentIndex> loops, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brep, loops, tolerance);
        }
    }

    public static class BrepFaceCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(BrepFace), caller);
        }
        /// <summary>
        /// Pulls one or more points to a brep face.
        /// </summary>
        /// <param name="points">Points to pull.</param>
        /// <param name="tolerance">Tolerance for pulling operation. Only points that are closer than tolerance will be pulled to the face.</param>
        /// <returns>An array of pulled points.</returns>
        public static Point3d[] PullPointsToFace(this BrepFace brepface, IEnumerable<Point3d> points, double tolerance)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), brepface, points, tolerance);
        }
        /// <summary>
        ///  Returns the surface draft angle and point at a parameter.
        /// </summary>
        /// <param name="testPoint">The u,v parameter on the face to evaluate.</param>
        /// <param name="testAngle">The angle in radians to test.</param>
        /// <param name="pullDirection">The pull direction.</param>
        /// <param name="edge">Restricts the point placement to an edge.</param>
        /// <param name="draftPoint">The draft angle point.</param>
        /// <param name="draftAngle">The draft angle in radians.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool DraftAnglePoint(this BrepFace brepface, Point2d testPoint, double testAngle, Vector3d pullDirection, bool edge, out Point3d draftPoint, out double draftAngle)
        {
            return ComputeServer.Post<bool, Point3d, double>(ApiAddress(), out draftPoint, out draftAngle, brepface, testPoint, testAngle, pullDirection, edge);
        }
        /// <summary>
        /// Remove all inner loops, or holes, from a Brep face.
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static Brep RemoveHoles(this BrepFace brepface, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brepface, tolerance);
        }
        /// <summary>
        /// Split this face using 3D trimming curves.
        /// </summary>
        /// <param name="curves">Curves to split with.</param>
        /// <param name="tolerance">Tolerance for splitting, when in doubt use the Document Absolute Tolerance.</param>
        /// <returns>A brep consisting of all the split fragments, or null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_tightboundingbox.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_tightboundingbox.cs' lang='cs'/>
        /// <code source='examples\py\ex_tightboundingbox.py' lang='py'/>
        /// </example>
        public static Brep Split(this BrepFace brepface, IEnumerable<Curve> curves, double tolerance)
        {
            return ComputeServer.Post<Brep>(ApiAddress(), brepface, curves, tolerance);
        }
        /// <summary>
        /// Tests if a parameter space point is on the interior of a trimmed face.
        /// </summary>
        /// <param name="u">Parameter space point u value.</param>
        /// <param name="v">Parameter space point v value.</param>
        /// <returns>A value describing the spatial relationship between the point and the face.</returns>
        public static PointFaceRelation IsPointOnFace(this BrepFace brepface, double u, double v)
        {
            return ComputeServer.Post<PointFaceRelation>(ApiAddress(), brepface, u, v);
        }
        /// <summary>
        /// Gets intervals where the iso curve exists on a BrepFace (trimmed surface)
        /// </summary>
        /// <param name="direction">Direction of isocurve.
        /// <para>0 = Isocurve connects all points with a constant U value.</para>
        /// <para>1 = Isocurve connects all points with a constant V value.</para>
        /// </param>
        /// <param name="constantParameter">Surface parameter that remains identical along the isocurves.</param>
        /// <returns>
        /// If direction = 0, the parameter space iso interval connects the 2d points
        /// (intervals[i][0],iso_constant) and (intervals[i][1],iso_constant).
        /// If direction = 1, the parameter space iso interval connects the 2d points
        /// (iso_constant,intervals[i][0]) and (iso_constant,intervals[i][1]).
        /// </returns>
        public static Interval[] TrimAwareIsoIntervals(this BrepFace brepface, int direction, double constantParameter)
        {
            return ComputeServer.Post<Interval[]>(ApiAddress(), brepface, direction, constantParameter);
        }
        /// <summary>
        /// Similar to IsoCurve function, except this function pays attention to trims on faces 
        /// and may return multiple curves.
        /// </summary>
        /// <param name="direction">Direction of isocurve.
        /// <para>0 = Isocurve connects all points with a constant U value.</para>
        /// <para>1 = Isocurve connects all points with a constant V value.</para>
        /// </param>
        /// <param name="constantParameter">Surface parameter that remains identical along the isocurves.</param>
        /// <returns>Isoparametric curves connecting all points with the constantParameter value.</returns>
        /// <remarks>
        /// In this function "direction" indicates which direction the resulting curve runs.
        /// 0: horizontal, 1: vertical
        /// In the other Surface functions that take a "direction" argument,
        /// "direction" indicates if "constantParameter" is a "u" or "v" parameter.
        /// </remarks>
        public static Curve[] TrimAwareIsoCurve(this BrepFace brepface, int direction, double constantParameter)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), brepface, direction, constantParameter);
        }
        /// <summary>
        /// Expert user tool that replaces the 3d surface geometry use by the face.
        /// </summary>
        /// <param name="surfaceIndex">brep surface index of new surface.</param>
        /// <returns>true if successful.</returns>
        /// <remarks>
        /// If the face had a surface and new surface has a different shape, then
        /// you probably want to call something like RebuildEdges() to move
        /// the 3d edge curves so they will lie on the new surface. This doesn't
        /// delete the old surface; call Brep.CullUnusedSurfaces() or Brep.Compact()
        /// to remove unused surfaces.
        /// </remarks>
        public static bool ChangeSurface(this BrepFace brepface, out BrepFace updatedInstance, int surfaceIndex)
        {
            return ComputeServer.Post<bool, BrepFace>(ApiAddress(), out updatedInstance, brepface, surfaceIndex);
        }
        /// <summary>
        /// Rebuild the edges used by a face so they lie on the surface.
        /// </summary>
        /// <param name="tolerance">tolerance for fitting 3d edge curves.</param>
        /// <param name="rebuildSharedEdges">
        /// if false and and edge is used by this face and a neighbor, then the edge
        /// will be skipped.
        /// </param>
        /// <param name="rebuildVertices">
        /// if true, vertex locations are updated to lie on the surface.
        /// </param>
        /// <returns>true on success.</returns>
        public static bool RebuildEdges(this BrepFace brepface, out BrepFace updatedInstance, double tolerance, bool rebuildSharedEdges, bool rebuildVertices)
        {
            return ComputeServer.Post<bool, BrepFace>(ApiAddress(), out updatedInstance, brepface, tolerance, rebuildSharedEdges, rebuildVertices);
        }
    }

    public static class CurveCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(Curve), caller);
        }
        /// <summary>
        /// Returns the type of conic section based on the curve's shape.
        /// </summary>
        /// <returns></returns>
        public static ConicSectionType GetConicSectionType(this Curve curve)
        {
            return ComputeServer.Post<ConicSectionType>(ApiAddress(), curve);
        }
        /// <summary>
        /// Interpolates a sequence of points. Used by InterpCurve Command
        /// This routine works best when degree=3.
        /// </summary>
        /// <param name="degree">The degree of the curve >=1.  Degree must be odd.</param>
        /// <param name="points">
        /// Points to interpolate (Count must be >= 2)
        /// </param>
        /// <returns>interpolated curve on success. null on failure.</returns>
        public static Curve CreateInterpolatedCurve(IEnumerable<Point3d> points, int degree)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), points, degree);
        }
        /// <summary>
        /// Interpolates a sequence of points. Used by InterpCurve Command
        /// This routine works best when degree=3.
        /// </summary>
        /// <param name="degree">The degree of the curve >=1.  Degree must be odd.</param>
        /// <param name="points">
        /// Points to interpolate. For periodic curves if the final point is a
        /// duplicate of the initial point it is  ignored. (Count must be >=2)
        /// </param>
        /// <param name="knots">
        /// Knot-style to use  and specifies if the curve should be periodic.
        /// </param>
        /// <returns>interpolated curve on success. null on failure.</returns>
        public static Curve CreateInterpolatedCurve(IEnumerable<Point3d> points, int degree, CurveKnotStyle knots)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), points, degree, knots);
        }
        /// <summary>
        /// Interpolates a sequence of points. Used by InterpCurve Command
        /// This routine works best when degree=3.
        /// </summary>
        /// <param name="degree">The degree of the curve >=1.  Degree must be odd.</param>
        /// <param name="points">
        /// Points to interpolate. For periodic curves if the final point is a
        /// duplicate of the initial point it is  ignored. (Count must be >=2)
        /// </param>
        /// <param name="knots">
        /// Knot-style to use  and specifies if the curve should be periodic.
        /// </param>
        /// <param name="startTangent">A starting tangent.</param>
        /// <param name="endTangent">An ending tangent.</param>
        /// <returns>interpolated curve on success. null on failure.</returns>
        public static Curve CreateInterpolatedCurve(IEnumerable<Point3d> points, int degree, CurveKnotStyle knots, Vector3d startTangent, Vector3d endTangent)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), points, degree, knots, startTangent, endTangent);
        }
        /// <summary>
        /// Creates a soft edited curve from an exising curve using a smooth field of influence.
        /// </summary>
        /// <param name="curve">The curve to soft edit.</param>
        /// <param name="t">
        /// A parameter on the curve to move from. This location on the curve is moved, and the move
        ///  is smoothly tapered off with increasing distance along the curve from this parameter.
        /// </param>
        /// <param name="delta">The direction and magitude, or maximum distance, of the move.</param>
        /// <param name="length">
        /// The distance along the curve from the editing point over which the strength 
        /// of the editing falls off smoothly.
        /// </param>
        /// <param name="fixEnds"></param>
        /// <returns>The soft edited curve if successful. null on failure.</returns>
        public static Curve CreateSoftEditCurve(Curve curve, double t, Vector3d delta, double length, bool fixEnds)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, t, delta, length, fixEnds);
        }
        /// <summary>
        /// Rounds the corners of a kinked curve with arcs of a single, specified radius.
        /// </summary>
        /// <param name="curve">The curve to fillet.</param>
        /// <param name="radius">The fillet radius.</param>
        /// <param name="tolerance">The tolerance. When in doubt, use the document's model space absolute tolerance.</param>
        /// <param name="angleTolerance">The angle tolerance in radians. When in doubt, use the document's model space angle tolerance.</param>
        /// <returns>The filleted curve if successful. null on failure.</returns>
        public static Curve CreateFilletCornersCurve(Curve curve, double radius, double tolerance, double angleTolerance)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, radius, tolerance, angleTolerance);
        }
        /// <summary>
        /// Creates a polycurve consisting of two tangent arc segments that connect two points and two directions.
        /// </summary>
        /// <param name="startPt">Start of the arc blend curve.</param>
        /// <param name="startDir">Start direction of the arc blend curve.</param>
        /// <param name="endPt">End of the arc blend curve.</param>
        /// <param name="endDir">End direction of the arc blend curve.</param>
        /// <param name="controlPointLengthRatio">
        /// The ratio of the control polygon lengths of the two arcs. Note, a value of 1.0 
        /// means the control polygon lengths for both arcs will be the same.
        /// </param>
        /// <returns>The arc blend curve, or null on error.</returns>
        public static Curve CreateArcBlend(Point3d startPt, Vector3d startDir, Point3d endPt, Vector3d endDir, double controlPointLengthRatio)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), startPt, startDir, endPt, endDir, controlPointLengthRatio);
        }
        /// <summary>
        /// Constructs a mean, or average, curve from two curves.
        /// </summary>
        /// <param name="curveA">A first curve.</param>
        /// <param name="curveB">A second curve.</param>
        /// <param name="angleToleranceRadians">
        /// The angle tolerance, in radians, used to match kinks between curves.
        /// If you are unsure how to set this parameter, then either use the
        /// document's angle tolerance RhinoDoc.AngleToleranceRadians,
        /// or the default value (RhinoMath.UnsetValue)
        /// </param>
        /// <returns>The average curve, or null on error.</returns>
        /// <exception cref="ArgumentNullException">If curveA or curveB are null.</exception>
        public static Curve CreateMeanCurve(Curve curveA, Curve curveB, double angleToleranceRadians)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curveA, curveB, angleToleranceRadians);
        }
        /// <summary>
        /// Constructs a mean, or average, curve from two curves.
        /// </summary>
        /// <param name="curveA">A first curve.</param>
        /// <param name="curveB">A second curve.</param>
        /// <returns>The average curve, or null on error.</returns>
        /// <exception cref="ArgumentNullException">If curveA or curveB are null.</exception>
        public static Curve CreateMeanCurve(Curve curveA, Curve curveB)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curveA, curveB);
        }
        /// <summary>
        /// Create a Blend curve between two existing curves.
        /// </summary>
        /// <param name="curveA">Curve to blend from (blending will occur at curve end point).</param>
        /// <param name="curveB">Curve to blend to (blending will occur at curve start point).</param>
        /// <param name="continuity">Continuity of blend.</param>
        /// <returns>A curve representing the blend between A and B or null on failure.</returns>
        public static Curve CreateBlendCurve(Curve curveA, Curve curveB, BlendContinuity continuity)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curveA, curveB, continuity);
        }
        /// <summary>
        /// Create a Blend curve between two existing curves.
        /// </summary>
        /// <param name="curveA">Curve to blend from (blending will occur at curve end point).</param>
        /// <param name="curveB">Curve to blend to (blending will occur at curve start point).</param>
        /// <param name="continuity">Continuity of blend.</param>
        /// <param name="bulgeA">Bulge factor at curveA end of blend. Values near 1.0 work best.</param>
        /// <param name="bulgeB">Bulge factor at curveB end of blend. Values near 1.0 work best.</param>
        /// <returns>A curve representing the blend between A and B or null on failure.</returns>
        public static Curve CreateBlendCurve(Curve curveA, Curve curveB, BlendContinuity continuity, double bulgeA, double bulgeB)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curveA, curveB, continuity, bulgeA, bulgeB);
        }
        /// <summary>
        /// Makes a curve blend between 2 curves at the parameters specified
        /// with the directions and continuities specified
        /// </summary>
        /// <param name="curve0">First curve to blend from</param>
        /// <param name="t0">Parameter on first curve for blend endpoint</param>
        /// <param name="reverse0">
        /// If false, the blend will go in the natural direction of the curve.
        /// If true, the blend will go in the opposite direction to the curve
        /// </param>
        /// <param name="continuity0">Continuity for the blend at the start</param>
        /// <param name="curve1">Second curve to blend from</param>
        /// <param name="t1">Parameter on second curve for blend endpoint</param>
        /// <param name="reverse1">
        /// If false, the blend will go in the natural direction of the curve.
        /// If true, the blend will go in the opposite direction to the curve
        /// </param>
        /// <param name="continuity1">Continuity for the blend at the end</param>
        /// <returns>The blend curve on success. null on failure</returns>
        public static Curve CreateBlendCurve(Curve curve0, double t0, bool reverse0, BlendContinuity continuity0, Curve curve1, double t1, bool reverse1, BlendContinuity continuity1)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve0, t0, reverse0, continuity0, curve1, t1, reverse1, continuity1);
        }
        /// <summary>
        /// Creates curves between two open or closed input curves. Uses the control points of the curves for finding tween curves.
        /// That means the first control point of first curve is matched to first control point of the second curve and so on.
        /// There is no matching of curves direction. Caller must match input curves direction before calling the function.
        /// </summary>
        /// <param name="curve0">The first, or starting, curve.</param>
        /// <param name="curve1">The second, or ending, curve.</param>
        /// <param name="numCurves">Number of tween curves to create.</param>
        /// <returns>An array of joint curves. This array can be empty.</returns>
        public static Curve[] CreateTweenCurves(Curve curve0, Curve curve1, int numCurves)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, curve1, numCurves);
        }
        /// <summary>
        /// Creates curves between two open or closed input curves. Uses the control points of the curves for finding tween curves.
        /// That means the first control point of first curve is matched to first control point of the second curve and so on.
        /// There is no matching of curves direction. Caller must match input curves direction before calling the function.
        /// </summary>
        /// <param name="curve0">The first, or starting, curve.</param>
        /// <param name="curve1">The second, or ending, curve.</param>
        /// <param name="numCurves">Number of tween curves to create.</param>
        /// <param name="tolerance"></param>
        /// <returns>An array of joint curves. This array can be empty.</returns>
        public static Curve[] CreateTweenCurves(Curve curve0, Curve curve1, int numCurves, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, curve1, numCurves, tolerance);
        }
        /// <summary>
        /// Creates curves between two open or closed input curves. Make the structure of input curves compatible if needed.
        /// Refits the input curves to have the same structure. The resulting curves are usually more complex than input unless
        /// input curves are compatible and no refit is needed. There is no matching of curves direction.
        /// Caller must match input curves direction before calling the function.
        /// </summary>
        /// <param name="curve0">The first, or starting, curve.</param>
        /// <param name="curve1">The second, or ending, curve.</param>
        /// <param name="numCurves">Number of tween curves to create.</param>
        /// <returns>An array of joint curves. This array can be empty.</returns>
        public static Curve[] CreateTweenCurvesWithMatching(Curve curve0, Curve curve1, int numCurves)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, curve1, numCurves);
        }
        /// <summary>
        /// Creates curves between two open or closed input curves. Make the structure of input curves compatible if needed.
        /// Refits the input curves to have the same structure. The resulting curves are usually more complex than input unless
        /// input curves are compatible and no refit is needed. There is no matching of curves direction.
        /// Caller must match input curves direction before calling the function.
        /// </summary>
        /// <param name="curve0">The first, or starting, curve.</param>
        /// <param name="curve1">The second, or ending, curve.</param>
        /// <param name="numCurves">Number of tween curves to create.</param>
        /// <param name="tolerance"></param>
        /// <returns>An array of joint curves. This array can be empty.</returns>
        public static Curve[] CreateTweenCurvesWithMatching(Curve curve0, Curve curve1, int numCurves, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, curve1, numCurves, tolerance);
        }
        /// <summary>
        /// Creates curves between two open or closed input curves. Use sample points method to make curves compatible.
        /// This is how the algorithm workd: Divides the two curves into an equal number of points, finds the midpoint between the 
        /// corresponding points on the curves and interpolates the tween curve through those points. There is no matching of curves
        /// direction. Caller must match input curves direction before calling the function.
        /// </summary>
        /// <param name="curve0">The first, or starting, curve.</param>
        /// <param name="curve1">The second, or ending, curve.</param>
        /// <param name="numCurves">Number of tween curves to create.</param>
        /// <param name="numSamples">Number of sample points along input curves.</param>
        /// <returns>>An array of joint curves. This array can be empty.</returns>
        public static Curve[] CreateTweenCurvesWithSampling(Curve curve0, Curve curve1, int numCurves, int numSamples)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, curve1, numCurves, numSamples);
        }
        /// <summary>
        /// Creates curves between two open or closed input curves. Use sample points method to make curves compatible.
        /// This is how the algorithm workd: Divides the two curves into an equal number of points, finds the midpoint between the 
        /// corresponding points on the curves and interpolates the tween curve through those points. There is no matching of curves
        /// direction. Caller must match input curves direction before calling the function.
        /// </summary>
        /// <param name="curve0">The first, or starting, curve.</param>
        /// <param name="curve1">The second, or ending, curve.</param>
        /// <param name="numCurves">Number of tween curves to create.</param>
        /// <param name="numSamples">Number of sample points along input curves.</param>
        /// <param name="tolerance"></param>
        /// <returns>>An array of joint curves. This array can be empty.</returns>
        public static Curve[] CreateTweenCurvesWithSampling(Curve curve0, Curve curve1, int numCurves, int numSamples, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, curve1, numCurves, numSamples, tolerance);
        }
        /// <summary>
        /// Joins a collection of curve segments together.
        /// </summary>
        /// <param name="inputCurves">Curve segments to join.</param>
        /// <returns>An array of curves which contains.</returns>
        public static Curve[] JoinCurves(IEnumerable<Curve> inputCurves)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), inputCurves);
        }
        /// <summary>
        /// Joins a collection of curve segments together.
        /// </summary>
        /// <param name="inputCurves">An array, a list or any enumerable set of curve segments to join.</param>
        /// <param name="joinTolerance">Joining tolerance, 
        /// i.e. the distance between segment end-points that is allowed.</param>
        /// <returns>An array of joint curves. This array can be empty.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_dividebylength.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dividebylength.cs' lang='cs'/>
        /// <code source='examples\py\ex_dividebylength.py' lang='py'/>
        /// </example>
        /// <exception cref="ArgumentNullException">If inputCurves is null.</exception>
        public static Curve[] JoinCurves(IEnumerable<Curve> inputCurves, double joinTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), inputCurves, joinTolerance);
        }
        /// <summary>
        /// Joins a collection of curve segments together.
        /// </summary>
        /// <param name="inputCurves">An array, a list or any enumerable set of curve segments to join.</param>
        /// <param name="joinTolerance">Joining tolerance, 
        /// i.e. the distance between segment end-points that is allowed.</param>
        /// <param name="preserveDirection">
        /// <para>If true, curve endpoints will be compared to curve startpoints.</para>
        /// <para>If false, all start and endpoints will be compared and copies of input curves may be reversed in output.</para>
        /// </param>
        /// <returns>An array of joint curves. This array can be empty.</returns>
        /// <exception cref="ArgumentNullException">If inputCurves is null.</exception>
        public static Curve[] JoinCurves(IEnumerable<Curve> inputCurves, double joinTolerance, bool preserveDirection)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), inputCurves, joinTolerance, preserveDirection);
        }
        /// <summary>
        /// Makes adjustments to the ends of one or both input curves so that they meet at a point.
        /// </summary>
        /// <param name="curveA">1st curve to adjust.</param>
        /// <param name="adjustStartCurveA">
        /// Which end of the 1st curve to adjust: true is start, false is end.
        /// </param>
        /// <param name="curveB">2nd curve to adjust.</param>
        /// <param name="adjustStartCurveB">
        /// which end of the 2nd curve to adjust true==start, false==end.
        /// </param>
        /// <returns>true on success.</returns>
        public static bool MakeEndsMeet(Curve curveA, bool adjustStartCurveA, Curve curveB, bool adjustStartCurveB)
        {
            return ComputeServer.Post<bool>(ApiAddress(), curveA, adjustStartCurveA, curveB, adjustStartCurveB);
        }
        /// <summary>
        /// Computes the fillet arc for a curve filleting operation.
        /// </summary>
        /// <param name="curve0">First curve to fillet.</param>
        /// <param name="curve1">Second curve to fillet.</param>
        /// <param name="radius">Fillet radius.</param>
        /// <param name="t0Base">Parameter on curve0 where the fillet ought to start (approximately).</param>
        /// <param name="t1Base">Parameter on curve1 where the fillet ought to end (approximately).</param>
        /// <returns>The fillet arc on success, or Arc.Unset on failure.</returns>
        public static Arc CreateFillet(Curve curve0, Curve curve1, double radius, double t0Base, double t1Base)
        {
            return ComputeServer.Post<Arc>(ApiAddress(), curve0, curve1, radius, t0Base, t1Base);
        }
        /// <summary>
        /// Creates a tangent arc between two curves and trims or extends the curves to the arc.
        /// </summary>
        /// <param name="curve0">The first curve to fillet.</param>
        /// <param name="point0">
        /// A point on the first curve that is near the end where the fillet will
        /// be created.
        /// </param>
        /// <param name="curve1">The second curve to fillet.</param>
        /// <param name="point1">
        /// A point on the second curve that is near the end where the fillet will
        /// be created.
        /// </param>
        /// <param name="radius">The radius of the fillet.</param>
        /// <param name="join">Join the output curves.</param>
        /// <param name="trim">
        /// Trim copies of the input curves to the output fillet curve.
        /// </param>
        /// <param name="arcExtension">
        /// Applies when arcs are filleted but need to be extended to meet the
        /// fillet curve or chamfer line. If true, then the arc is extended
        /// maintaining its validity. If false, then the arc is extended with a
        /// line segment, which is joined to the arc converting it to a polycurve.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance, generally the document's absolute tolerance.
        /// </param>
        /// <param name="angleTolerance"></param>
        /// <returns>
        /// The results of the fillet operation. The number of output curves depends
        /// on the input curves and the values of the parameters that were used
        /// during the fillet operation. In most cases, the output array will contain
        /// either one or three curves, although two curves can be returned if the
        /// radius is zero and join = false.
        /// For example, if both join and trim = true, then the output curve
        /// will be a polycurve containing the fillet curve joined with trimmed copies
        /// of the input curves. If join = false and trim = true, then three curves,
        /// the fillet curve and trimmed copies of the input curves, will be returned.
        /// If both join and trim = false, then just the fillet curve is returned.
        /// </returns>
        /// <example>
        /// <code source='examples\vbnet\ex_filletcurves.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_filletcurves.cs' lang='cs'/>
        /// <code source='examples\py\ex_filletcurves.py' lang='py'/>
        /// </example>
        public static Curve[] CreateFilletCurves(Curve curve0, Point3d point0, Curve curve1, Point3d point1, double radius, bool join, bool trim, bool arcExtension, double tolerance, double angleTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve0, point0, curve1, point1, radius, join, trim, arcExtension, tolerance, angleTolerance);
        }
        /// <summary>
        /// Calculates the boolean union of two or more closed, planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curves">The co-planar curves to union.</param>
        /// <returns>Result curves on success, empty array if no union could be calculated.</returns>
        public static Curve[] CreateBooleanUnion(IEnumerable<Curve> curves)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curves);
        }
        /// <summary>
        /// Calculates the boolean union of two or more closed, planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curves">The co-planar curves to union.</param>
        /// <param name="tolerance"></param>
        /// <returns>Result curves on success, empty array if no union could be calculated.</returns>
        public static Curve[] CreateBooleanUnion(IEnumerable<Curve> curves, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curves, tolerance);
        }
        /// <summary>
        /// Calculates the boolean intersection of two closed, planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curveA">The first closed, planar curve.</param>
        /// <param name="curveB">The second closed, planar curve.</param>
        /// <returns>Result curves on success, empty array if no intersection could be calculated.</returns>
        public static Curve[] CreateBooleanIntersection(Curve curveA, Curve curveB)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, curveB);
        }
        /// <summary>
        /// Calculates the boolean intersection of two closed, planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curveA">The first closed, planar curve.</param>
        /// <param name="curveB">The second closed, planar curve.</param>
        /// <param name="tolerance"></param>
        /// <returns>Result curves on success, empty array if no intersection could be calculated.</returns>
        public static Curve[] CreateBooleanIntersection(Curve curveA, Curve curveB, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, curveB, tolerance);
        }
        /// <summary>
        /// Calculates the boolean difference between two closed, planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curveA">The first closed, planar curve.</param>
        /// <param name="curveB">The second closed, planar curve.</param>
        /// <returns>Result curves on success, empty array if no difference could be calculated.</returns>
        public static Curve[] CreateBooleanDifference(Curve curveA, Curve curveB)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, curveB);
        }
        /// <summary>
        /// Calculates the boolean difference between two closed, planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curveA">The first closed, planar curve.</param>
        /// <param name="curveB">The second closed, planar curve.</param>
        /// <param name="tolerance"></param>
        /// <returns>Result curves on success, empty array if no difference could be calculated.</returns>
        public static Curve[] CreateBooleanDifference(Curve curveA, Curve curveB, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, curveB, tolerance);
        }
        /// <summary>
        /// Calculates the boolean difference between a closed planar curve, and a list of closed planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curveA">The first closed, planar curve.</param>
        /// <param name="subtractors">curves to subtract from the first closed curve.</param>
        /// <returns>Result curves on success, empty array if no difference could be calculated.</returns>
        public static Curve[] CreateBooleanDifference(Curve curveA, IEnumerable<Curve> subtractors)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, subtractors);
        }
        /// <summary>
        /// Calculates the boolean difference between a closed planar curve, and a list of closed planar curves. 
        /// Note, curves must be co-planar.
        /// </summary>
        /// <param name="curveA">The first closed, planar curve.</param>
        /// <param name="subtractors">curves to subtract from the first closed curve.</param>
        /// <param name="tolerance"></param>
        /// <returns>Result curves on success, empty array if no difference could be calculated.</returns>
        public static Curve[] CreateBooleanDifference(Curve curveA, IEnumerable<Curve> subtractors, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, subtractors, tolerance);
        }
        /// <summary>
        /// Creates outline curves created from a text string. The functionality is similar to what you find in Rhino's TextObject command or TextEntity.Explode() in RhinoCommon.
        /// </summary>
        /// <param name="text">The text from which to create outline curves.</param>
        /// <param name="font">The text font.</param>
        /// <param name="textHeight">The text height.</param>
        /// <param name="textStyle">The font style. The font style can be any number of the following: 0 - Normal, 1 - Bold, 2 - Italic</param>
        /// <param name="closeLoops">Set this value to True when dealing with normal fonts and when you expect closed loops. You may want to set this to False when specifying a single-stroke font where you don't want closed loops.</param>
        /// <param name="plane">The plane on which the outline curves will lie.</param>
        /// <param name="smallCapsScale">Displays lower-case letters as small caps. Set the relative text size to a percentage of the normal text.</param>
        /// <param name="tolerance">The tolerance for the operation.</param>
        /// <returns>An array containing one or more curves if successful.</returns>
        public static Curve[] CreateTextOutlines(string text, string font, double textHeight, int textStyle, bool closeLoops, Plane plane, double smallCapsScale, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), text, font, textHeight, textStyle, closeLoops, plane, smallCapsScale, tolerance);
        }
        /// <summary>
        /// Creates a third curve from two curves that are planar in different construction planes. 
        /// The new curve looks the same as each of the original curves when viewed in each plane.
        /// </summary>
        /// <param name="curveA">The first curve.</param>
        /// <param name="curveB">The second curve.</param>
        /// <param name="vectorA">A vector defining the normal direction of the plane which the first curve is drawn upon.</param>
        /// <param name="vectorB">A vector defining the normal direction of the plane which the seconf curve is drawn upon.</param>
        /// <param name="tolerance">The tolerance for the operation.</param>
        /// <param name="angleTolerance">The angle tolerance for the operation.</param>
        /// <returns>An array containing one or more curves if successful.</returns>
        public static Curve[] CreateCurve2View(Curve curveA, Curve curveB, Vector3d vectorA, Vector3d vectorB, double tolerance, double angleTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curveA, curveB, vectorA, vectorB, tolerance, angleTolerance);
        }
        /// <summary>
        /// Determines whether two curves travel more or less in the same direction.
        /// </summary>
        /// <param name="curveA">First curve to test.</param>
        /// <param name="curveB">Second curve to test.</param>
        /// <returns>true if both curves more or less point in the same direction, 
        /// false if they point in the opposite directions.</returns>
        public static bool DoDirectionsMatch(Curve curveA, Curve curveB)
        {
            return ComputeServer.Post<bool>(ApiAddress(), curveA, curveB);
        }
        /// <summary>
        /// Projects a curve to a mesh using a direction and tolerance.
        /// </summary>
        /// <param name="curve">A curve.</param>
        /// <param name="mesh">A mesh.</param>
        /// <param name="direction">A direction vector.</param>
        /// <param name="tolerance">A tolerance value.</param>
        /// <returns>A curve array.</returns>
        public static Curve[] ProjectToMesh(Curve curve, Mesh mesh, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, mesh, direction, tolerance);
        }
        /// <summary>
        /// Projects a curve to a set of meshes using a direction and tolerance.
        /// </summary>
        /// <param name="curve">A curve.</param>
        /// <param name="meshes">A list, an array or any enumerable of meshes.</param>
        /// <param name="direction">A direction vector.</param>
        /// <param name="tolerance">A tolerance value.</param>
        /// <returns>A curve array.</returns>
        public static Curve[] ProjectToMesh(Curve curve, IEnumerable<Mesh> meshes, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, meshes, direction, tolerance);
        }
        /// <summary>
        /// Projects a curve to a set of meshes using a direction and tolerance.
        /// </summary>
        /// <param name="curves">A list, an array or any enumerable of curves.</param>
        /// <param name="meshes">A list, an array or any enumerable of meshes.</param>
        /// <param name="direction">A direction vector.</param>
        /// <param name="tolerance">A tolerance value.</param>
        /// <returns>A curve array.</returns>
        public static Curve[] ProjectToMesh(IEnumerable<Curve> curves, IEnumerable<Mesh> meshes, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curves, meshes, direction, tolerance);
        }
        /// <summary>
        /// Projects a Curve onto a Brep along a given direction.
        /// </summary>
        /// <param name="curve">Curve to project.</param>
        /// <param name="brep">Brep to project onto.</param>
        /// <param name="direction">Direction of projection.</param>
        /// <param name="tolerance">Tolerance to use for projection.</param>
        /// <returns>An array of projected curves or empty array if the projection set is empty.</returns>
        public static Curve[] ProjectToBrep(Curve curve, Brep brep, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, brep, direction, tolerance);
        }
        /// <summary>
        /// Projects a Curve onto a collection of Breps along a given direction.
        /// </summary>
        /// <param name="curve">Curve to project.</param>
        /// <param name="breps">Breps to project onto.</param>
        /// <param name="direction">Direction of projection.</param>
        /// <param name="tolerance">Tolerance to use for projection.</param>
        /// <returns>An array of projected curves or empty array if the projection set is empty.</returns>
        public static Curve[] ProjectToBrep(Curve curve, IEnumerable<Brep> breps, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, breps, direction, tolerance);
        }
        /// <summary>
        /// Projects a Curve onto a collection of Breps along a given direction.
        /// </summary>
        /// <param name="curve">Curve to project.</param>
        /// <param name="breps">Breps to project onto.</param>
        /// <param name="direction">Direction of projection.</param>
        /// <param name="tolerance">Tolerance to use for projection.</param>
        /// <param name="brepIndices">(out) Integers that identify for each resulting curve which Brep it was projected onto.</param>
        /// <returns>An array of projected curves or null if the projection set is empty.</returns>
        public static Curve[] ProjectToBrep(Curve curve, IEnumerable<Brep> breps, Vector3d direction, double tolerance, out int[] brepIndices)
        {
            return ComputeServer.Post<Curve[], int[]>(ApiAddress(), out brepIndices, curve, breps, direction, tolerance);
        }
        /// <summary>
        /// Projects a collection of Curves onto a collection of Breps along a given direction.
        /// </summary>
        /// <param name="curves">Curves to project.</param>
        /// <param name="breps">Breps to project onto.</param>
        /// <param name="direction">Direction of projection.</param>
        /// <param name="tolerance">Tolerance to use for projection.</param>
        /// <returns>An array of projected curves or empty array if the projection set is empty.</returns>
        public static Curve[] ProjectToBrep(IEnumerable<Curve> curves, IEnumerable<Brep> breps, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curves, breps, direction, tolerance);
        }
        /// <summary>
        /// Projects a collection of Curves onto a collection of Breps along a given direction.
        /// </summary>
        /// <param name="curves">Curves to project.</param>
        /// <param name="breps">Breps to project onto.</param>
        /// <param name="direction">Direction of projection.</param>
        /// <param name="tolerance">Tolerance to use for projection.</param>
        /// <param name="curveIndices">Index of which curve in the input list was the source for a curve in the return array.</param>
        /// <param name="brepIndices">Index of which brep was used to generate a curve in the return array.</param>
        /// <returns>An array of projected curves. Array is empty if the projection set is empty.</returns>
        public static Curve[] ProjectToBrep(IEnumerable<Curve> curves, IEnumerable<Brep> breps, Vector3d direction, double tolerance, out int[] curveIndices, out int[] brepIndices)
        {
            return ComputeServer.Post<Curve[], int[], int[]>(ApiAddress(), out curveIndices, out brepIndices, curves, breps, direction, tolerance);
        }
        /// <summary>
        /// Constructs a curve by projecting an existing curve to a plane.
        /// </summary>
        /// <param name="curve">A curve.</param>
        /// <param name="plane">A plane.</param>
        /// <returns>The projected curve on success; null on failure.</returns>
        public static Curve ProjectToPlane(Curve curve, Plane plane)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, plane);
        }
        /// <summary>
        /// Pull a curve to a BrepFace using closest point projection.
        /// </summary>
        /// <param name="curve">Curve to pull.</param>
        /// <param name="face">Brepface that pulls.</param>
        /// <param name="tolerance">Tolerance to use for pulling.</param>
        /// <returns>An array of pulled curves, or an empty array on failure.</returns>
        public static Curve[] PullToBrepFace(Curve curve, BrepFace face, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, face, tolerance);
        }
        /// <summary>
        /// Determines whether two coplanar simple closed curves are disjoint or intersect;
        /// otherwise, if the regions have a containment relationship, discovers
        /// which curve encloses the other.
        /// </summary>
        /// <param name="curveA">A first curve.</param>
        /// <param name="curveB">A second curve.</param>
        /// <param name="testPlane">A plane.</param>
        /// <param name="tolerance">A tolerance value.</param>
        /// <returns>
        /// A value indicating the relationship between the first and the second curve.
        /// </returns>
        public static RegionContainment PlanarClosedCurveRelationship(Curve curveA, Curve curveB, Plane testPlane, double tolerance)
        {
            return ComputeServer.Post<RegionContainment>(ApiAddress(), curveA, curveB, testPlane, tolerance);
        }
        /// <summary>
        /// Determines if two coplanar curves collide (intersect).
        /// </summary>
        /// <param name="curveA">A curve.</param>
        /// <param name="curveB">Another curve.</param>
        /// <param name="testPlane">A valid plane containing the curves.</param>
        /// <param name="tolerance">A tolerance value for intersection.</param>
        /// <returns>true if the curves intersect, otherwise false</returns>
        public static bool PlanarCurveCollision(Curve curveA, Curve curveB, Plane testPlane, double tolerance)
        {
            return ComputeServer.Post<bool>(ApiAddress(), curveA, curveB, testPlane, tolerance);
        }
        /// <summary>
        /// Polylines will be exploded into line segments. ExplodeCurves will
        /// return the curves in topological order.
        /// </summary>
        /// <returns>
        /// An array of all the segments that make up this curve.
        /// </returns>
        public static Curve[] DuplicateSegments(this Curve curve, out Curve updatedInstance)
        {
            return ComputeServer.Post<Curve[], Curve>(ApiAddress(), out updatedInstance, curve);
        }
        /// <summary>
        /// If IsClosed, just return true. Otherwise, decide if curve can be closed as 
        /// follows: Linear curves polylinear curves with 2 segments, Nurbs with 3 or less 
        /// control points cannot be made closed. Also, if tolerance > 0 and the gap between 
        /// start and end is larger than tolerance, curve cannot be made closed. 
        /// Adjust the curve's endpoint to match its start point.
        /// </summary>
        /// <param name="tolerance">
        /// If nonzero, and the gap is more than tolerance, curve cannot be made closed.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool MakeClosed(this Curve curve, out Curve updatedInstance, double tolerance)
        {
            return ComputeServer.Post<bool, Curve>(ApiAddress(), out updatedInstance, curve, tolerance);
        }
        /// <summary>
        /// Find parameter of the point on a curve that is locally closest to 
        /// the testPoint.  The search for a local close point starts at
        /// a seed parameter.
        /// </summary>
        /// <param name="testPoint">A point to test against.</param>
        /// <param name="seed">The seed parameter.</param>
        /// <param name="t">>Parameter of the curve that is closest to testPoint.</param>
        /// <returns>true if the search is successful, false if the search fails.</returns>
        public static bool LcoalClosestPoint(this Curve curve, Point3d testPoint, double seed, out double t)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, testPoint, seed);
        }
        /// <summary>
        /// Finds parameter of the point on a curve that is closest to testPoint.
        /// If the maximumDistance parameter is > 0, then only points whose distance
        /// to the given point is &lt;= maximumDistance will be returned.  Using a 
        /// positive value of maximumDistance can substantially speed up the search.
        /// </summary>
        /// <param name="testPoint">Point to search from.</param>
        /// <param name="t">Parameter of local closest point.</param>
        /// <returns>true on success, false on failure.</returns>
        public static bool ClosestPoint(this Curve curve, Point3d testPoint, out double t)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, testPoint);
        }
        /// <summary>
        /// Finds the parameter of the point on a curve that is closest to testPoint.
        /// If the maximumDistance parameter is > 0, then only points whose distance
        /// to the given point is &lt;= maximumDistance will be returned.  Using a 
        /// positive value of maximumDistance can substantially speed up the search.
        /// </summary>
        /// <param name="testPoint">Point to project.</param>
        /// <param name="t">parameter of local closest point returned here.</param>
        /// <param name="maximumDistance">The maximum allowed distance.
        /// <para>Past this distance, the search is given up and false is returned.</para>
        /// <para>Use 0 to turn off this parameter.</para></param>
        /// <returns>true on success, false on failure.</returns>
        public static bool ClosestPoint(this Curve curve, Point3d testPoint, out double t, double maximumDistance)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, testPoint, maximumDistance);
        }
        /// <summary>
        /// Gets closest points between this and another curves.
        /// </summary>
        /// <param name="otherCurve">The other curve.</param>
        /// <param name="pointOnThisCurve">The point on this curve. This out parameter is assigned during this call.</param>
        /// <param name="pointOnOtherCurve">The point on other curve. This out parameter is assigned during this call.</param>
        /// <returns>true on success; false on error.</returns>
        public static bool ClosestPoints(this Curve curve, Curve otherCurve, out Point3d pointOnThisCurve, out Point3d pointOnOtherCurve)
        {
            return ComputeServer.Post<bool, Point3d, Point3d>(ApiAddress(), out pointOnThisCurve, out pointOnOtherCurve, curve, otherCurve);
        }
        /// <summary>
        /// Computes the relationship between a point and a closed curve region. 
        /// This curve must be closed or the return value will be Unset.
        /// Both curve and point are projected to the World XY plane.
        /// </summary>
        /// <param name="testPoint">Point to test.</param>
        /// <returns>Relationship between point and curve region.</returns>
        public static PointContainment Contains(this Curve curve, Point3d testPoint)
        {
            return ComputeServer.Post<PointContainment>(ApiAddress(), curve, testPoint);
        }
        /// <summary>
        /// Computes the relationship between a point and a closed curve region. 
        /// This curve must be closed or the return value will be Unset.
        /// </summary>
        /// <param name="testPoint">Point to test.</param>
        /// <param name="plane">Plane in in which to compare point and region.</param>
        /// <returns>Relationship between point and curve region.</returns>
        public static PointContainment Contains(this Curve curve, Point3d testPoint, Plane plane)
        {
            return ComputeServer.Post<PointContainment>(ApiAddress(), curve, testPoint, plane);
        }
        /// <summary>
        /// Computes the relationship between a point and a closed curve region. 
        /// This curve must be closed or the return value will be Unset.
        /// </summary>
        /// <param name="testPoint">Point to test.</param>
        /// <param name="plane">Plane in in which to compare point and region.</param>
        /// <param name="tolerance">Tolerance to use during comparison.</param>
        /// <returns>Relationship between point and curve region.</returns>
        public static PointContainment Contains(this Curve curve, Point3d testPoint, Plane plane, double tolerance)
        {
            return ComputeServer.Post<PointContainment>(ApiAddress(), curve, testPoint, plane, tolerance);
        }
        /// <summary>
        /// Returns the parameter values of all local extrema. 
        /// Parameter values are in increasing order so consecutive extrema 
        /// define an interval on which each component of the curve is monotone. 
        /// Note, non-periodic curves always return the end points.
        /// </summary>
        /// <param name="direction">The direction in which to perform the calculation.</param>
        /// <returns>The parameter values of all local extrema.</returns>
        public static double[] ExtremeParameters(this Curve curve, Vector3d direction)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, direction);
        }
        /// <summary>
        /// Removes kinks from a curve. Periodic curves deform smoothly without kinks.
        /// </summary>
        /// <param name="curve">The curve to make periodic. Curve must have degree >= 2.</param>
        /// <returns>The resulting curve if successful, null otherwise.</returns>
        public static Curve CreatePeriodicCurve(Curve curve)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve);
        }
        /// <summary>
        /// Removes kinks from a curve. Periodic curves deform smoothly without kinks.
        /// </summary>
        /// <param name="curve">The curve to make periodic. Curve must have degree >= 2.</param>
        /// <param name="smooth">
        /// If true, smooths any kinks in the curve and moves control points to make a smooth curve. 
        /// If false, control point locations are not changed or changed minimally (only one point may move) and only the knot vector is altered.
        /// </param>
        /// <returns>The resulting curve if successful, null otherwise.</returns>
        public static Curve CreatePeriodicCurve(Curve curve, bool smooth)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, smooth);
        }
        /// <summary>
        /// Gets a point at a certain length along the curve. The length must be 
        /// non-negative and less than or equal to the length of the curve. 
        /// Lengths will not be wrapped when the curve is closed or periodic.
        /// </summary>
        /// <param name="length">Length along the curve between the start point and the returned point.</param>
        /// <returns>Point on the curve at the specified length from the start point or Poin3d.Unset on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_arclengthpoint.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_arclengthpoint.cs' lang='cs'/>
        /// <code source='examples\py\ex_arclengthpoint.py' lang='py'/>
        /// </example>
        public static Point3d PointAtLength(this Curve curve, double length)
        {
            return ComputeServer.Post<Point3d>(ApiAddress(), curve, length);
        }
        /// <summary>
        /// Gets a point at a certain normalized length along the curve. The length must be 
        /// between or including 0.0 and 1.0, where 0.0 equals the start of the curve and 
        /// 1.0 equals the end of the curve. 
        /// </summary>
        /// <param name="length">Normalized length along the curve between the start point and the returned point.</param>
        /// <returns>Point on the curve at the specified normalized length from the start point or Poin3d.Unset on failure.</returns>
        public static Point3d PointAtNormalizedLength(this Curve curve, double length)
        {
            return ComputeServer.Post<Point3d>(ApiAddress(), curve, length);
        }
        /// <summary>
        /// Return a 3d frame at a parameter. This is slightly different than FrameAt in
        /// that the frame is computed in a way so there is minimal rotation from one
        /// frame to the next.
        /// </summary>
        /// <param name="t">Evaluation parameter.</param>
        /// <param name="plane">The frame is returned here.</param>
        /// <returns>true on success, false on failure.</returns>
        public static bool PerpendicularFrameAt(this Curve curve, double t, out Plane plane)
        {
            return ComputeServer.Post<bool, Plane>(ApiAddress(), out plane, curve, t);
        }
        /// <summary>
        /// Gets a collection of perpendicular frames along the curve. Perpendicular frames 
        /// are also known as 'Zero-twisting frames' and they minimize rotation from one frame to the next.
        /// </summary>
        /// <param name="parameters">A collection of <i>strictly increasing</i> curve parameters to place perpendicular frames on.</param>
        /// <returns>An array of perpendicular frames on success or null on failure.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the curve parameters are not increasing.</exception>
        public static Plane[] GetPerpendicularFrames(this Curve curve, IEnumerable<double> parameters)
        {
            return ComputeServer.Post<Plane[]>(ApiAddress(), curve, parameters);
        }
        /// <summary>
        /// Gets the length of the curve with a fractional tolerance of 1.0e-8.
        /// </summary>
        /// <returns>The length of the curve on success, or zero on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_arclengthpoint.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_arclengthpoint.cs' lang='cs'/>
        /// <code source='examples\py\ex_arclengthpoint.py' lang='py'/>
        /// </example>
        public static double GetLength(this Curve curve)
        {
            return ComputeServer.Post<double>(ApiAddress(), curve);
        }
        /// <summary>Get the length of the curve.</summary>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision. 
        /// fabs(("exact" length from start to t) - arc_length)/arc_length &lt;= fractionalTolerance.
        /// </param>
        /// <returns>The length of the curve on success, or zero on failure.</returns>
        public static double GetLength(this Curve curve, double fractionalTolerance)
        {
            return ComputeServer.Post<double>(ApiAddress(), curve, fractionalTolerance);
        }
        /// <summary>Get the length of a sub-section of the curve with a fractional tolerance of 1e-8.</summary>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve (must be non-decreasing).
        /// </param>
        /// <returns>The length of the sub-curve on success, or zero on failure.</returns>
        public static double GetLength(this Curve curve, Interval subdomain)
        {
            return ComputeServer.Post<double>(ApiAddress(), curve, subdomain);
        }
        /// <summary>Get the length of a sub-section of the curve.</summary>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision. 
        /// fabs(("exact" length from start to t) - arc_length)/arc_length &lt;= fractionalTolerance.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve (must be non-decreasing).
        /// </param>
        /// <returns>The length of the sub-curve on success, or zero on failure.</returns>
        public static double GetLength(this Curve curve, double fractionalTolerance, Interval subdomain)
        {
            return ComputeServer.Post<double>(ApiAddress(), curve, fractionalTolerance, subdomain);
        }
        /// <summary>Used to quickly find short curves.</summary>
        /// <param name="tolerance">Length threshold value for "shortness".</param>
        /// <returns>true if the length of the curve is &lt;= tolerance.</returns>
        /// <remarks>Faster than calling Length() and testing the result.</remarks>
        /// <example>
        /// <code source='examples\vbnet\ex_dividebylength.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dividebylength.cs' lang='cs'/>
        /// <code source='examples\py\ex_dividebylength.py' lang='py'/>
        /// </example>
        public static bool IsShort(this Curve curve, double tolerance)
        {
            return ComputeServer.Post<bool>(ApiAddress(), curve, tolerance);
        }
        /// <summary>Used to quickly find short curves.</summary>
        /// <param name="tolerance">Length threshold value for "shortness".</param>
        /// <param name="subdomain">
        /// The test is performed on the interval that is the intersection of subdomain with Domain()
        /// </param>
        /// <returns>true if the length of the curve is &lt;= tolerance.</returns>
        /// <remarks>Faster than calling Length() and testing the result.</remarks>
        public static bool IsShort(this Curve curve, double tolerance, Interval subdomain)
        {
            return ComputeServer.Post<bool>(ApiAddress(), curve, tolerance, subdomain);
        }
        /// <summary>
        /// Looks for segments that are shorter than tolerance that can be removed. 
        /// Does not change the domain, but it will change the relative parameterization.
        /// </summary>
        /// <param name="tolerance">Tolerance which defines "short" segments.</param>
        /// <returns>
        /// true if removable short segments were found. 
        /// false if no removable short segments were found.
        /// </returns>
        public static bool RemoveShortSegments(this Curve curve, out Curve updatedInstance, double tolerance)
        {
            return ComputeServer.Post<bool, Curve>(ApiAddress(), out updatedInstance, curve, tolerance);
        }
        /// <summary>
        /// Gets the parameter along the curve which coincides with a given length along the curve. 
        /// A fractional tolerance of 1e-8 is used in this version of the function.
        /// </summary>
        /// <param name="segmentLength">
        /// Length of segment to measure. Must be less than or equal to the length of the curve.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from the curve start point to t equals length.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool LengthParameter(this Curve curve, double segmentLength, out double t)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, segmentLength);
        }
        /// <summary>
        /// Gets the parameter along the curve which coincides with a given length along the curve.
        /// </summary>
        /// <param name="segmentLength">
        /// Length of segment to measure. Must be less than or equal to the length of the curve.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from the curve start point to t equals s.
        /// </param>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision.
        /// fabs(("exact" length from start to t) - arc_length)/arc_length &lt;= fractionalTolerance.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool LengthParameter(this Curve curve, double segmentLength, out double t, double fractionalTolerance)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, segmentLength, fractionalTolerance);
        }
        /// <summary>
        /// Gets the parameter along the curve which coincides with a given length along the curve. 
        /// A fractional tolerance of 1e-8 is used in this version of the function.
        /// </summary>
        /// <param name="segmentLength">
        /// Length of segment to measure. Must be less than or equal to the length of the subdomain.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from the start of the subdomain to t is s.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve rather than the whole curve.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool LengthParameter(this Curve curve, double segmentLength, out double t, Interval subdomain)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, segmentLength, subdomain);
        }
        /// <summary>
        /// Gets the parameter along the curve which coincides with a given length along the curve.
        /// </summary>
        /// <param name="segmentLength">
        /// Length of segment to measure. Must be less than or equal to the length of the subdomain.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from the start of the subdomain to t is s.
        /// </param>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision. 
        /// fabs(("exact" length from start to t) - arc_length)/arc_length &lt;= fractionalTolerance.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve rather than the whole curve.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool LengthParameter(this Curve curve, double segmentLength, out double t, double fractionalTolerance, Interval subdomain)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, segmentLength, fractionalTolerance, subdomain);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve. 
        /// A fractional tolerance of 1e-8 is used in this version of the function.
        /// </summary>
        /// <param name="s">
        /// Normalized arc length parameter. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from its start to t is arc_length.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool NormalizedLengthParameter(this Curve curve, double s, out double t)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, s);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve.
        /// </summary>
        /// <param name="s">
        /// Normalized arc length parameter. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from its start to t is arc_length.
        /// </param>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision. 
        /// fabs(("exact" length from start to t) - arc_length)/arc_length &lt;= fractionalTolerance.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool NormalizedLengthParameter(this Curve curve, double s, out double t, double fractionalTolerance)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, s, fractionalTolerance);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve. 
        /// A fractional tolerance of 1e-8 is used in this version of the function.
        /// </summary>
        /// <param name="s">
        /// Normalized arc length parameter. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from its start to t is arc_length.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool NormalizedLengthParameter(this Curve curve, double s, out double t, Interval subdomain)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, s, subdomain);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve.
        /// </summary>
        /// <param name="s">
        /// Normalized arc length parameter. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="t">
        /// Parameter such that the length of the curve from its start to t is arc_length.
        /// </param>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision. 
        /// fabs(("exact" length from start to t) - arc_length)/arc_length &lt;= fractionalTolerance.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public static bool NormalizedLengthParameter(this Curve curve, double s, out double t, double fractionalTolerance, Interval subdomain)
        {
            return ComputeServer.Post<bool, double>(ApiAddress(), out t, curve, s, fractionalTolerance, subdomain);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve. 
        /// A fractional tolerance of 1e-8 is used in this version of the function.
        /// </summary>
        /// <param name="s">
        /// Array of normalized arc length parameters. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="absoluteTolerance">
        /// If absoluteTolerance > 0, then the difference between (s[i+1]-s[i])*curve_length 
        /// and the length of the curve segment from t[i] to t[i+1] will be &lt;= absoluteTolerance.
        /// </param>
        /// <returns>
        /// If successful, array of curve parameters such that the length of the curve from its start to t[i] is s[i]*curve_length. 
        /// Null on failure.
        /// </returns>
        public static double[] NormalizedLengthParameters(this Curve curve, double[] s, double absoluteTolerance)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, s, absoluteTolerance);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve.
        /// </summary>
        /// <param name="s">
        /// Array of normalized arc length parameters. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="absoluteTolerance">
        /// If absoluteTolerance > 0, then the difference between (s[i+1]-s[i])*curve_length 
        /// and the length of the curve segment from t[i] to t[i+1] will be &lt;= absoluteTolerance.
        /// </param>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision for each segment. 
        /// fabs("true" length - actual length)/(actual length) &lt;= fractionalTolerance.
        /// </param>
        /// <returns>
        /// If successful, array of curve parameters such that the length of the curve from its start to t[i] is s[i]*curve_length. 
        /// Null on failure.
        /// </returns>
        public static double[] NormalizedLengthParameters(this Curve curve, double[] s, double absoluteTolerance, double fractionalTolerance)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, s, absoluteTolerance, fractionalTolerance);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve. 
        /// A fractional tolerance of 1e-8 is used in this version of the function.
        /// </summary>
        /// <param name="s">
        /// Array of normalized arc length parameters. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="absoluteTolerance">
        /// If absoluteTolerance > 0, then the difference between (s[i+1]-s[i])*curve_length 
        /// and the length of the curve segment from t[i] to t[i+1] will be &lt;= absoluteTolerance.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve. 
        /// A 0.0 s value corresponds to subdomain->Min() and a 1.0 s value corresponds to subdomain->Max().
        /// </param>
        /// <returns>
        /// If successful, array of curve parameters such that the length of the curve from its start to t[i] is s[i]*curve_length. 
        /// Null on failure.
        /// </returns>
        public static double[] NormalizedLengthParameters(this Curve curve, double[] s, double absoluteTolerance, Interval subdomain)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, s, absoluteTolerance, subdomain);
        }
        /// <summary>
        /// Input the parameter of the point on the curve that is a prescribed arc length from the start of the curve.
        /// </summary>
        /// <param name="s">
        /// Array of normalized arc length parameters. 
        /// E.g., 0 = start of curve, 1/2 = midpoint of curve, 1 = end of curve.
        /// </param>
        /// <param name="absoluteTolerance">
        /// If absoluteTolerance > 0, then the difference between (s[i+1]-s[i])*curve_length 
        /// and the length of the curve segment from t[i] to t[i+1] will be &lt;= absoluteTolerance.
        /// </param>
        /// <param name="fractionalTolerance">
        /// Desired fractional precision for each segment. 
        /// fabs("true" length - actual length)/(actual length) &lt;= fractionalTolerance.
        /// </param>
        /// <param name="subdomain">
        /// The calculation is performed on the specified sub-domain of the curve. 
        /// A 0.0 s value corresponds to subdomain->Min() and a 1.0 s value corresponds to subdomain->Max().
        /// </param>
        /// <returns>
        /// If successful, array of curve parameters such that the length of the curve from its start to t[i] is s[i]*curve_length. 
        /// Null on failure.
        /// </returns>
        public static double[] NormalizedLengthParameters(this Curve curve, double[] s, double absoluteTolerance, double fractionalTolerance, Interval subdomain)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, s, absoluteTolerance, fractionalTolerance, subdomain);
        }
        /// <summary>
        /// Divide the curve into a number of equal-length segments.
        /// </summary>
        /// <param name="segmentCount">Segment count. Note that the number of division points may differ from the segment count.</param>
        /// <param name="includeEnds">If true, then the point at the start of the first division segment is returned.</param>
        /// <returns>
        /// List of curve parameters at the division points on success, null on failure.
        /// </returns>
        public static double[] DivideByCount(this Curve curve, int segmentCount, bool includeEnds)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, segmentCount, includeEnds);
        }
        /// <summary>
        /// Divide the curve into a number of equal-length segments.
        /// </summary>
        /// <param name="segmentCount">Segment count. Note that the number of division points may differ from the segment count.</param>
        /// <param name="includeEnds">If true, then the point at the start of the first division segment is returned.</param>
        /// <param name="points">A list of division points. If the function returns successfully, this point-array will be filled in.</param>
        /// <returns>Array containing division curve parameters on success, null on failure.</returns>
        public static double[] DivideByCount(this Curve curve, int segmentCount, bool includeEnds, out Point3d[] points)
        {
            return ComputeServer.Post<double[], Point3d[]>(ApiAddress(), out points, curve, segmentCount, includeEnds);
        }
        /// <summary>
        /// Divide the curve into specific length segments.
        /// </summary>
        /// <param name="segmentLength">The length of each and every segment (except potentially the last one).</param>
        /// <param name="includeEnds">If true, then the point at the start of the first division segment is returned.</param>
        /// <returns>Array containing division curve parameters if successful, null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_dividebylength.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dividebylength.cs' lang='cs'/>
        /// <code source='examples\py\ex_dividebylength.py' lang='py'/>
        /// </example>
        public static double[] DivideByLength(this Curve curve, double segmentLength, bool includeEnds)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, segmentLength, includeEnds);
        }
        /// <summary>
        /// Divide the curve into specific length segments.
        /// </summary>
        /// <param name="segmentLength">The length of each and every segment (except potentially the last one).</param>
        /// <param name="includeEnds">If true, then the point at the start of the first division segment is returned.</param>
        /// <param name="reverse">If true, then the divisions start from the end of the curve.</param>
        /// <returns>Array containing division curve parameters if successful, null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_dividebylength.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dividebylength.cs' lang='cs'/>
        /// <code source='examples\py\ex_dividebylength.py' lang='py'/>
        /// </example>
        public static double[] DivideByLength(this Curve curve, double segmentLength, bool includeEnds, bool reverse)
        {
            return ComputeServer.Post<double[]>(ApiAddress(), curve, segmentLength, includeEnds, reverse);
        }
        /// <summary>
        /// Divide the curve into specific length segments.
        /// </summary>
        /// <param name="segmentLength">The length of each and every segment (except potentially the last one).</param>
        /// <param name="includeEnds">If true, then the point at the start of the first division segment is returned.</param>
        /// <param name="points">If function is successful, points at each parameter value are returned in points.</param>
        /// <returns>Array containing division curve parameters if successful, null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_dividebylength.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dividebylength.cs' lang='cs'/>
        /// <code source='examples\py\ex_dividebylength.py' lang='py'/>
        /// </example>
        public static double[] DivideByLength(this Curve curve, double segmentLength, bool includeEnds, out Point3d[] points)
        {
            return ComputeServer.Post<double[], Point3d[]>(ApiAddress(), out points, curve, segmentLength, includeEnds);
        }
        /// <summary>
        /// Divide the curve into specific length segments.
        /// </summary>
        /// <param name="segmentLength">The length of each and every segment (except potentially the last one).</param>
        /// <param name="includeEnds">If true, then the point at the start of the first division segment is returned.</param>
        /// <param name="reverse">If true, then the divisions start from the end of the curve.</param>
        /// <param name="points">If function is successful, points at each parameter value are returned in points.</param>
        /// <returns>Array containing division curve parameters if successful, null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_dividebylength.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dividebylength.cs' lang='cs'/>
        /// <code source='examples\py\ex_dividebylength.py' lang='py'/>
        /// </example>
        public static double[] DivideByLength(this Curve curve, double segmentLength, bool includeEnds, bool reverse, out Point3d[] points)
        {
            return ComputeServer.Post<double[], Point3d[]>(ApiAddress(), out points, curve, segmentLength, includeEnds, reverse);
        }
        /// <summary>
        /// Calculates 3d points on a curve where the linear distance between the points is equal.
        /// </summary>
        /// <param name="distance">The distance betwen division points.</param>
        /// <returns>An array of equidistant points, or null on error.</returns>
        public static Point3d[] DivideEquidistant(this Curve curve, double distance)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), curve, distance);
        }
        /// <summary>
        /// Divides this curve at fixed steps along a defined contour line.
        /// </summary>
        /// <param name="contourStart">The start of the contouring line.</param>
        /// <param name="contourEnd">The end of the contouring line.</param>
        /// <param name="interval">A distance to measure on the contouring axis.</param>
        /// <returns>An array of points; or null on error.</returns>
        public static Point3d[] DivideAsContour(this Curve curve, Point3d contourStart, Point3d contourEnd, double interval)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), curve, contourStart, contourEnd, interval);
        }
        /// <summary>
        /// Shortens a curve by a given length
        /// </summary>
        /// <param name="side"></param>
        /// <param name="length"></param>
        /// <returns>Trimmed curve if successful, null on failure.</returns>
        public static Curve Trim(this Curve curve, CurveEnd side, double length)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, length);
        }
        /// <summary>
        /// Splits a curve into pieces using a polysurface.
        /// </summary>
        /// <param name="cutter">A cutting surface or polysurface.</param>
        /// <param name="tolerance">A tolerance for computing intersections.</param>
        /// <returns>An array of curves. This array can be empty.</returns>
        public static Curve[] Split(this Curve curve, Brep cutter, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, cutter, tolerance);
        }
        /// <summary>
        /// Splits a curve into pieces using a polysurface.
        /// </summary>
        /// <param name="cutter">A cutting surface or polysurface.</param>
        /// <param name="tolerance">A tolerance for computing intersections.</param>
        /// <param name="angleToleranceRadians"></param>
        /// <returns>An array of curves. This array can be empty.</returns>
        public static Curve[] Split(this Curve curve, Brep cutter, double tolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, cutter, tolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Splits a curve into pieces using a surface.
        /// </summary>
        /// <param name="cutter">A cutting surface or polysurface.</param>
        /// <param name="tolerance">A tolerance for computing intersections.</param>
        /// <returns>An array of curves. This array can be empty.</returns>
        public static Curve[] Split(this Curve curve, Surface cutter, double tolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, cutter, tolerance);
        }
        /// <summary>
        /// Splits a curve into pieces using a surface.
        /// </summary>
        /// <param name="cutter">A cutting surface or polysurface.</param>
        /// <param name="tolerance">A tolerance for computing intersections.</param>
        /// <param name="angleToleranceRadians"></param>
        /// <returns>An array of curves. This array can be empty.</returns>
        public static Curve[] Split(this Curve curve, Surface cutter, double tolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, cutter, tolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Where possible, analytically extends curve to include the given domain. 
        /// This will not work on closed curves. The original curve will be identical to the 
        /// restriction of the resulting curve to the original curve domain.
        /// </summary>
        /// <param name="t0">Start of extension domain, if the start is not inside the 
        /// Domain of this curve, an attempt will be made to extend the curve.</param>
        /// <param name="t1">End of extension domain, if the end is not inside the 
        /// Domain of this curve, an attempt will be made to extend the curve.</param>
        /// <returns>Extended curve on success, null on failure.</returns>
        public static Curve Extend(this Curve curve, double t0, double t1)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, t0, t1);
        }
        /// <summary>
        /// Where possible, analytically extends curve to include the given domain. 
        /// This will not work on closed curves. The original curve will be identical to the 
        /// restriction of the resulting curve to the original curve domain.
        /// </summary>
        /// <param name="domain">Extension domain.</param>
        /// <returns>Extended curve on success, null on failure.</returns>
        public static Curve Extend(this Curve curve, Interval domain)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, domain);
        }
        /// <summary>
        /// Extends a curve by a specific length.
        /// </summary>
        /// <param name="side">Curve end to extend.</param>
        /// <param name="length">Length to add to the curve end.</param>
        /// <param name="style">Extension style.</param>
        /// <returns>A curve with extended ends or null on failure.</returns>
        public static Curve Extend(this Curve curve, CurveEnd side, double length, CurveExtensionStyle style)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, length, style);
        }
        /// <summary>
        /// Extends a curve until it intersects a collection of objects.
        /// </summary>
        /// <param name="side">The end of the curve to extend.</param>
        /// <param name="style">The style or type of extension to use.</param>
        /// <param name="geometry">A collection of objects. Allowable object types are Curve, Surface, Brep.</param>
        /// <returns>New extended curve result on success, null on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_extendcurve.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_extendcurve.cs' lang='cs'/>
        /// <code source='examples\py\ex_extendcurve.py' lang='py'/>
        /// </example>
        public static Curve Extend(this Curve curve, CurveEnd side, CurveExtensionStyle style, System.Collections.Generic.IEnumerable<GeometryBase> geometry)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, style, geometry);
        }
        /// <summary>
        /// Extends a curve to a point.
        /// </summary>
        /// <param name="side">The end of the curve to extend.</param>
        /// <param name="style">The style or type of extension to use.</param>
        /// <param name="endPoint">A new end point.</param>
        /// <returns>New extended curve result on success, null on failure.</returns>
        public static Curve Extend(this Curve curve, CurveEnd side, CurveExtensionStyle style, Point3d endPoint)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, style, endPoint);
        }
        /// <summary>
        /// Extends a curve on a surface.
        /// </summary>
        /// <param name="side">The end of the curve to extend.</param>
        /// <param name="surface">Surface that contains the curve.</param>
        /// <returns>New extended curve result on success, null on failure.</returns>
        public static Curve ExtendOnSurface(this Curve curve, CurveEnd side, Surface surface)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, surface);
        }
        /// <summary>
        /// Extends a curve on a surface.
        /// </summary>
        /// <param name="side">The end of the curve to extend.</param>
        /// <param name="face">BrepFace that contains the curve.</param>
        /// <returns>New extended curve result on success, null on failure.</returns>
        public static Curve ExtendOnSurface(this Curve curve, CurveEnd side, BrepFace face)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, face);
        }
        /// <summary>
        /// Extends a curve by a line until it intersects a collection of objects.
        /// </summary>
        /// <param name="side">The end of the curve to extend.</param>
        /// <param name="geometry">A collection of objects. Allowable object types are Curve, Surface, Brep.</param>
        /// <returns>New extended curve result on success, null on failure.</returns>
        public static Curve ExtendByLine(this Curve curve, CurveEnd side, System.Collections.Generic.IEnumerable<GeometryBase> geometry)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, geometry);
        }
        /// <summary>
        /// Extends a curve by an Arc until it intersects a collection of objects.
        /// </summary>
        /// <param name="side">The end of the curve to extend.</param>
        /// <param name="geometry">A collection of objects. Allowable object types are Curve, Surface, Brep.</param>
        /// <returns>New extended curve result on success, null on failure.</returns>
        public static Curve ExtendByArc(this Curve curve, CurveEnd side, System.Collections.Generic.IEnumerable<GeometryBase> geometry)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, side, geometry);
        }
        /// <summary>
        /// Returns a geometrically equivalent PolyCurve.
        /// <para>The PolyCurve has the following properties</para>
        /// <para>
        ///	1. All the PolyCurve segments are LineCurve, PolylineCurve, ArcCurve, or NurbsCurve.
        /// </para>
        /// <para>
        ///	2. The Nurbs Curves segments do not have fully multiple interior knots.
        /// </para>
        /// <para>
        ///	3. Rational Nurbs curves do not have constant weights.
        /// </para>
        /// <para>
        ///	4. Any segment for which IsLinear() or IsArc() is true is a Line, 
        ///        Polyline segment, or an Arc.
        /// </para>
        /// <para>
        ///	5. Adjacent Colinear or Cocircular segments are combined.
        /// </para>
        /// <para>
        ///	6. Segments that meet with G1-continuity have there ends tuned up so
        ///        that they meet with G1-continuity to within machine precision.
        /// </para>
        /// </summary>
        /// <param name="options">Simplification options.</param>
        /// <param name="distanceTolerance">A distance tolerance for the simplification.</param>
        /// <param name="angleToleranceRadians">An angle tolerance for the simplification.</param>
        /// <returns>New simplified curve on success, null on failure.</returns>
        public static Curve Simplify(this Curve curve, CurveSimplifyOptions options, double distanceTolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, options, distanceTolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Same as SimplifyCurve, but simplifies only the last two segments at "side" end.
        /// </summary>
        /// <param name="end">If CurveEnd.Start the function simplifies the last two start 
        /// side segments, otherwise if CurveEnd.End the last two end side segments are simplified.
        /// </param>
        /// <param name="options">Simplification options.</param>
        /// <param name="distanceTolerance">A distance tolerance for the simplification.</param>
        /// <param name="angleToleranceRadians">An angle tolerance for the simplification.</param>
        /// <returns>New simplified curve on success, null on failure.</returns>
        public static Curve SimplifyEnd(this Curve curve, CurveEnd end, CurveSimplifyOptions options, double distanceTolerance, double angleToleranceRadians)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, end, options, distanceTolerance, angleToleranceRadians);
        }
        /// <summary>
        /// Fairs a curve object. Fair works best on degree 3 (cubic) curves. Attempts to 
        /// remove large curvature variations while limiting the geometry changes to be no 
        /// more than the specified tolerance. 
        /// </summary>
        /// <param name="distanceTolerance">Maximum allowed distance the faired curve is allowed to deviate from the input.</param>
        /// <param name="angleTolerance">(in radians) kinks with angles &lt;= angleTolerance are smoothed out 0.05 is a good default.</param>
        /// <param name="clampStart">The number of (control vertices-1) to preserve at start. 
        /// <para>0 = preserve start point</para>
        /// <para>1 = preserve start point and 1st derivative</para>
        /// <para>2 = preserve start point, 1st and 2nd derivative</para>
        /// </param>
        /// <param name="clampEnd">Same as clampStart.</param>
        /// <param name="iterations">The number of iteratoins to use in adjusting the curve.</param>
        /// <returns>Returns new faired Curve on success, null on failure.</returns>
        public static Curve Fair(this Curve curve, double distanceTolerance, double angleTolerance, int clampStart, int clampEnd, int iterations)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, distanceTolerance, angleTolerance, clampStart, clampEnd, iterations);
        }
        /// <summary>
        /// Fits a new curve through an existing curve.
        /// </summary>
        /// <param name="degree">The degree of the returned Curve. Must be bigger than 1.</param>
        /// <param name="fitTolerance">The fitting tolerance. If fitTolerance is RhinoMath.UnsetValue or &lt;=0.0,
        /// the document absolute tolerance is used.</param>
        /// <param name="angleTolerance">The kink smoothing tolerance in radians.
        /// <para>If angleTolerance is 0.0, all kinks are smoothed</para>
        /// <para>If angleTolerance is &gt;0.0, kinks smaller than angleTolerance are smoothed</para>  
        /// <para>If angleTolerance is RhinoMath.UnsetValue or &lt;0.0, the document angle tolerance is used for the kink smoothing</para>
        /// </param>
        /// <returns>Returns a new fitted Curve if successful, null on failure.</returns>
        public static Curve Fit(this Curve curve, int degree, double fitTolerance, double angleTolerance)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, degree, fitTolerance, angleTolerance);
        }
        /// <summary>
        /// Rebuild a curve with a specific point count.
        /// </summary>
        /// <param name="pointCount">Number of control points in the rebuild curve.</param>
        /// <param name="degree">Degree of curve. Valid values are between and including 1 and 11.</param>
        /// <param name="preserveTangents">If true, the end tangents of the input curve will be preserved.</param>
        /// <returns>A Nurbs curve on success or null on failure.</returns>
        public static NurbsCurve Rebuild(this Curve curve, int pointCount, int degree, bool preserveTangents)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), curve, pointCount, degree, preserveTangents);
        }
        /// <summary>
        /// Gets a polyline approximation of a curve.
        /// </summary>
        /// <param name="mainSegmentCount">
        /// If mainSegmentCount &lt;= 0, then both subSegmentCount and mainSegmentCount are ignored. 
        /// If mainSegmentCount &gt; 0, then subSegmentCount must be &gt;= 1. In this 
        /// case the nurb will be broken into mainSegmentCount equally spaced 
        /// chords. If needed, each of these chords can be split into as many 
        /// subSegmentCount sub-parts if the subdivision is necessary for the 
        /// mesh to meet the other meshing constraints. In particular, if 
        /// subSegmentCount = 0, then the curve is broken into mainSegmentCount 
        /// pieces and no further testing is performed.</param>
        /// <param name="subSegmentCount">An amount of subsegments.</param>
        /// <param name="maxAngleRadians">
        /// ( 0 to pi ) Maximum angle (in radians) between unit tangents at 
        /// adjacent vertices.</param>
        /// <param name="maxChordLengthRatio">Maximum permitted value of 
        /// (distance chord midpoint to curve) / (length of chord).</param>
        /// <param name="maxAspectRatio">If maxAspectRatio &lt; 1.0, the parameter is ignored. 
        /// If 1 &lt;= maxAspectRatio &lt; sqrt(2), it is treated as if maxAspectRatio = sqrt(2). 
        /// This parameter controls the maximum permitted value of 
        /// (length of longest chord) / (length of shortest chord).</param>
        /// <param name="tolerance">If tolerance = 0, the parameter is ignored. 
        /// This parameter controls the maximum permitted value of the 
        /// distance from the curve to the polyline.</param>
        /// <param name="minEdgeLength">The minimum permitted edge length.</param>
        /// <param name="maxEdgeLength">If maxEdgeLength = 0, the parameter 
        /// is ignored. This parameter controls the maximum permitted edge length.
        /// </param>
        /// <param name="keepStartPoint">If true the starting point of the curve 
        /// is added to the polyline. If false the starting point of the curve is 
        /// not added to the polyline.</param>
        /// <returns>PolylineCurve on success, null on error.</returns>
        public static PolylineCurve ToPolyline(this Curve curve, int mainSegmentCount, int subSegmentCount, double maxAngleRadians, double maxChordLengthRatio, double maxAspectRatio, double tolerance, double minEdgeLength, double maxEdgeLength, bool keepStartPoint)
        {
            return ComputeServer.Post<PolylineCurve>(ApiAddress(), curve, mainSegmentCount, subSegmentCount, maxAngleRadians, maxChordLengthRatio, maxAspectRatio, tolerance, minEdgeLength, maxEdgeLength, keepStartPoint);
        }
        /// <summary>
        /// Gets a polyline approximation of a curve.
        /// </summary>
        /// <param name="mainSegmentCount">
        /// If mainSegmentCount &lt;= 0, then both subSegmentCount and mainSegmentCount are ignored. 
        /// If mainSegmentCount &gt; 0, then subSegmentCount must be &gt;= 1. In this 
        /// case the nurb will be broken into mainSegmentCount equally spaced 
        /// chords. If needed, each of these chords can be split into as many 
        /// subSegmentCount sub-parts if the subdivision is necessary for the 
        /// mesh to meet the other meshing constraints. In particular, if 
        /// subSegmentCount = 0, then the curve is broken into mainSegmentCount 
        /// pieces and no further testing is performed.</param>
        /// <param name="subSegmentCount">An amount of subsegments.</param>
        /// <param name="maxAngleRadians">
        /// ( 0 to pi ) Maximum angle (in radians) between unit tangents at 
        /// adjacent vertices.</param>
        /// <param name="maxChordLengthRatio">Maximum permitted value of 
        /// (distance chord midpoint to curve) / (length of chord).</param>
        /// <param name="maxAspectRatio">If maxAspectRatio &lt; 1.0, the parameter is ignored. 
        /// If 1 &lt;= maxAspectRatio &lt; sqrt(2), it is treated as if maxAspectRatio = sqrt(2). 
        /// This parameter controls the maximum permitted value of 
        /// (length of longest chord) / (length of shortest chord).</param>
        /// <param name="tolerance">If tolerance = 0, the parameter is ignored. 
        /// This parameter controls the maximum permitted value of the 
        /// distance from the curve to the polyline.</param>
        /// <param name="minEdgeLength">The minimum permitted edge length.</param>
        /// <param name="maxEdgeLength">If maxEdgeLength = 0, the parameter 
        /// is ignored. This parameter controls the maximum permitted edge length.
        /// </param>
        /// <param name="keepStartPoint">If true the starting point of the curve 
        /// is added to the polyline. If false the starting point of the curve is 
        /// not added to the polyline.</param>
        /// <param name="curveDomain">This subdomain of the NURBS curve is approximated.</param>
        /// <returns>PolylineCurve on success, null on error.</returns>
        public static PolylineCurve ToPolyline(this Curve curve, int mainSegmentCount, int subSegmentCount, double maxAngleRadians, double maxChordLengthRatio, double maxAspectRatio, double tolerance, double minEdgeLength, double maxEdgeLength, bool keepStartPoint, Interval curveDomain)
        {
            return ComputeServer.Post<PolylineCurve>(ApiAddress(), curve, mainSegmentCount, subSegmentCount, maxAngleRadians, maxChordLengthRatio, maxAspectRatio, tolerance, minEdgeLength, maxEdgeLength, keepStartPoint, curveDomain);
        }
        /// <summary>
        /// Gets a polyline approximation of a curve.
        /// </summary>
        /// <param name="tolerance">The tolerance. This is the maximum deviation from line midpoints to the curve. When in doubt, use the document's model space absolute tolerance.</param>
        /// <param name="angleTolerance">The angle tolerance in radians. This is the maximum deviation of the line directions. When in doubt, use the document's model space angle tolerance.</param>
        /// <param name="minimumLength">The minimum segment length.</param>
        /// <param name="maximumLength">The maximum segment length.</param>
        /// <returns>PolyCurve on success, null on error.</returns>
        public static PolylineCurve ToPolyline(this Curve curve, double tolerance, double angleTolerance, double minimumLength, double maximumLength)
        {
            return ComputeServer.Post<PolylineCurve>(ApiAddress(), curve, tolerance, angleTolerance, minimumLength, maximumLength);
        }
        /// <summary>
        /// Converts a curve into polycurve consisting of arc segments. Sections of the input curves that are nearly straight are converted to straight-line segments.
        /// </summary>
        /// <param name="tolerance">The tolerance. This is the maximum deviation from arc midpoints to the curve. When in doubt, use the document's model space absolute tolerance.</param>
        /// <param name="angleTolerance">The angle tolerance in radians. This is the maximum deviation of the arc end directions from the curve direction. When in doubt, use the document's model space angle tolerance.</param>
        /// <param name="minimumLength">The minimum segment length.</param>
        /// <param name="maximumLength">The maximum segment length.</param>
        /// <returns>PolyCurve on success, null on error.</returns>
        public static PolyCurve ToArcsAndLines(this Curve curve, double tolerance, double angleTolerance, double minimumLength, double maximumLength)
        {
            return ComputeServer.Post<PolyCurve>(ApiAddress(), curve, tolerance, angleTolerance, minimumLength, maximumLength);
        }
        /// <summary>
        /// Makes a polyline approximation of the curve and gets the closest point on the mesh for each point on the curve. 
        /// Then it "connects the points" so that you have a polyline on the mesh.
        /// </summary>
        /// <param name="mesh">Mesh to project onto.</param>
        /// <param name="tolerance">Input tolerance (RhinoDoc.ModelAbsoluteTolerance is a good default)</param>
        /// <returns>A polyline curve on success, null on failure.</returns>
        public static PolylineCurve PullToMesh(this Curve curve, Mesh mesh, double tolerance)
        {
            return ComputeServer.Post<PolylineCurve>(ApiAddress(), curve, mesh, tolerance);
        }
        /// <summary>
        /// Offsets this curve. If you have a nice offset, then there will be one entry in 
        /// the array. If the original curve had kinks or the offset curve had self 
        /// intersections, you will get multiple segments in the offset_curves[] array.
        /// </summary>
        /// <param name="plane">Offset solution plane.</param>
        /// <param name="distance">The positive or negative distance to offset.</param>
        /// <param name="tolerance">The offset or fitting tolerance.</param>
        /// <param name="cornerStyle">Corner style for offset kinks.</param>
        /// <returns>Offset curves on success, null on failure.</returns>
        public static Curve[] Offset(this Curve curve, Plane plane, double distance, double tolerance, CurveOffsetCornerStyle cornerStyle)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, plane, distance, tolerance, cornerStyle);
        }
        /// <summary>
        /// Offsets this curve. If you have a nice offset, then there will be one entry in 
        /// the array. If the original curve had kinks or the offset curve had self 
        /// intersections, you will get multiple segments in the offset_curves[] array.
        /// </summary>
        /// <param name="directionPoint">A point that indicates the direction of the offset.</param>
        /// <param name="normal">The normal to the offset plane.</param>
        /// <param name="distance">The positive or negative distance to offset.</param>
        /// <param name="tolerance">The offset or fitting tolerance.</param>
        /// <param name="cornerStyle">Corner style for offset kinks.</param>
        /// <returns>Offset curves on success, null on failure.</returns>
        public static Curve[] Offset(this Curve curve, Point3d directionPoint, Vector3d normal, double distance, double tolerance, CurveOffsetCornerStyle cornerStyle)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, directionPoint, normal, distance, tolerance, cornerStyle);
        }
        /// <summary>
        /// Offsets a closed curve in the following way: pProject the curve to a plane with given normal.
        /// Then, loose Offset the projection by distance + blend_radius and trim off self-intersection.
        /// THen, Offset the remaining curve back in the opposite direction by blend_radius, filling gaps with blends.
        /// Finally, use the elevations of the input curve to get the correct elevations of the result.
        /// </summary>
        /// <param name="distance">The positive distance to offset the curve.</param>
        /// <param name="blendRadius">
        /// Positive, typically the same as distance. When the offset results in a self-intersection
        /// that gets trimmed off at a kink, the kink will be blended out using this radius.
        /// </param>
        /// <param name="directionPoint">
        /// A point that indicates the direction of the offset. If the offset is inward,
        /// the point's projection to the plane should be well within the curve.
        /// It will be used to decide which part of the offset to keep if there are self-intersections.
        /// </param>
        /// <param name="normal">A vector that indicates the normal of the plane in which the offset will occur.</param>
        /// <param name="tolerance">Used to determine self-intersections, not offset error.</param>
        /// <returns>The offset curve if successful.</returns>
        public static Curve RibbonOffset(this Curve curve, out Curve updatedInstance, double distance, double blendRadius, Point3d directionPoint, Vector3d normal, double tolerance)
        {
            return ComputeServer.Post<Curve, Curve>(ApiAddress(), out updatedInstance, curve, distance, blendRadius, directionPoint, normal, tolerance);
        }
        /// <summary>
        /// Offset this curve on a brep face surface. This curve must lie on the surface.
        /// </summary>
        /// <param name="face">The brep face on which to offset.</param>
        /// <param name="distance">A distance to offset (+)left, (-)right.</param>
        /// <param name="fittingTolerance">A fitting tolerance.</param>
        /// <returns>Offset curves on success, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">If face is null.</exception>
        public static Curve[] OffsetOnSurface(this Curve curve, BrepFace face, double distance, double fittingTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, face, distance, fittingTolerance);
        }
        /// <summary>
        /// Offset a curve on a brep face surface. This curve must lie on the surface.
        /// <para>This overload allows to specify a surface point at which the offset will pass.</para>
        /// </summary>
        /// <param name="face">The brep face on which to offset.</param>
        /// <param name="throughPoint">2d point on the brep face to offset through.</param>
        /// <param name="fittingTolerance">A fitting tolerance.</param>
        /// <returns>Offset curves on success, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">If face is null.</exception>
        public static Curve[] OffsetOnSurface(this Curve curve, BrepFace face, Point2d throughPoint, double fittingTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, face, throughPoint, fittingTolerance);
        }
        /// <summary>
        /// Offset a curve on a brep face surface. This curve must lie on the surface.
        /// <para>This overload allows to specify different offsets for different curve parameters.</para>
        /// </summary>
        /// <param name="face">The brep face on which to offset.</param>
        /// <param name="curveParameters">Curve parameters corresponding to the offset distances.</param>
        /// <param name="offsetDistances">distances to offset (+)left, (-)right.</param>
        /// <param name="fittingTolerance">A fitting tolerance.</param>
        /// <returns>Offset curves on success, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">If face, curveParameters or offsetDistances are null.</exception>
        public static Curve[] OffsetOnSurface(this Curve curve, BrepFace face, double[] curveParameters, double[] offsetDistances, double fittingTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, face, curveParameters, offsetDistances, fittingTolerance);
        }
        /// <summary>
        /// Offset a curve on a surface. This curve must lie on the surface.
        /// </summary>
        /// <param name="surface">A surface on which to offset.</param>
        /// <param name="distance">A distance to offset (+)left, (-)right.</param>
        /// <param name="fittingTolerance">A fitting tolerance.</param>
        /// <returns>Offset curves on success, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">If surface is null.</exception>
        public static Curve[] OffsetOnSurface(this Curve curve, Surface surface, double distance, double fittingTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, surface, distance, fittingTolerance);
        }
        /// <summary>
        /// Offset a curve on a surface. This curve must lie on the surface.
        /// <para>This overload allows to specify a surface point at which the offset will pass.</para>
        /// </summary>
        /// <param name="surface">A surface on which to offset.</param>
        /// <param name="throughPoint">2d point on the brep face to offset through.</param>
        /// <param name="fittingTolerance">A fitting tolerance.</param>
        /// <returns>Offset curves on success, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">If surface is null.</exception>
        public static Curve[] OffsetOnSurface(this Curve curve, Surface surface, Point2d throughPoint, double fittingTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, surface, throughPoint, fittingTolerance);
        }
        /// <summary>
        /// Offset this curve on a surface. This curve must lie on the surface.
        /// <para>This overload allows to specify different offsets for different curve parameters.</para>
        /// </summary>
        /// <param name="surface">A surface on which to offset.</param>
        /// <param name="curveParameters">Curve parameters corresponding to the offset distances.</param>
        /// <param name="offsetDistances">Distances to offset (+)left, (-)right.</param>
        /// <param name="fittingTolerance">A fitting tolerance.</param>
        /// <returns>Offset curves on success, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">If surface, curveParameters or offsetDistances are null.</exception>
        public static Curve[] OffsetOnSurface(this Curve curve, Surface surface, double[] curveParameters, double[] offsetDistances, double fittingTolerance)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), curve, surface, curveParameters, offsetDistances, fittingTolerance);
        }

        /// <summary>
        /// Finds a curve by offsetting an existing curve normal to a surface.
        /// The caller is responsible for ensuring that the curve lies on the input surface.
        /// </summary>
        /// <param name="surface">Surface from which normals are calculated.</param>
        /// <param name="height">offset distance (distance from surface to result curve)</param>
        /// <returns>
        /// Offset curve at distance height from the surface.  The offset curve is
        /// interpolated through a small number of points so if the surface is irregular
        /// or complicated, the result will not be a very accurate offset.
        /// </returns>
        public static Curve OffsetNormalToSurface(this Curve curve, Surface surface, double height)
        {
            return ComputeServer.Post<Curve>(ApiAddress(), curve, surface, height);
        }
    }

    public class AreaMassProperties
    {
        public double Area { get; set; }
        public double AreaError { get; set; }
        public Point3d Centroid { get; set; }
        public Vector3d CentroidError { get; set; }
        public Vector3d WorldCoordinatesFirstMoments { get; set; }
        public Vector3d WorldCoordinatesFirstMomentsError { get; set; }
        public Vector3d WorldCoordinatesSecondMoments { get; set; }
        public Vector3d WorldCoordinatesSecondMomentsError { get; set; }
        public Vector3d WorldCoordinatesProductMoments { get; set; }
        public Vector3d WorldCoordinatesProductMomentsError { get; set; }
        public Vector3d WorldCoordinatesMomentsOfInertia { get; set; }
        public Vector3d WorldCoordinatesMomentsOfInertiaError { get; set; }
        public Vector3d WorldCoordinatesRadiiOfGyration { get; set; }
        public Vector3d CentroidCoordinatesSecondMoments { get; set; }
        public Vector3d CentroidCoordinatesSecondMomentsError { get; set; }
        public Vector3d CentroidCoordinatesMomentsOfInertia { get; set; }
        public Vector3d CentroidCoordinatesMomentsOfInertiaError { get; set; }
        public Vector3d CentroidCoordinatesRadiiOfGyration { get; set; }
    }


    public static class AreaMassPropertiesCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return "rhino/geometry/areamassproperties/" + caller;
        }

        /// <summary>
        /// Computes an AreaMassProperties for a closed planar curve.
        /// </summary>
        /// <param name="closedPlanarCurve">Curve to measure.</param>
        /// <returns>The AreaMassProperties for the given curve or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When closedPlanarCurve is null.</exception>
        public static AreaMassProperties Compute(Curve closedPlanarCurve)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-curve"), closedPlanarCurve);
        }
        /// <summary>
        /// Computes an AreaMassProperties for a closed planar curve.
        /// </summary>
        /// <param name="closedPlanarCurve">Curve to measure.</param>
        /// <param name="planarTolerance">absolute tolerance used to insure the closed curve is planar</param>
        /// <returns>The AreaMassProperties for the given curve or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When closedPlanarCurve is null.</exception>
        public static AreaMassProperties Compute(Curve closedPlanarCurve, double planarTolerance)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-curve_double"), closedPlanarCurve, planarTolerance);
        }
        /// <summary>
        /// Computes an AreaMassProperties for a hatch.
        /// </summary>
        /// <param name="hatch">Hatch to measure.</param>
        /// <returns>The AreaMassProperties for the given hatch or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When hatch is null.</exception>
        public static AreaMassProperties Compute(Hatch hatch)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-hatch"), hatch);
        }
        /// <summary>
        /// Computes an AreaMassProperties for a mesh.
        /// </summary>
        /// <param name="mesh">Mesh to measure.</param>
        /// <returns>The AreaMassProperties for the given Mesh or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When mesh is null.</exception>
        public static AreaMassProperties Compute(Mesh mesh)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-mesh"), mesh);
        }
        /// <summary>
        /// Compute the AreaMassProperties for a single Mesh.
        /// </summary>
        /// <param name="mesh">Mesh to measure.</param>
        /// <param name="area">true to calculate area.</param>
        /// <param name="firstMoments">true to calculate area first moments, area, and area centroid.</param>
        /// <param name="secondMoments">true to calculate area second moments.</param>
        /// <param name="productMoments">true to calculate area product moments.</param>
        /// <returns>The AreaMassProperties for the given Mesh or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When mesh is null.</exception>
        public static AreaMassProperties Compute(Mesh mesh, bool area, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress(
                "compute-mesh_bool_bool_bool_bool"),
                mesh, area, firstMoments, secondMoments, productMoments);
        }
        /// <summary>
        /// Computes an AreaMassProperties for a brep.
        /// </summary>
        /// <param name="brep">Brep to measure.</param>
        /// <returns>The AreaMassProperties for the given Brep or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When brep is null.</exception>
        public static AreaMassProperties Compute(Brep brep)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-brep"), brep);
        }
        /// <summary>
        /// Compute the AreaMassProperties for a single Brep.
        /// </summary>
        /// <param name="brep">Brep to measure.</param>
        /// <param name="area">true to calculate area.</param>
        /// <param name="firstMoments">true to calculate area first moments, area, and area centroid.</param>
        /// <param name="secondMoments">true to calculate area second moments.</param>
        /// <param name="productMoments">true to calculate area product moments.</param>
        /// <returns>The AreaMassProperties for the given Brep or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When brep is null.</exception>
        public static AreaMassProperties Compute(Brep brep, bool area, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress(
                "compute-brep_bool_bool_bool_bool"
                ), brep, area, firstMoments, secondMoments, productMoments);
        }
        /// <summary>
        /// Computes an AreaMassProperties for a surface.
        /// </summary>
        /// <param name="surface">Surface to measure.</param>
        /// <returns>The AreaMassProperties for the given Surface or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When surface is null.</exception>
        public static AreaMassProperties Compute(Surface surface)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-surface"), surface);
        }
        /// <summary>
        /// Compute the AreaMassProperties for a single Surface.
        /// </summary>
        /// <param name="surface">Surface to measure.</param>
        /// <param name="area">true to calculate area.</param>
        /// <param name="firstMoments">true to calculate area first moments, area, and area centroid.</param>
        /// <param name="secondMoments">true to calculate area second moments.</param>
        /// <param name="productMoments">true to calculate area product moments.</param>
        /// <returns>The AreaMassProperties for the given Surface or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When surface is null.</exception>
        public static AreaMassProperties Compute(Surface surface, bool area, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress(
                "compute-surface_bool_bool_bool_bool"
                ), surface, area, firstMoments, secondMoments, productMoments);
        }
        /// <summary>
        /// Computes the Area properties for a collection of geometric objects. 
        /// At present only Breps, Surfaces, Meshes and Planar Closed Curves are supported.
        /// </summary>
        /// <param name="geometry">Objects to include in the area computation.</param>
        /// <returns>The Area properties for the entire collection or null on failure.</returns>
        public static AreaMassProperties Compute(IEnumerable<GeometryBase> geometry)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress("compute-geometrybasearray"), geometry);
        }
        /// <summary>
        /// Computes the AreaMassProperties for a collection of geometric objects. 
        /// At present only Breps, Surfaces, Meshes and Planar Closed Curves are supported.
        /// </summary>
        /// <param name="geometry">Objects to include in the area computation.</param>
        /// <param name="area">true to calculate area.</param>
        /// <param name="firstMoments">true to calculate area first moments, area, and area centroid.</param>
        /// <param name="secondMoments">true to calculate area second moments.</param>
        /// <param name="productMoments">true to calculate area product moments.</param>
        /// <returns>The AreaMassProperties for the entire collection or null on failure.</returns>
        public static AreaMassProperties Compute(IEnumerable<GeometryBase> geometry, bool area, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<AreaMassProperties>(ApiAddress(
                "compute-geometrybasearray_bool_bool_bool_bool"
                ), geometry, area, firstMoments, secondMoments, productMoments);
        }
    }

    public class VolumeMassProperties
    {
        public double Volume { get; set; }
        public double VolumeError { get; set; }
        public Point3d Centroid { get; set; }
        public Vector3d CentroidError { get; set; }
        public Vector3d WorldCoordinatesFirstMoments { get; set; }
        public Vector3d WorldCoordinatesFirstMomentsError { get; set; }
        public Vector3d WorldCoordinatesSecondMoments { get; set; }
        public Vector3d WorldCoordinatesSecondMomentsError { get; set; }
        public Vector3d WorldCoordinatesProductMoments { get; set; }
        public Vector3d WorldCoordinatesProductMomentsError { get; set; }
        public Vector3d WorldCoordinatesMomentsOfInertia { get; set; }
        public Vector3d WorldCoordinatesMomentsOfInertiaError { get; set; }
        public Vector3d WorldCoordinatesRadiiOfGyration { get; set; }
        public Vector3d CentroidCoordinatesSecondMoments { get; set; }
        public Vector3d CentroidCoordinatesSecondMomentsError { get; set; }
        public Vector3d CentroidCoordinatesMomentsOfInertia { get; set; }
        public Vector3d CentroidCoordinatesMomentsOfInertiaError { get; set; }
        public Vector3d CentroidCoordinatesRadiiOfGyration { get; set; }

    }

    public static class VolumeMassPropertiesCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return "rhino/geometry/volumemassproperties/" + caller;
        }

        /// <summary>
        /// Compute the VolumeMassProperties for a single Mesh.
        /// </summary>
        /// <param name="mesh">Mesh to measure.</param>
        /// <returns>The VolumeMassProperties for the given Mesh or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When mesh is null.</exception>
        public static VolumeMassProperties Compute(Mesh mesh)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress("compute-mesh"), mesh);
        }
        /// <summary>
        /// Compute the VolumeMassProperties for a single Mesh.
        /// </summary>
        /// <param name="mesh">Mesh to measure.</param>
        /// <param name="volume">true to calculate volume.</param>
        /// <param name="firstMoments">true to calculate volume first moments, volume, and volume centroid.</param>
        /// <param name="secondMoments">true to calculate volume second moments.</param>
        /// <param name="productMoments">true to calculate volume product moments.</param>
        /// <returns>The VolumeMassProperties for the given Mesh or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When Mesh is null.</exception>
        public static VolumeMassProperties Compute(Mesh mesh, bool volume, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress(
                "compute-mesh_bool_bool_bool_bool"
                ), mesh, volume, firstMoments, secondMoments, productMoments);
        }
        /// <summary>
        /// Compute the VolumeMassProperties for a single Brep.
        /// </summary>
        /// <param name="brep">Brep to measure.</param>
        /// <returns>The VolumeMassProperties for the given Brep or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When brep is null.</exception>
        public static VolumeMassProperties Compute(Brep brep)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress("compute-brep"), brep);
        }
        /// <summary>
        /// Compute the VolumeMassProperties for a single Brep.
        /// </summary>
        /// <param name="brep">Brep to measure.</param>
        /// <param name="volume">true to calculate volume.</param>
        /// <param name="firstMoments">true to calculate volume first moments, volume, and volume centroid.</param>
        /// <param name="secondMoments">true to calculate volume second moments.</param>
        /// <param name="productMoments">true to calculate volume product moments.</param>
        /// <returns>The VolumeMassProperties for the given Brep or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When brep is null.</exception>
        public static VolumeMassProperties Compute(Brep brep, bool volume, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress(
                "compute-brep_bool_bool_bool_bool"
                ), brep, volume, firstMoments, secondMoments, productMoments);
        }
        /// <summary>
        /// Compute the VolumeMassProperties for a single Surface.
        /// </summary>
        /// <param name="surface">Surface to measure.</param>
        /// <returns>The VolumeMassProperties for the given Surface or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When surface is null.</exception>
        public static VolumeMassProperties Compute(Surface surface)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress("compute-surface"), surface);
        }
        /// <summary>
        /// Compute the VolumeMassProperties for a single Surface.
        /// </summary>
        /// <param name="surface">Surface to measure.</param>
        /// <param name="volume">true to calculate volume.</param>
        /// <param name="firstMoments">true to calculate volume first moments, volume, and volume centroid.</param>
        /// <param name="secondMoments">true to calculate volume second moments.</param>
        /// <param name="productMoments">true to calculate volume product moments.</param>
        /// <returns>The VolumeMassProperties for the given Surface or null on failure.</returns>
        /// <exception cref="System.ArgumentNullException">When surface is null.</exception>
        public static VolumeMassProperties Compute(Surface surface, bool volume, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress(
                "compute-surface_bool_bool_bool_bool"
                ), surface, volume, firstMoments, secondMoments, productMoments);
        }
        /// <summary>
        /// Computes the VolumeMassProperties for a collection of geometric objects. 
        /// At present only Breps, Surfaces, and Meshes are supported.
        /// </summary>
        /// <param name="geometry">Objects to include in the area computation.</param>
        /// <returns>The VolumeMassProperties for the entire collection or null on failure.</returns>
        public static VolumeMassProperties Compute(IEnumerable<GeometryBase> geometry)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress(
                "compute-geometrybasearray"
                ), geometry);
        }
        /// <summary>
        /// Computes the VolumeMassProperties for a collection of geometric objects. 
        /// At present only Breps, Surfaces, Meshes and Planar Closed Curves are supported.
        /// </summary>
        /// <param name="geometry">Objects to include in the area computation.</param>
        /// <param name="volume">true to calculate volume.</param>
        /// <param name="firstMoments">true to calculate volume first moments, volume, and volume centroid.</param>
        /// <param name="secondMoments">true to calculate volume second moments.</param>
        /// <param name="productMoments">true to calculate volume product moments.</param>
        /// <returns>The VolumeMassProperties for the entire collection or null on failure.</returns>
        public static VolumeMassProperties Compute(IEnumerable<GeometryBase> geometry, bool volume, bool firstMoments, bool secondMoments, bool productMoments)
        {
            return ComputeServer.Post<VolumeMassProperties>(ApiAddress(
                "compute-geometrybasearray_bool_bool_bool_bool"
                ), geometry, volume, firstMoments, secondMoments, productMoments);
        }
    }

    public static class MeshCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(Mesh), caller);
        }
        /// <summary>
        /// Constructs a planar mesh grid.
        /// </summary>
        /// <param name="plane">Plane of mesh.</param>
        /// <param name="xInterval">Interval describing size and extends of mesh along plane x-direction.</param>
        /// <param name="yInterval">Interval describing size and extends of mesh along plane y-direction.</param>
        /// <param name="xCount">Number of faces in x-direction.</param>
        /// <param name="yCount">Number of faces in y-direction.</param>
        /// <exception cref="ArgumentException">Thrown when plane is a null reference.</exception>
        /// <exception cref="ArgumentException">Thrown when xInterval is a null reference.</exception>
        /// <exception cref="ArgumentException">Thrown when yInterval is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when xCount is less than or equal to zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when yCount is less than or equal to zero.</exception>
        public static Mesh CreateFromPlane(Plane plane, Interval xInterval, Interval yInterval, int xCount, int yCount)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), plane, xInterval, yInterval, xCount, yCount);
        }
        /// <summary>
        /// Constructs new mesh that matches a bounding box.
        /// </summary>
        /// <param name="box">A box to use for creation.</param>
        /// <param name="xCount">Number of faces in x-direction.</param>
        /// <param name="yCount">Number of faces in y-direction.</param>
        /// <param name="zCount">Number of faces in z-direction.</param>
        /// <returns>A new brep, or null on failure.</returns>
        public static Mesh CreateFromBox(BoundingBox box, int xCount, int yCount, int zCount)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), box, xCount, yCount, zCount);
        }
        /// <summary>
        ///  Constructs new mesh that matches an aligned box.
        /// </summary>
        /// <param name="box">Box to match.</param>
        /// <param name="xCount">Number of faces in x-direction.</param>
        /// <param name="yCount">Number of faces in y-direction.</param>
        /// <param name="zCount">Number of faces in z-direction.</param>
        /// <returns></returns>
        public static Mesh CreateFromBox(Box box, int xCount, int yCount, int zCount)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), box, xCount, yCount, zCount);
        }
        /// <summary>
        /// Constructs new mesh from 8 corner points.
        /// </summary>
        /// <param name="corners">
        /// 8 points defining the box corners arranged as the vN labels indicate.
        /// <pre>
        /// <para>v7_____________v6</para>
        /// <para>|\             |\</para>
        /// <para>| \            | \</para>
        /// <para>|  \ _____________\</para>
        /// <para>|   v4         |   v5</para>
        /// <para>|   |          |   |</para>
        /// <para>|   |          |   |</para>
        /// <para>v3--|----------v2  |</para>
        /// <para> \  |           \  |</para>
        /// <para>  \ |            \ |</para>
        /// <para>   \|             \|</para>
        /// <para>    v0_____________v1</para>
        /// </pre>
        /// </param>
        /// <param name="xCount">Number of faces in x-direction.</param>
        /// <param name="yCount">Number of faces in y-direction.</param>
        /// <param name="zCount">Number of faces in z-direction.</param>
        /// <returns>A new brep, or null on failure.</returns>
        /// <returns>A new box mesh, on null on error.</returns>
        public static Mesh CreateFromBox(IEnumerable<Point3d> corners, int xCount, int yCount, int zCount)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), corners, xCount, yCount, zCount);
        }
        /// <summary>
        /// Constructs a mesh sphere.
        /// </summary>
        /// <param name="sphere">Base sphere for mesh.</param>
        /// <param name="xCount">Number of faces in the around direction.</param>
        /// <param name="yCount">Number of faces in the top-to-bottom direction.</param>
        /// <exception cref="ArgumentException">Thrown when sphere is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when xCount is less than or equal to two.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when yCount is less than or equal to two.</exception>
        /// <returns></returns>
        public static Mesh CreateFromSphere(Sphere sphere, int xCount, int yCount)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), sphere, xCount, yCount);
        }
        /// <summary>
        /// Constructs a icospherical mesh. A mesh icosphere differs from a standard
        /// UV mesh sphere in that it's vertices are evenly distributed. A mesh icosphere
        /// starts from an icosahedron (a regular polyhedron with 20 equilateral triangles).
        /// It is then refined by splitting each triangle into 4 smaller triangles.
        /// This splitting can be done several times.
        /// </summary>
        /// <param name="sphere">The input sphere provides the orienting plane and radius.</param>
        /// <param name="subdivisions">
        /// The number of times you want the faces split, where 0  &lt;= subdivisions &lt;= 7. 
        /// Note, the total number of mesh faces produces is: 20 * (4 ^ subdivisions)
        /// </param>
        /// <returns>A welded mesh icosphere if successful, or null on failure.</returns>
        public static Mesh CreateIcoSphere(Sphere sphere, int subdivisions)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), sphere, subdivisions);
        }
        /// <summary>
        /// Constructs a quad mesh sphere. A quad mesh sphere differs from a standard
        /// UV mesh sphere in that it's vertices are evenly distributed. A quad mesh sphere
        /// starts from a cube (a regular polyhedron with 6 square sides).
        /// It is then refined by splitting each quad into 4 smaller quads.
        /// This splitting can be done several times.
        /// </summary>
        /// <param name="sphere">The input sphere provides the orienting plane and radius.</param>
        /// <param name="subdivisions">
        /// The number of times you want the faces split, where 0  &lt;= subdivisions &lt;= 8. 
        /// Note, the total number of mesh faces produces is: 6 * (4 ^ subdivisions)
        /// </param>
        /// <returns>A welded quad mesh sphere if successful, or null on failure.</returns>
        public static Mesh CreateQuadSphere(Sphere sphere, int subdivisions)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), sphere, subdivisions);
        }
        /// <summary>Constructs a mesh cylinder</summary>
        /// <param name="cylinder"></param>
        /// <param name="vertical">Number of faces in the top-to-bottom direction</param>
        /// <param name="around">Number of faces around the cylinder</param>
        /// <exception cref="ArgumentException">Thrown when cylinder is invalid.</exception>
        /// <returns></returns>
        public static Mesh CreateFromCylinder(Cylinder cylinder, int vertical, int around)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), cylinder, vertical, around);
        }
        /// <summary>Constructs a solid mesh cone.</summary>
        /// <param name="cone"></param>
        /// <param name="vertical">Number of faces in the top-to-bottom direction.</param>
        /// <param name="around">Number of faces around the cone.</param>
        /// <exception cref="ArgumentException">Thrown when cone is invalid.</exception>
        /// <returns>A valid mesh if successful.</returns>
        public static Mesh CreateFromCone(Cone cone, int vertical, int around)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), cone, vertical, around);
        }
        /// <summary>Constructs a mesh cone.</summary>
        /// <param name="cone"></param>
        /// <param name="vertical">Number of faces in the top-to-bottom direction.</param>
        /// <param name="around">Number of faces around the cone.</param>
        /// <param name="solid">If false the mesh will be open with no faces on the circular planar portion.</param>
        /// <exception cref="ArgumentException">Thrown when cone is invalid.</exception>
        /// <returns>A valid mesh if successful.</returns>
        public static Mesh CreateFromCone(Cone cone, int vertical, int around, bool solid)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), cone, vertical, around, solid);
        }
        /// <summary>
        /// Do not use this overload. Use version that takes a tolerance parameter instead.
        /// </summary>
        /// <param name="boundary">Do not use.</param>
        /// <param name="parameters">Do not use.</param>
        /// <returns>
        /// Do not use.
        /// </returns>
        public static Mesh CreateFromPlanarBoundary(Curve boundary, MeshingParameters parameters)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), boundary, parameters);
        }
        /// <summary>
        /// Attempts to construct a mesh from a closed planar curve.RhinoMakePlanarMeshes
        /// </summary>
        /// <param name="boundary">must be a closed planar curve.</param>
        /// <param name="parameters">parameters used for creating the mesh.</param>
        /// <param name="tolerance">Tolerance to use during operation.</param>
        /// <returns>
        /// New mesh on success or null on failure.
        /// </returns>
        public static Mesh CreateFromPlanarBoundary(Curve boundary, MeshingParameters parameters, double tolerance)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), boundary, parameters, tolerance);
        }
        /// <summary>
        /// Attempts to create a Mesh that is a triangulation of a closed polyline.
        /// </summary>
        /// <param name="polyline">must be closed</param>
        /// <returns>
        /// New mesh on success or null on failure.
        /// </returns>
        public static Mesh CreateFromClosedPolyline(Polyline polyline)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), polyline);
        }
        /// <summary>
        /// Attempts to create a mesh that is a triangulation of a list of points, projected on a plane,
        /// including its holes and fixed edges.
        /// </summary>
        /// <param name="points">A list, an array or any enumerable of points.</param>
        /// <param name="plane">A plane.</param>
        /// <param name="allowNewVertices">If true, the mesh might have more vertices than the list of input points,
        /// if doing so will improve long thin triangles.</param>
        /// <param name="edges">A list of polylines, or other lists of points representing edges.
        /// This can be null. If nested enumerable items are null, they will be discarded.</param>
        /// <returns>A new mesh, or null if not successful.</returns>
        /// <exception cref="ArgumentNullException">If points is null.</exception>
        /// <exception cref="ArgumentException">If plane is not valid.</exception>
        public static Mesh CreateFromTessellation(IEnumerable<Point3d> points, IEnumerable<IEnumerable<Point3d>> edges, Plane plane, bool allowNewVertices)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), points, edges, plane, allowNewVertices);
        }
        /// <summary>
        /// Constructs a mesh from a brep.
        /// </summary>
        /// <param name="brep">Brep to approximate.</param>
        /// <returns>An array of meshes.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_tightboundingbox.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_tightboundingbox.cs' lang='cs'/>
        /// <code source='examples\py\ex_tightboundingbox.py' lang='py'/>
        /// </example>
        public static Mesh[] CreateFromBrep(Brep brep)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), brep);
        }
        /// <summary>
        /// Constructs a mesh from a brep.
        /// </summary>
        /// <param name="brep">Brep to approximate.</param>
        /// <param name="meshingParameters">Parameters to use during meshing.</param>
        /// <returns>An array of meshes.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_createmeshfrombrep.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_createmeshfrombrep.cs' lang='cs'/>
        /// <code source='examples\py\ex_createmeshfrombrep.py' lang='py'/>
        /// </example>
        public static Mesh[] CreateFromBrep(Brep brep, MeshingParameters meshingParameters)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), brep, meshingParameters);
        }
        /// <summary>
        /// Constructs a mesh from a surface
        /// </summary>
        /// <param name="surface">Surface to approximate</param>
        /// <returns>New mesh representing the surface</returns>
        public static Mesh CreateFromSurface(Surface surface)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), surface);
        }
        /// <summary>
        /// Constructs a mesh from a surface
        /// </summary>
        /// <param name="surface">Surface to approximate</param>
        /// <param name="meshingParameters">settings used to create the mesh</param>
        /// <returns>New mesh representing the surface</returns>
        public static Mesh CreateFromSurface(Surface surface, MeshingParameters meshingParameters)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), surface, meshingParameters);
        }
        /// <summary>
        /// Construct a mesh patch from a variety of input geometry.
        /// </summary>
        /// <param name="outerBoundary">(optional: can be null) Outer boundary
        /// polyline, if provided this will become the outer boundary of the
        /// resulting mesh. Any of the input that is completely outside the outer
        /// boundary will be ignored and have no impact on the result. If any of
        /// the input intersects the outer boundary the result will be
        /// unpredictable and is likely to not include the entire outer boundary.
        /// </param>
        /// <param name="angleToleranceRadians">
        /// Maximum angle between unit tangents and adjacent verticies. Used to
        /// divide curve inputs that cannot otherwise be represented as a polyline.
        /// </param>
        /// <param name="innerBoundaryCurves">
        /// (optional: can be null) Polylines to create holes in the output mesh.
        /// If innerBoundaryCurves are the only input then the result may be null
        /// if trimback is set to false (see comments for trimback) because the
        /// resulting mesh could be invalid (all faces created contained vertexes
        /// from the perimeter boundary).
        /// </param>
        /// <param name="pullbackSurface">
        /// (optional: can be null) Initial surface where 3d input will be pulled
        /// to make a 2d representation used by the function that generates the mesh.
        /// Providing a pullbackSurface can be helpful when it is similar in shape
        /// to the pattern of the input, the pulled 2d points will be a better
        /// representation of the 3d points. If all of the input is more or less
        /// coplanar to start with, providing pullbackSurface has no real benefit.
        /// </param>
        /// <param name="innerBothSideCurves">
        /// (optional: can be null) These polylines will create faces on both sides
        /// of the edge. If there are only input points(innerPoints) there is no
        /// way to guarantee a triangulation that will create an edge between two
        /// particular points. Adding a line, or polyline, to innerBothsideCurves
        /// that includes points from innerPoints will help guide the triangulation.
        /// </param>
        /// <param name="innerPoints">
        /// (optional: can be null) Points to be used to generate the mesh. If
        /// outerBoundary is not null, points outside of that boundary after it has
        /// been pulled to pullbackSurface (or the best plane through the input if
        /// pullbackSurface is null) will be ignored.
        /// </param>
        /// <param name="trimback">
        /// Only used when a outerBoundary has not been provided. When that is the
        /// case, the function uses the perimeter of the surface as the outer boundary
        /// instead. If true, any face of the resulting triangulated mesh that
        /// contains a vertex of the perimeter boundary will be removed.
        /// </param>
        /// <param name="divisions">
        /// Only used when a outerBoundary has not been provided. When that is the
        /// case, division becomes the number of divisions each side of the surface's
        /// perimeter will be divided into to create an outer boundary to work with.
        /// </param>
        /// <returns>mesh on success; null on failure</returns>
        public static Mesh CreatePatch(Polyline outerBoundary, double angleToleranceRadians, Surface pullbackSurface, IEnumerable<Curve> innerBoundaryCurves, IEnumerable<Curve> innerBothSideCurves, IEnumerable<Point3d> innerPoints, bool trimback, int divisions)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), outerBoundary, angleToleranceRadians, pullbackSurface, innerBoundaryCurves, innerBothSideCurves, innerPoints, trimback, divisions);
        }
        /// <summary>
        /// Computes the solid union of a set of meshes.
        /// </summary>
        /// <param name="meshes">Meshes to union.</param>
        /// <returns>An array of Mesh results or null on failure.</returns>
        public static Mesh[] CreateBooleanUnion(IEnumerable<Mesh> meshes)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), meshes);
        }
        /// <summary>
        /// Computes the solid difference of two sets of Meshes.
        /// </summary>
        /// <param name="firstSet">First set of Meshes (the set to subtract from).</param>
        /// <param name="secondSet">Second set of Meshes (the set to subtract).</param>
        /// <returns>An array of Mesh results or null on failure.</returns>
        public static Mesh[] CreateBooleanDifference(IEnumerable<Mesh> firstSet, IEnumerable<Mesh> secondSet)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), firstSet, secondSet);
        }
        /// <summary>
        /// Computes the solid intersection of two sets of meshes.
        /// </summary>
        /// <param name="firstSet">First set of Meshes.</param>
        /// <param name="secondSet">Second set of Meshes.</param>
        /// <returns>An array of Mesh results or null on failure.</returns>
        public static Mesh[] CreateBooleanIntersection(IEnumerable<Mesh> firstSet, IEnumerable<Mesh> secondSet)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), firstSet, secondSet);
        }
        /// <summary>
        /// Splits a set of meshes with another set.
        /// </summary>
        /// <param name="meshesToSplit">A list, an array, or any enumerable set of meshes to be split. If this is null, null will be returned.</param>
        /// <param name="meshSplitters">A list, an array, or any enumerable set of meshes that cut. If this is null, null will be returned.</param>
        /// <returns>A new mesh array, or null on error.</returns>
        public static Mesh[] CreateBooleanSplit(IEnumerable<Mesh> meshesToSplit, IEnumerable<Mesh> meshSplitters)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), meshesToSplit, meshSplitters);
        }

        /// <summary>
        /// Compute volume of the mesh. 
        /// </summary>
        /// <returns>Volume of the mesh.</returns>
        public static double Volume(this Mesh mesh)
        {
            return ComputeServer.Post<double>(ApiAddress(), mesh);
        }
        /// <summary>
        /// Makes sure that faces sharing an edge and having a difference of normal greater
        /// than or equal to angleToleranceRadians have unique vertexes along that edge,
        /// adding vertices if necessary.
        /// </summary>
        /// <param name="angleToleranceRadians">Angle at which to make unique vertices.</param>
        /// <param name="modifyNormals">
        /// Determines whether new vertex normals will have the same vertex normal as the original (false)
        /// or vertex normals made from the corrsponding face normals (true)
        /// </param>
        public static Mesh Unweld(this Mesh mesh, double angleToleranceRadians, bool modifyNormals)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh, angleToleranceRadians, modifyNormals);
        }
        /// <summary>
        /// Adds creases to a smooth mesh by creating coincident vertices along selected edges.
        /// </summary>
        /// <param name="edgeIndices">An array of mesh topology edge indices.</param>
        /// <param name="modifyNormals">
        /// If true, the vertex normals on each side of the edge take the same value as the face to which they belong, giving the mesh a hard edge look.
        /// If false, each of the vertex normals on either side of the edge is assigned the same value as the original normal that the pair is replacing, keeping a smooth look.
        /// </param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool UnweldEdge(this Mesh mesh, out Mesh updatedInstance, IEnumerable<int> edgeIndices, bool modifyNormals)
        {
            return ComputeServer.Post<bool, Mesh>(ApiAddress(), out updatedInstance, mesh, edgeIndices, modifyNormals);
        }
        /// <summary>
        /// Makes sure that faces sharing an edge and having a difference of normal greater
        /// than or equal to angleToleranceRadians share vertexes along that edge, vertex normals
        /// are averaged.
        /// </summary>
        /// <param name="angleToleranceRadians">Angle at which to weld vertices.</param>
        public static Mesh Weld(this Mesh mesh, double angleToleranceRadians)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh, angleToleranceRadians);
        }
        /// <summary>
        /// Removes mesh normals and reconstructs the face and vertex normals based
        /// on the orientation of the faces.
        /// </summary>
        public static Mesh RebuildNormals(this Mesh mesh)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh);
        }
        /// <summary>
        /// Extracts, or removes, non-manifold mesh edges. 
        /// </summary>
        /// <param name="selective">If true, then extract hanging faces only.</param>
        /// <returns>A mesh containing the extracted non-manifold parts if successful, null otherwise.</returns>
        public static Mesh ExtractNonManifoldEdges(this Mesh mesh, out Mesh updatedInstance, bool selective)
        {
            return ComputeServer.Post<Mesh, Mesh>(ApiAddress(), out updatedInstance, mesh, selective);
        }
        /// <summary>
        /// Attempts to "heal" naked edges in a mesh based on a given distance.  
        /// First attempts to move vertexes to neighboring vertexes that are within that
        /// distance away. Then it finds edges that have a closest point to the vertex within
        /// the distance and splits the edge. When it finds one it splits the edge and
        /// makes two new edges using that point.
        /// </summary>
        /// <param name="distance">Distance to not exceed when modifying the mesh.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool HealNakedEdges(this Mesh mesh, out Mesh updatedInstance, double distance)
        {
            return ComputeServer.Post<bool, Mesh>(ApiAddress(), out updatedInstance, mesh, distance);
        }
        /// <summary>
        /// Attempts to determine "holes" in the mesh by chaining naked edges together. 
        /// Then it triangulates the closed polygons adds the faces to the mesh.
        /// </summary>
        /// <returns>true if successful, false otherwise.</returns>
        /// <remarks>This function does not differentiate between inner and outer naked edges.  
        /// If you need that, it would be better to use Mesh.FillHole.
        /// </remarks>
        public static bool FillHoles(this Mesh mesh, out Mesh updatedInstance)
        {
            return ComputeServer.Post<bool, Mesh>(ApiAddress(), out updatedInstance, mesh);
        }
        /// <summary>
        /// Given a starting "naked" edge index, this function attempts to determine a "hole"
        /// by chaining additional naked edges together until if returns to the start index.
        /// Then it triangulates the closed polygon and either adds the faces to the mesh.
        /// </summary>
        /// <param name="topologyEdgeIndex">Starting naked edge index.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool FileHole(this Mesh mesh, out Mesh updatedInstance, int topologyEdgeIndex)
        {
            return ComputeServer.Post<bool, Mesh>(ApiAddress(), out updatedInstance, mesh, topologyEdgeIndex);
        }
        /// <summary>
        /// Attempts to fix inconsistencies in the directions of mesh faces in a mesh. This function
        /// does not modify mesh vertex normals, it rearranges the mesh face winding and face
        /// normals to make them all consistent. Note, you may want to call Mesh.Normals.ComputeNormals()
        /// to recompute vertex normals after calling this functions.
        /// </summary>
        /// <returns>number of faces that were modified.</returns>
        public static int UnifyNormals(this Mesh mesh, out Mesh updatedInstance)
        {
            return ComputeServer.Post<int, Mesh>(ApiAddress(), out updatedInstance, mesh);
        }
        /// <summary>
        /// Attempts to fix inconsistencies in the directions of mesh faces in a mesh. This function
        /// does not modify mesh vertex normals, it rearranges the mesh face winding and face
        /// normals to make them all consistent. Note, you may want to call Mesh.Normals.ComputeNormals()
        /// to recompute vertex normals after calling this functions.
        /// </summary>
        /// <param name="countOnly">If true, then only the number of faces that would be modified is determined.</param>
        /// <returns>If countOnly=false, the number of faces that were modified. If countOnly=true, the number of faces that would be modified.</returns>
        public static int UnifyNormals(this Mesh mesh, out Mesh updatedInstance, bool countOnly)
        {
            return ComputeServer.Post<int, Mesh>(ApiAddress(), out updatedInstance, mesh, countOnly);
        }
        /// <summary>
        /// Splits up the mesh into its unconnected pieces.
        /// </summary>
        /// <returns>An array containing all the disjoint pieces that make up this Mesh.</returns>
        public static Mesh[] SplitDisjointPieces(this Mesh mesh)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), mesh);
        }
        /// <summary>
        /// Split a mesh by an infinite plane.
        /// </summary>
        /// <param name="plane">The splitting plane.</param>
        /// <returns>A new mesh array with the split result. This can be null if no result was found.</returns>
        public static Mesh[] Split(this Mesh mesh, Plane plane)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), mesh, plane);
        }
        /// <summary>
        /// Split a mesh with another mesh.
        /// </summary>
        /// <param name="mesh">Mesh to split with.</param>
        /// <returns>An array of mesh segments representing the split result.</returns>
        public static Mesh[] Split(this Mesh thisMesh, Mesh mesh)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), thisMesh, mesh);
        }
        /// <summary>
        /// Split a mesh with a collection of meshes.
        /// </summary>
        /// <param name="meshes">Meshes to split with.</param>
        /// <returns>An array of mesh segments representing the split result.</returns>
        public static Mesh[] Split(this Mesh mesh, IEnumerable<Mesh> meshes)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), mesh, meshes);
        }
        /// <summary>
        /// Constructs the outlines of a mesh projected against a plane.
        /// </summary>
        /// <param name="plane">A plane to project against.</param>
        /// <returns>An array of polylines, or null on error.</returns>
        public static Polyline[] GetOutlines(this Mesh mesh, Plane plane)
        {
            return ComputeServer.Post<Polyline[]>(ApiAddress(), mesh, plane);
        }
        /// <summary>
        /// Returns all edges of a mesh that are considered "naked" in the
        /// sense that the edge only has one face.
        /// </summary>
        /// <returns>An array of polylines, or null on error.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_dupmeshboundary.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_dupmeshboundary.cs' lang='cs'/>
        /// <code source='examples\py\ex_dupmeshboundary.py' lang='py'/>
        /// </example>
        public static Polyline[] GetNakedEdges(this Mesh mesh)
        {
            return ComputeServer.Post<Polyline[]>(ApiAddress(), mesh);
        }
        /// <summary>
        /// Explode the mesh into submeshes where a submesh is a collection of faces that are contained
        /// within a closed loop of "unwelded" edges. Unwelded edges are edges where the faces that share
        /// the edge have unique mesh vertexes (not mesh topology vertexes) at both ends of the edge.
        /// </summary>
        /// <returns>
        /// Array of submeshes on success; null on error. If the count in the returned array is 1, then
        /// nothing happened and the ouput is essentially a copy of the input.
        /// </returns>
        public static Mesh[] ExplodeAtUnweldedEdges(this Mesh mesh)
        {
            return ComputeServer.Post<Mesh[]>(ApiAddress(), mesh);
        }
        /// <summary>
        /// Gets the point on the mesh that is closest to a given test point.
        /// </summary>
        /// <param name="testPoint">Point to seach for.</param>
        /// <returns>The point on the mesh closest to testPoint, or Point3d.Unset on failure.</returns>
        public static Point3d ClosestPoint(this Mesh mesh, Point3d testPoint)
        {
            return ComputeServer.Post<Point3d>(ApiAddress(), mesh, testPoint);
        }
        /// <summary>
        /// Gets the point on the mesh that is closest to a given test point. Similar to the 
        /// ClosestPoint function except this returns a MeshPoint class which includes
        /// extra information beyond just the location of the closest point.
        /// </summary>
        /// <param name="testPoint">The source of the search.</param>
        /// <param name="maximumDistance">
        /// Optional upper bound on the distance from test point to the mesh. 
        /// If you are only interested in finding a point Q on the mesh when 
        /// testPoint.DistanceTo(Q) &lt; maximumDistance, 
        /// then set maximumDistance to that value. 
        /// This parameter is ignored if you pass 0.0 for a maximumDistance.
        /// </param>
        /// <returns>closest point information on success. null on failure.</returns>
        public static MeshPoint ClosestMeshPoint(this Mesh mesh, Point3d testPoint, double maximumDistance)
        {
            return ComputeServer.Post<MeshPoint>(ApiAddress(), mesh, testPoint, maximumDistance);
        }
        /// <summary>
        /// Gets the point on the mesh that is closest to a given test point.
        /// </summary>
        /// <param name="testPoint">Point to seach for.</param>
        /// <param name="pointOnMesh">Point on the mesh closest to testPoint.</param>
        /// <param name="maximumDistance">
        /// Optional upper bound on the distance from test point to the mesh. 
        /// If you are only interested in finding a point Q on the mesh when 
        /// testPoint.DistanceTo(Q) &lt; maximumDistance, 
        /// then set maximumDistance to that value. 
        /// This parameter is ignored if you pass 0.0 for a maximumDistance.
        /// </param>
        /// <returns>
        /// Index of face that the closest point lies on if successful. 
        /// -1 if not successful; the value of pointOnMesh is undefined.
        /// </returns>
        public static int ClosestPoint(this Mesh mesh, Point3d testPoint, out Point3d pointOnMesh, double maximumDistance)
        {
            return ComputeServer.Post<int, Point3d>(ApiAddress(), out pointOnMesh, mesh, testPoint, maximumDistance);
        }
        /// <summary>
        /// Gets the point on the mesh that is closest to a given test point.
        /// </summary>
        /// <param name="testPoint">Point to seach for.</param>
        /// <param name="pointOnMesh">Point on the mesh closest to testPoint.</param>
        /// <param name="normalAtPoint">The normal vector of the mesh at the closest point.</param>
        /// <param name="maximumDistance">
        /// Optional upper bound on the distance from test point to the mesh. 
        /// If you are only interested in finding a point Q on the mesh when 
        /// testPoint.DistanceTo(Q) &lt; maximumDistance, 
        /// then set maximumDistance to that value. 
        /// This parameter is ignored if you pass 0.0 for a maximumDistance.
        /// </param>
        /// <returns>
        /// Index of face that the closest point lies on if successful. 
        /// -1 if not successful; the value of pointOnMesh is undefined.
        /// </returns>
        public static int ClosestPoint(this Mesh mesh, Point3d testPoint, out Point3d pointOnMesh, out Vector3d normalAtPoint, double maximumDistance)
        {
            return ComputeServer.Post<int, Point3d, Vector3d>(ApiAddress(), out pointOnMesh, out normalAtPoint, mesh, testPoint, maximumDistance);
        }
        /// <summary>
        /// Evaluate a mesh at a set of barycentric coordinates.
        /// </summary>
        /// <param name="meshPoint">MeshPoint instance contiaining a valid Face Index and Barycentric coordinates.</param>
        /// <returns>A Point on the mesh or Point3d.Unset if the faceIndex is not valid or if the barycentric coordinates could not be evaluated.</returns>
        public static Point3d PointAt(this Mesh mesh, MeshPoint meshPoint)
        {
            return ComputeServer.Post<Point3d>(ApiAddress(), mesh, meshPoint);
        }
        /// <summary>
        /// Evaluates a mesh at a set of barycentric coordinates. Barycentric coordinates must 
        /// be assigned in accordance with the rules as defined by MeshPoint.T.
        /// </summary>
        /// <param name="faceIndex">Index of triangle or quad to evaluate.</param>
        /// <param name="t0">First barycentric coordinate.</param>
        /// <param name="t1">Second barycentric coordinate.</param>
        /// <param name="t2">Third barycentric coordinate.</param>
        /// <param name="t3">Fourth barycentric coordinate. If the face is a triangle, this coordinate will be ignored.</param>
        /// <returns>A Point on the mesh or Point3d.Unset if the faceIndex is not valid or if the barycentric coordinates could not be evaluated.</returns>
        public static Point3d PointAt(this Mesh mesh, int faceIndex, double t0, double t1, double t2, double t3)
        {
            return ComputeServer.Post<Point3d>(ApiAddress(), mesh, faceIndex, t0, t1, t2, t3);
        }
        /// <summary>
        /// Evaluate a mesh normal at a set of barycentric coordinates.
        /// </summary>
        /// <param name="meshPoint">MeshPoint instance contiaining a valid Face Index and Barycentric coordinates.</param>
        /// <returns>A Normal vector to the mesh or Vector3d.Unset if the faceIndex is not valid or if the barycentric coordinates could not be evaluated.</returns>
        public static Vector3d NormalAt(this Mesh mesh, MeshPoint meshPoint)
        {
            return ComputeServer.Post<Vector3d>(ApiAddress(), mesh, meshPoint);
        }
        /// <summary>
        /// Evaluate a mesh normal at a set of barycentric coordinates. Barycentric coordinates must 
        /// be assigned in accordance with the rules as defined by MeshPoint.T.
        /// </summary>
        /// <param name="faceIndex">Index of triangle or quad to evaluate.</param>
        /// <param name="t0">First barycentric coordinate.</param>
        /// <param name="t1">Second barycentric coordinate.</param>
        /// <param name="t2">Third barycentric coordinate.</param>
        /// <param name="t3">Fourth barycentric coordinate. If the face is a triangle, this coordinate will be ignored.</param>
        /// <returns>A Normal vector to the mesh or Vector3d.Unset if the faceIndex is not valid or if the barycentric coordinates could not be evaluated.</returns>
        public static Vector3d NormalAt(this Mesh mesh, int faceIndex, double t0, double t1, double t2, double t3)
        {
            return ComputeServer.Post<Vector3d>(ApiAddress(), mesh, faceIndex, t0, t1, t2, t3);
        }
        /// <summary>
        /// Pulls a collection of points to a mesh.
        /// </summary>
        /// <param name="points">An array, a list or any enumerable set of points.</param>
        /// <returns>An array of points. This can be empty.</returns>
        public static Point3d[] PullPointsToMesh(this Mesh mesh, IEnumerable<Point3d> points)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), mesh, points);
        }
        /// <summary>
        /// Makes a new mesh with vertices offset a distance in the opposite direction of the existing vertex normals.
        /// Same as Mesh.Offset(distance, false)
        /// </summary>
        /// <param name="distance">A distance value to use for offsetting.</param>
        /// <returns>A new mesh on success, or null on failure.</returns>
        public static Mesh Offset(this Mesh mesh, double distance)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh, distance);
        }
        /// <summary>
        /// Makes a new mesh with vertices offset a distance in the opposite direction of the existing vertex normals.
        /// Optionally, based on the value of solidify, adds the input mesh and a ribbon of faces along any naked edges.
        /// If solidify is false it acts exactly as the Offset(distance) function.
        /// </summary>
        /// <param name="distance">A distance value.</param>
        /// <param name="solidify">true if the mesh should be solidified.</param>
        /// <returns>A new mesh on success, or null on failure.</returns>
        public static Mesh Offset(this Mesh mesh, double distance, bool solidify)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh, distance, solidify);
        }
        /// <summary>
        /// Makes a new mesh with vertices offset a distance along the direction parameter.
        /// Optionally, based on the value of solidify, adds the input mesh and a ribbon of faces along any naked edges.
        /// If solidify is false it acts exactly as the Offset(distance) function.
        /// </summary>
        /// <param name="distance">A distance value.</param>
        /// <param name="solidify">true if the mesh should be solidified.</param>
        /// <param name="direction">Direction of offset for all vertices.</param>
        /// <returns>A new mesh on success, or null on failure.</returns>
        public static Mesh Offset(this Mesh mesh, double distance, bool solidify, Vector3d direction)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh, distance, solidify, direction);
        }
        /// <summary>
        /// Collapses multiple mesh faces, with greater/less than edge length, based on the principles 
        /// found in Stan Melax's mesh reduction PDF, 
        /// see http://pomax.nihongoresources.com/downloads/PolygonReduction.pdf
        /// </summary>
        /// <param name="bGreaterThan">Determines whether edge with lengths greater than or less than edgeLength are collapsed.</param>
        /// <param name="edgeLength">Length with which to compare to edge lengths.</param>
        /// <returns>Number of edges (faces) that were collapsed.</returns>
        /// <remarks>
        /// This number may differ from the initial number of edges that meet
        /// the input criteria because the lengths of some initial edges may be altered as other edges are collapsed.
        /// </remarks>
        public static int CollapseFacesByEdgeLength(this Mesh mesh, out Mesh updatedInstance, bool bGreaterThan, double edgeLength)
        {
            return ComputeServer.Post<int, Mesh>(ApiAddress(), out updatedInstance, mesh, bGreaterThan, edgeLength);
        }
        /// <summary>
        /// Collapses multiple mesh faces, with areas less than LessThanArea and greater than GreaterThanArea, 
        /// based on the principles found in Stan Melax's mesh reduction PDF, 
        /// see http://pomax.nihongoresources.com/downloads/PolygonReduction.pdf
        /// </summary>
        /// <param name="lessThanArea">Area in which faces are selected if their area is less than or equal to.</param>
        /// <param name="greaterThanArea">Area in which faces are selected if their area is greater than or equal to.</param>
        /// <returns>Number of faces that were collapsed in the process.</returns>
        /// <remarks>
        /// This number may differ from the initial number of faces that meet
        /// the input criteria because the areas of some initial faces may be altered as other faces are collapsed.
        /// The face area must be both less than LessThanArea AND greater than GreaterThanArea in order to be considered.  
        /// Use large numbers for lessThanArea or zero for greaterThanArea to simulate an OR.
        /// </remarks>
        public static int CollapseFacesByArea(this Mesh mesh, out Mesh updatedInstance, double lessThanArea, double greaterThanArea)
        {
            return ComputeServer.Post<int, Mesh>(ApiAddress(), out updatedInstance, mesh, lessThanArea, greaterThanArea);
        }
        /// <summary>
        /// Collapses a multiple mesh faces, determined by face aspect ratio, based on criteria found in Stan Melax's polygon reduction,
        /// see http://pomax.nihongoresources.com/downloads/PolygonReduction.pdf
        /// </summary>
        /// <param name="aspectRatio">Faces with an aspect ratio less than aspectRatio are considered as candidates.</param>
        /// <returns>Number of faces that were collapsed in the process.</returns>
        /// <remarks>
        /// This number may differ from the initial number of faces that meet 
        /// the input criteria because the aspect ratios of some initial faces may be altered as other faces are collapsed.
        /// </remarks>
        public static int CollapseFacesByByAspectRatio(this Mesh mesh, out Mesh updatedInstance, double aspectRatio)
        {
            return ComputeServer.Post<int, Mesh>(ApiAddress(), out updatedInstance, mesh, aspectRatio);
        }
        /// <summary>
        /// Constructs new mesh from the current one, with edge softening applied to it.
        /// </summary>
        /// <param name="softeningRadius">The softening radius.</param>
        /// <param name="chamfer">Specifies whether to chamfer the edges.</param>
        /// <param name="faceted">Specifies whether the edges are faceted.</param>
        /// <param name="force">Specifies whether to soften edges despite too large a radius.</param>
        /// <param name="angleThreshold">Threshold angle (in degrees) which controls whether an edge is softened or not.
        /// The angle refers to the angles between the adjacent faces of an edge.</param>
        /// <returns>A new mesh with soft edges.</returns>
        /// <exception cref="InvalidOperationException">If displacement failed
        /// because of an error. The exception message specifies the error.</exception>
        public static Mesh WithEdgeSoftening(this Mesh mesh, double softeningRadius, bool chamfer, bool faceted, bool force, double angleThreshold)
        {
            return ComputeServer.Post<Mesh>(ApiAddress(), mesh, softeningRadius, chamfer, faceted, force, angleThreshold);
        }
        /// <summary>
        /// Reduce polygon count
        /// </summary>
        /// <param name="desiredPolygonCount">desired or target number of faces</param>
        /// <param name="allowDistortion">
        /// If true mesh appearance is not changed even if the target polygon count is not reached
        /// </param>
        /// <param name="accuracy">Integer from 1 to 10 telling how accurate reduction algorithm
        ///  to use. Greater number gives more accurate results
        /// </param>
        /// <param name="normalizeSize">If true mesh is fitted to an axis aligned unit cube until reduction is complete</param>
        /// <returns>True if mesh is successfully reduced and false if mesh could not be reduced for some reason.</returns>
        public static bool Reduce(this Mesh mesh, out Mesh updatedInstance, int desiredPolygonCount, bool allowDistortion, int accuracy, bool normalizeSize)
        {
            return ComputeServer.Post<bool, Mesh>(ApiAddress(), out updatedInstance, mesh, desiredPolygonCount, allowDistortion, accuracy, normalizeSize);
        }
        /// <summary>
        /// Constructs contour curves for a mesh, sectioned along a linear axis.
        /// </summary>
        /// <param name="meshToContour">A mesh to contour.</param>
        /// <param name="contourStart">A start point of the contouring axis.</param>
        /// <param name="contourEnd">An end point of the contouring axis.</param>
        /// <param name="interval">An interval distance.</param>
        /// <returns>An array of curves. This array can be empty.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_makerhinocontours.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_makerhinocontours.cs' lang='cs'/>
        /// <code source='examples\py\ex_makerhinocontours.py' lang='py'/>
        /// </example>
        public static Curve[] CreateContourCurves(Mesh meshToContour, Point3d contourStart, Point3d contourEnd, double interval)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), meshToContour, contourStart, contourEnd, interval);
        }
        /// <summary>
        /// Constructs contour curves for a mesh, sectioned at a plane.
        /// </summary>
        /// <param name="meshToContour">A mesh to contour.</param>
        /// <param name="sectionPlane">A cutting plane.</param>
        /// <returns>An array of curves. This array can be empty.</returns>
        public static Curve[] CreateContourCurves(Mesh meshToContour, Plane sectionPlane)
        {
            return ComputeServer.Post<Curve[]>(ApiAddress(), meshToContour, sectionPlane);
        }
    }

    public static class NurbsCurveCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(NurbsCurve), caller);
        }
        /// <summary>
        /// For expert use only. From the input curves, make an array of compatible NURBS curves.
        /// </summary>
        /// <param name="curves">The input curves.</param>
        /// <param name="startPt">The start point. To omit, specify Point3d.Unset.</param>
        /// <param name="endPt">The end point. To omit, specify Point3d.Unset.</param>
        /// <param name="simplifyMethod">The simplify method.</param>
        /// <param name="numPoints">The number of rebuild points.</param>
        /// <param name="refitTolerance">The refit tolerance.</param>
        /// <param name="angleTolerance">The angle tolerance in radians.</param>
        /// <returns>The output NURBS surfaces if successful.</returns>
        public static NurbsCurve[] MakeCompatible(IEnumerable<Curve> curves, Point3d startPt, Point3d endPt, int simplifyMethod, int numPoints, double refitTolerance, double angleTolerance)
        {
            return ComputeServer.Post<NurbsCurve[]>(ApiAddress(), curves, startPt, endPt, simplifyMethod, numPoints, refitTolerance, angleTolerance);
        }
        /// <summary>
        /// Createsa a parabola from vertex and end points.
        /// </summary>
        /// <param name="vertex">The vertex point.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point</param>
        /// <returns>A 2 degree NURBS curve if successful, false otherwise.</returns>
        public static NurbsCurve CreateParabolaFromVertex(Point3d vertex, Point3d startPoint, Point3d endPoint)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), vertex, startPoint, endPoint);
        }
        /// <summary>
        /// Creates a parabola from focus and end points.
        /// </summary>
        /// <param name="focus">The focal point.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point</param>
        /// <returns>A 2 degree NURBS curve if successful, false otherwise.</returns>
        public static NurbsCurve CreateParabolaFromFocus(Point3d focus, Point3d startPoint, Point3d endPoint)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), focus, startPoint, endPoint);
        }
        /// <summary>
        /// Create a uniform non-ratonal cubic NURBS approximation of an arc.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="degree">&gt;=1</param>
        /// <param name="cvCount">cv count &gt;=5</param>
        /// <returns>NURBS curve approximation of an arc on success</returns>
        public static NurbsCurve CreateFromArc(Arc arc, int degree, int cvCount)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), arc, degree, cvCount);
        }
        /// <summary>
        /// Create a uniform non-ratonal cubic NURBS approximation of a circle.
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="degree">&gt;=1</param>
        /// <param name="cvCount">cv count &gt;=5</param>
        /// <returns>NURBS curve approximation of a circle on success</returns>
        public static NurbsCurve CreateFromCircle(Circle circle, int degree, int cvCount)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), circle, degree, cvCount);
        }
        /// <summary>
        /// Sets all Greville (Edit) points for this curve.
        /// </summary>
        /// <param name="points">
        /// The new point locations. The number of points should match 
        /// the number of point returned by NurbsCurve.GrevillePoints().
        /// </param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SetGrevillePoints(this NurbsCurve nurbscurve, out NurbsCurve updatedInstance, IEnumerable<Point3d> points)
        {
            return ComputeServer.Post<bool, NurbsCurve>(ApiAddress(), out updatedInstance, nurbscurve, points);
        }
        /// <summary>
        /// Creates a C1 cubic NURBS approximation of a helix or spiral. For a helix,
        /// you may have radius0 == radius1. For a spiral radius0 == radius0 produces
        /// a circle. Zero and negative radii are permissible.
        /// </summary>
        /// <param name="axisStart">Helix's axis starting point or center of spiral.</param>
        /// <param name="axisDir">Helix's axis vector or normal to spiral's plane.</param>
        /// <param name="radiusPoint">
        /// Point used only to get a vector that is perpedicular to the axis. In
        /// particular, this vector must not be (anti)parallel to the axis vector.
        /// </param>
        /// <param name="pitch">
        /// The pitch, where a spiral has a pitch = 0, and pitch > 0 is the distance
        /// between the helix's "threads".
        /// </param>
        /// <param name="turnCount">The number of turns in spiral or helix. Positive
        /// values produce counter-clockwise orientation, negitive values produce
        /// clockwise orientation. Note, for a helix, turnCount * pitch = length of
        /// the helix's axis.
        /// </param>
        /// <param name="radius0">The starting radius.</param>
        /// <param name="radius1">The ending radius.</param>
        /// <returns>NurbsCurve on success, null on failure.</returns>
        public static NurbsCurve CreateSpiral(Point3d axisStart, Vector3d axisDir, Point3d radiusPoint, double pitch, double turnCount, double radius0, double radius1)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), axisStart, axisDir, radiusPoint, pitch, turnCount, radius0, radius1);
        }
        /// <summary>
        /// Create a C2 non-rational uniform cubic NURBS approximation of a swept helix or spiral.
        /// </summary>
        /// <param name="railCurve">The rail curve.</param>
        /// <param name="t0">Starting portion of rail curve's domain to sweep along.</param>
        /// <param name="t1">Ending portion of rail curve's domain to sweep along.</param>
        /// <param name="radiusPoint">
        /// Point used only to get a vector that is perpedicular to the axis. In
        /// particular, this vector must not be (anti)parallel to the axis vector.
        /// </param>
        /// <param name="pitch">
        /// The pitch. Positive values produce counter-clockwise orientation,
        /// negative values produce clockwise orientation.
        /// </param>
        /// <param name="turnCount">
        /// The turn count. If != 0, then the resulting helix will have this many
        /// turns. If = 0, then pitch must be != 0 and the approximate distance
        /// between turns will be set to pitch. Positive values produce counter-clockwise
        /// orientation, negitive values produce clockwise orientation.
        /// </param>
        /// <param name="radius0">
        /// The starting radius. At least one radii must benonzero. Negative values
        /// are allowed.
        /// </param>
        /// <param name="radius1">
        /// The ending radius. At least ont radii must be nonzero. Negative values
        /// are allowed.
        /// </param>
        /// <param name="pointsPerTurn">
        /// Number of points to intepolate per turn. Must be greater than 4.
        /// When in doubt, use 12.
        /// </param>
        /// <returns>NurbsCurve on success, null on failure.</returns>
        public static NurbsCurve CreateSpiral(Curve railCurve, double t0, double t1, Point3d radiusPoint, double pitch, double turnCount, double radius0, double radius1, int pointsPerTurn)
        {
            return ComputeServer.Post<NurbsCurve>(ApiAddress(), railCurve, t0, t1, radiusPoint, pitch, turnCount, radius0, radius1, pointsPerTurn);
        }
    }
}

namespace Rhino.Compute.Intersect
{
    public static class IntersectionCompute
    {
        static string ApiAddress([CallerMemberName] string caller = null)
        {
            return ComputeServer.ApiAddress(typeof(Intersection), caller);
        }
        /// <summary>
        /// Intersects a curve with an (infinite) plane.
        /// </summary>
        /// <param name="curve">Curve to intersect.</param>
        /// <param name="plane">Plane to intersect with.</param>
        /// <param name="tolerance">Tolerance to use during intersection.</param>
        /// <returns>A list of intersection events or null if no intersections were recorded.</returns>
        public static CurveIntersections CurvePlane(Curve curve, Plane plane, double tolerance)
        {
            return ComputeServer.Post<CurveIntersections>(ApiAddress(), curve, plane, tolerance);
        }
        /// <summary>
        /// Intersects a mesh with an (infinite) plane.
        /// </summary>
        /// <param name="mesh">Mesh to intersect.</param>
        /// <param name="plane">Plane to intersect with.</param>
        /// <returns>An array of polylines describing the intersection loops or null (Nothing in Visual Basic) if no intersections could be found.</returns>
        public static Polyline[] MeshPlane(Mesh mesh, Plane plane)
        {
            return ComputeServer.Post<Polyline[]>(ApiAddress(), mesh, plane);
        }
        /// <summary>
        /// Intersects a mesh with a collection of (infinite) planes.
        /// </summary>
        /// <param name="mesh">Mesh to intersect.</param>
        /// <param name="planes">Planes to intersect with.</param>
        /// <returns>An array of polylines describing the intersection loops or null (Nothing in Visual Basic) if no intersections could be found.</returns>
        /// <exception cref="ArgumentNullException">If planes is null.</exception>
        public static Polyline[] MeshPlane(Mesh mesh, IEnumerable<Plane> planes)
        {
            return ComputeServer.Post<Polyline[]>(ApiAddress(), mesh, planes);
        }
        /// <summary>
        /// Intersects a Brep with an (infinite) plane.
        /// </summary>
        /// <param name="brep">Brep to intersect.</param>
        /// <param name="plane">Plane to intersect with.</param>
        /// <param name="tolerance">Tolerance to use for intersections.</param>
        /// <param name="intersectionCurves">The intersection curves will be returned here.</param>
        /// <param name="intersectionPoints">The intersection points will be returned here.</param>
        /// <returns>true on success, false on failure.</returns>
        public static bool BrepPlane(Brep brep, Plane plane, double tolerance, out Curve[] intersectionCurves, out Point3d[] intersectionPoints)
        {
            return ComputeServer.Post<bool, Curve[], Point3d[]>(ApiAddress(), out intersectionCurves, out intersectionPoints, brep, plane, tolerance);
        }
        /// <summary>
        /// Finds the places where a curve intersects itself. 
        /// </summary>
        /// <param name="curve">Curve for self-intersections.</param>
        /// <param name="tolerance">Intersection tolerance. If the curve approaches itself to within tolerance, 
        /// an intersection is assumed.</param>
        /// <returns>A collection of intersection events.</returns>
        public static CurveIntersections CurveSelf(Curve curve, double tolerance)
        {
            return ComputeServer.Post<CurveIntersections>(ApiAddress(), curve, tolerance);
        }
        /// <summary>
        /// Finds the intersections between two curves. 
        /// </summary>
        /// <param name="curveA">First curve for intersection.</param>
        /// <param name="curveB">Second curve for intersection.</param>
        /// <param name="tolerance">Intersection tolerance. If the curves approach each other to within tolerance, 
        /// an intersection is assumed.</param>
        /// <param name="overlapTolerance">The tolerance with which the curves are tested.</param>
        /// <returns>A collection of intersection events.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_intersectcurves.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_intersectcurves.cs' lang='cs'/>
        /// <code source='examples\py\ex_intersectcurves.py' lang='py'/>
        /// </example>
        public static CurveIntersections CurveCurve(Curve curveA, Curve curveB, double tolerance, double overlapTolerance)
        {
            return ComputeServer.Post<CurveIntersections>(ApiAddress(), curveA, curveB, tolerance, overlapTolerance);
        }
        /// <summary>
        /// Intersects a curve and an infinite line. 
        /// </summary>
        /// <param name="curve">Curve for intersection.</param>
        /// <param name="line">Infinite line to intesect.</param>
        /// <param name="tolerance">Intersection tolerance. If the curves approach each other to within tolerance, 
        /// an intersection is assumed.</param>
        /// <param name="overlapTolerance">The tolerance with which the curves are tested.</param>
        /// <returns>A collection of intersection events.</returns>
        public static CurveIntersections CurveLine(Curve curve, Line line, double tolerance, double overlapTolerance)
        {
            return ComputeServer.Post<CurveIntersections>(ApiAddress(), curve, line, tolerance, overlapTolerance);
        }
        /// <summary>
        /// Intersects a curve and a surface.
        /// </summary>
        /// <param name="curve">Curve for intersection.</param>
        /// <param name="surface">Surface for intersection.</param>
        /// <param name="tolerance">Intersection tolerance. If the curve approaches the surface to within tolerance, 
        /// an intersection is assumed.</param>
        /// <param name="overlapTolerance">The tolerance with which the curves are tested.</param>
        /// <returns>A collection of intersection events.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_curvesurfaceintersect.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_curvesurfaceintersect.cs' lang='cs'/>
        /// <code source='examples\py\ex_curvesurfaceintersect.py' lang='py'/>
        /// </example>
        public static CurveIntersections CurveSurface(Curve curve, Surface surface, double tolerance, double overlapTolerance)
        {
            return ComputeServer.Post<CurveIntersections>(ApiAddress(), curve, surface, tolerance, overlapTolerance);
        }
        /// <summary>
        /// Intersects a (sub)curve and a surface.
        /// </summary>
        /// <param name="curve">Curve for intersection.</param>
        /// <param name="curveDomain">Domain of surbcurve to take into consideration for Intersections.</param>
        /// <param name="surface">Surface for intersection.</param>
        /// <param name="tolerance">Intersection tolerance. If the curve approaches the surface to within tolerance, 
        /// an intersection is assumed.</param>
        /// <param name="overlapTolerance">The tolerance with which the curves are tested.</param>
        /// <returns>A collection of intersection events.</returns>
        public static CurveIntersections CurveSurface(Curve curve, Interval curveDomain, Surface surface, double tolerance, double overlapTolerance)
        {
            return ComputeServer.Post<CurveIntersections>(ApiAddress(), curve, curveDomain, surface, tolerance, overlapTolerance);
        }
        /// <summary>
        /// Intersects a curve with a Brep. This function returns the 3D points of intersection
        /// and 3D overlap curves. If an error occurs while processing overlap curves, this function 
        /// will return false, but it will still provide partial results.
        /// </summary>
        /// <param name="curve">Curve for intersection.</param>
        /// <param name="brep">Brep for intersection.</param>
        /// <param name="tolerance">Fitting and near miss tolerance.</param>
        /// <param name="overlapCurves">The overlap curves will be returned here.</param>
        /// <param name="intersectionPoints">The intersection points will be returned here.</param>
        /// <returns>true on success, false on failure.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_elevation.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_elevation.cs' lang='cs'/>
        /// <code source='examples\py\ex_elevation.py' lang='py'/>
        /// </example>
        public static bool CurveBrep(Curve curve, Brep brep, double tolerance, out Curve[] overlapCurves, out Point3d[] intersectionPoints)
        {
            return ComputeServer.Post<bool, Curve[], Point3d[]>(ApiAddress(), out overlapCurves, out intersectionPoints, curve, brep, tolerance);
        }
        /// <summary>
        /// Intersect a curve with a Brep. This function returns the intersection parameters on the curve.
        /// </summary>
        /// <param name="curve">Curve.</param>
        /// <param name="brep">Brep.</param>
        /// <param name="tolerance">Absolute tolerance for intersections.</param>
        /// <param name="angleTolerance">Angle tolerance in radians.</param>
        /// <param name="t">Curve parameters at intersections.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool CurveBrep(Curve curve, Brep brep, double tolerance, double angleTolerance, out double[] t)
        {
            return ComputeServer.Post<bool, double[]>(ApiAddress(), out t, curve, brep, tolerance, angleTolerance);
        }
        /// <summary>
        /// Intersects two Surfaces.
        /// </summary>
        /// <param name="surfaceA">First Surface for intersection.</param>
        /// <param name="surfaceB">Second Surface for intersection.</param>
        /// <param name="tolerance">Intersection tolerance.</param>
        /// <param name="intersectionCurves">The intersection curves will be returned here.</param>
        /// <param name="intersectionPoints">The intersection points will be returned here.</param>
        /// <returns>true on success, false on failure.</returns>
        public static bool SurfaceSurface(Surface surfaceA, Surface surfaceB, double tolerance, out Curve[] intersectionCurves, out Point3d[] intersectionPoints)
        {
            return ComputeServer.Post<bool, Curve[], Point3d[]>(ApiAddress(), out intersectionCurves, out intersectionPoints, surfaceA, surfaceB, tolerance);
        }
        /// <summary>
        /// Intersects two Breps.
        /// </summary>
        /// <param name="brepA">First Brep for intersection.</param>
        /// <param name="brepB">Second Brep for intersection.</param>
        /// <param name="tolerance">Intersection tolerance.</param>
        /// <param name="intersectionCurves">The intersection curves will be returned here.</param>
        /// <param name="intersectionPoints">The intersection points will be returned here.</param>
        /// <returns>true on success; false on failure.</returns>
        public static bool BrepBrep(Brep brepA, Brep brepB, double tolerance, out Curve[] intersectionCurves, out Point3d[] intersectionPoints)
        {
            return ComputeServer.Post<bool, Curve[], Point3d[]>(ApiAddress(), out intersectionCurves, out intersectionPoints, brepA, brepB, tolerance);
        }
        /// <summary>
        /// Intersects a Brep and a Surface.
        /// </summary>
        /// <param name="brep">A brep to be intersected.</param>
        /// <param name="surface">A surface to be intersected.</param>
        /// <param name="tolerance">A tolerance value.</param>
        /// <param name="intersectionCurves">The intersection curves array argument. This out reference is assigned during the call.</param>
        /// <param name="intersectionPoints">The intersection points array argument. This out reference is assigned during the call.</param>
        /// <returns>true on success; false on failure.</returns>
        public static bool BrepSurface(Brep brep, Surface surface, double tolerance, out Curve[] intersectionCurves, out Point3d[] intersectionPoints)
        {
            return ComputeServer.Post<bool, Curve[], Point3d[]>(ApiAddress(), out intersectionCurves, out intersectionPoints, brep, surface, tolerance);
        }
        /// <summary>
        /// Quickly intersects two meshes. Overlaps and near misses are ignored.
        /// </summary>
        /// <param name="meshA">First mesh for intersection.</param>
        /// <param name="meshB">Second mesh for intersection.</param>
        /// <returns>An array of intersection line segments, or null.</returns>
        public static Line[] MeshMeshFast(Mesh meshA, Mesh meshB)
        {
            return ComputeServer.Post<Line[]>(ApiAddress(), meshA, meshB);
        }
        /// <summary>
        /// Intersects two meshes. Overlaps and near misses are handled.
        /// </summary>
        /// <param name="meshA">First mesh for intersection.</param>
        /// <param name="meshB">Second mesh for intersection.</param>
        /// <param name="tolerance">Intersection tolerance.</param>
        /// <returns>An array of intersection polylines.</returns>
        public static Polyline[] MeshMeshAccurate(Mesh meshA, Mesh meshB, double tolerance)
        {
            return ComputeServer.Post<Polyline[]>(ApiAddress(), meshA, meshB, tolerance);
        }
        /// <summary>Finds the first intersection of a ray with a mesh.</summary>
        /// <param name="mesh">A mesh to intersect.</param>
        /// <param name="ray">A ray to be casted.</param>
        /// <returns>
        /// >= 0.0 parameter along ray if successful.
        /// &lt; 0.0 if no intersection found.
        /// </returns>
        public static double MeshRay(Mesh mesh, Ray3d ray)
        {
            return ComputeServer.Post<double>(ApiAddress(), mesh, ray);
        }
        /// <summary>Finds the first intersection of a ray with a mesh.</summary>
        /// <param name="mesh">A mesh to intersect.</param>
        /// <param name="ray">A ray to be casted.</param>
        /// <param name="meshFaceIndices">faces on mesh that ray intersects.</param>
        /// <returns>
        /// >= 0.0 parameter along ray if successful.
        /// &lt; 0.0 if no intersection found.
        /// </returns>
        /// <remarks>
        /// The ray may intersect more than one face in cases where the ray hits
        /// the edge between two faces or the vertex corner shared by multiple faces.
        /// </remarks>
        public static double MeshRay(Mesh mesh, Ray3d ray, out int[] meshFaceIndices)
        {
            return ComputeServer.Post<double, int[]>(ApiAddress(), out meshFaceIndices, mesh, ray);
        }
        /// <summary>
        /// Finds the intersection of a mesh and a polyline.
        /// </summary>
        /// <param name="mesh">A mesh to intersect.</param>
        /// <param name="curve">A polyline curves to intersect.</param>
        /// <param name="faceIds">The indices of the intersecting faces. This out reference is assigned during the call.</param>
        /// <returns>An array of points: one for each face that was passed by the faceIds out reference.</returns>
        public static Point3d[] MeshPolyline(Mesh mesh, PolylineCurve curve, out int[] faceIds)
        {
            return ComputeServer.Post<Point3d[], int[]>(ApiAddress(), out faceIds, mesh, curve);
        }
        /// <summary>
        /// Finds the intersection of a mesh and a line
        /// </summary>
        /// <param name="mesh">A mesh to intersect</param>
        /// <param name="line">The line to intersect with the mesh</param>
        /// <param name="faceIds">The indices of the intersecting faces. This out reference is assigned during the call.</param>
        /// <returns>An array of points: one for each face that was passed by the faceIds out reference.</returns>
        public static Point3d[] MeshLine(Mesh mesh, Line line, out int[] faceIds)
        {
            return ComputeServer.Post<Point3d[], int[]>(ApiAddress(), out faceIds, mesh, line);
        }
        /// <summary>
        /// Computes point intersections that occur when shooting a ray to a collection of surfaces.
        /// </summary>
        /// <param name="ray">A ray used in intersection.</param>
        /// <param name="geometry">Only Surface and Brep objects are currently supported. Trims are ignored on Breps.</param>
        /// <param name="maxReflections">The maximum number of reflections. This value should be any value between 1 and 1000, inclusive.</param>
        /// <returns>An array of points: one for each face that was passed by the faceIds out reference.</returns>
        /// <exception cref="ArgumentNullException">geometry is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxReflections is strictly outside the [1-1000] range.</exception>
        public static Point3d[] RayShoot(Ray3d ray, IEnumerable<GeometryBase> geometry, int maxReflections)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), ray, geometry, maxReflections);
        }
        /// <summary>
        /// Projects points onto meshes.
        /// </summary>
        /// <param name="meshes">the meshes to project on to.</param>
        /// <param name="points">the points to project.</param>
        /// <param name="direction">the direction to project.</param>
        /// <param name="tolerance">
        /// Projection tolerances used for culling close points and for line-mesh intersection.
        /// </param>
        /// <returns>
        /// Array of projected points, or null in case of any error or invalid input.
        /// </returns>
        public static Point3d[] ProjectPointsToMeshes(IEnumerable<Mesh> meshes, IEnumerable<Point3d> points, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), meshes, points, direction, tolerance);
        }
        /// <summary>
        /// Projects points onto meshes.
        /// </summary>
        /// <param name="meshes">the meshes to project on to.</param>
        /// <param name="points">the points to project.</param>
        /// <param name="direction">the direction to project.</param>
        /// <param name="tolerance">
        /// Projection tolerances used for culling close points and for line-mesh intersection.
        /// </param>
        /// <param name="indices">Return points[i] is a projection of points[indices[i]]</param>
        /// <returns>
        /// Array of projected points, or null in case of any error or invalid input.
        /// </returns>
        /// <example>
        /// <code source='examples\vbnet\ex_projectpointstomeshesex.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_projectpointstomeshesex.cs' lang='cs'/>
        /// <code source='examples\py\ex_projectpointstomeshesex.py' lang='py'/>
        /// </example>
        public static Point3d[] ProjectPointsToMeshesEx(IEnumerable<Mesh> meshes, IEnumerable<Point3d> points, Vector3d direction, double tolerance, out int[] indices)
        {
            return ComputeServer.Post<Point3d[], int[]>(ApiAddress(), out indices, meshes, points, direction, tolerance);
        }
        /// <summary>
        /// Projects points onto breps.
        /// </summary>
        /// <param name="breps">The breps projection targets.</param>
        /// <param name="points">The points to project.</param>
        /// <param name="direction">The direction to project.</param>
        /// <param name="tolerance">The tolerance used for intersections.</param>
        /// <returns>
        /// Array of projected points, or null in case of any error or invalid input.
        /// </returns>
        /// <example>
        /// <code source='examples\vbnet\ex_projectpointstobreps.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_projectpointstobreps.cs' lang='cs'/>
        /// <code source='examples\py\ex_projectpointstobreps.py' lang='py'/>
        /// </example>
        public static Point3d[] ProjectPointsToBreps(IEnumerable<Brep> breps, IEnumerable<Point3d> points, Vector3d direction, double tolerance)
        {
            return ComputeServer.Post<Point3d[]>(ApiAddress(), breps, points, direction, tolerance);
        }
        /// <summary>
        /// Projects points onto breps.
        /// </summary>
        /// <param name="breps">The breps projection targets.</param>
        /// <param name="points">The points to project.</param>
        /// <param name="direction">The direction to project.</param>
        /// <param name="tolerance">The tolerance used for intersections.</param>
        /// <param name="indices">Return points[i] is a projection of points[indices[i]]</param>
        /// <returns>
        /// Array of projected points, or null in case of any error or invalid input.
        /// </returns>
        public static Point3d[] ProjectPointsToBrepsEx(IEnumerable<Brep> breps, IEnumerable<Point3d> points, Vector3d direction, double tolerance, out int[] indices)
        {
            return ComputeServer.Post<Point3d[], int[]>(ApiAddress(), out indices, breps, points, direction, tolerance);
        }
    }

    public class IntersectionEvent
    {
        /// <summary>
        /// All curve intersection events are either a single point or an overlap.
        /// </summary>
        public bool IsPoint { get; set; }

        /// <summary>
        /// All curve intersection events are either a single point or an overlap.
        /// </summary>
        /// <example>
        /// <code source='examples\vbnet\ex_curvesurfaceintersect.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_curvesurfaceintersect.cs' lang='cs'/>
        /// <code source='examples\py\ex_curvesurfaceintersect.py' lang='py'/>
        /// </example>
        public bool IsOverlap { get; set; }

        /// <summary>
        /// Gets the point on Curve A where the intersection occured. 
        /// If the intersection type is overlap, then this will return the 
        /// start of the overlap region.
        /// </summary>
        public Point3d PointA { get; set; }
        /// <summary>
        /// Gets the end point of the overlap on Curve A. 
        /// If the intersection type is not overlap, this value is meaningless.
        /// </summary>
        public Point3d PointA2 { get; set; }

        /// <summary>
        /// Gets the point on Curve B (or Surface B) where the intersection occured. 
        /// If the intersection type is overlap, then this will return the 
        /// start of the overlap region.
        /// </summary>
        public Point3d PointB { get; set; }
        /// <summary>
        /// Gets the end point of the overlap on Curve B (or Surface B). 
        /// If the intersection type is not overlap, this value is meaningless.
        /// </summary>
        public Point3d PointB2 { get; set; }

        /// <summary>
        /// Gets the parameter on Curve A where the intersection occured. 
        /// If the intersection type is overlap, then this will return the 
        /// start of the overlap region.
        /// </summary>
        public double ParameterA { get; set; }
        /// <summary>
        /// Gets the parameter on Curve B where the intersection occured. 
        /// If the intersection type is overlap, then this will return the 
        /// start of the overlap region.
        /// </summary>
        public double ParameterB { get; set; }

        /// <summary>
        /// Gets the interval on curve A where the overlap occurs. 
        /// If the intersection type is not overlap, this value is meaningless.
        /// </summary>
        public Interval OverlapA { get; set; }

        /// <summary>
        /// Gets the interval on curve B where the overlap occurs. 
        /// If the intersection type is not overlap, this value is meaningless.
        /// </summary>
        public Interval OverlapB { get; set; }
    }

    public class CurveIntersections : List<IntersectionEvent> { }
}
