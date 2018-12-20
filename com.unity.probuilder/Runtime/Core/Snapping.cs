using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.ProBuilder
{
    /// <summary>
    /// Snapping functions and ProGrids compatibility.
    /// </summary>
    static class Snapping
    {
        const float k_MaxRaySnapDistance = Mathf.Infinity;

        /// <summary>
        /// Round value to nearest snpVal increment.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="snpVal"></param>
        /// <returns></returns>
        public static Vector3 SnapValue(Vector3 vertex, float snpVal)
        {
            // snapValue is a global setting that comes from ProGrids
            return new Vector3(
                snpVal * Mathf.Round(vertex.x / snpVal),
                snpVal * Mathf.Round(vertex.y / snpVal),
                snpVal * Mathf.Round(vertex.z / snpVal));
        }

        /// <summary>
        /// Round value to nearest snpVal increment.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="snpVal"></param>
        /// <returns></returns>
        public static float SnapValue(float val, float snpVal)
        {
            if (snpVal < Mathf.Epsilon)
                return val;
            return snpVal * Mathf.Round(val / snpVal);
        }

        /// <summary>
        /// An override that accepts a vector3 to use as a mask for which values to snap.  Ex;
        /// Snap((.3f, 3f, 41f), (0f, 1f, .4f)) only snaps Y and Z values (to 1 & .4 unit increments).
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="snap"></param>
        /// <returns></returns>
        public static Vector3 SnapValue(Vector3 vertex, Vector3 snap)
        {
            float _x = vertex.x, _y = vertex.y, _z = vertex.z;
            Vector3 v = new Vector3(
                    (Mathf.Abs(snap.x) < 0.0001f ? _x : snap.x * Mathf.Round(_x / snap.x)),
                    (Mathf.Abs(snap.y) < 0.0001f ? _y : snap.y * Mathf.Round(_y / snap.y)),
                    (Mathf.Abs(snap.z) < 0.0001f ? _z : snap.z * Mathf.Round(_z / snap.z))
                    );
            return v;
        }

        /// <summary>
        /// Snap all vertices to an increment of @snapValue in world space.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="indexes"></param>
        /// <param name="snap"></param>
        public static void SnapVertices(ProBuilderMesh mesh, IEnumerable<int> indexes, Vector3 snap)
        {
            Vector3[] verts = mesh.positionsInternal;

            foreach (var v in indexes)
                verts[v] = mesh.transform.InverseTransformPoint(SnapValue(mesh.transform.TransformPoint(verts[v]), snap));
        }

        internal static Vector3 GetSnappingMaskBasedOnNormalVector(Vector3 normal)
        {
            return new Vector3(
                (Mathf.Approximately(Mathf.Abs(normal.x), 1f)) ? 0f : 1f,
                (Mathf.Approximately(Mathf.Abs(normal.y), 1f)) ? 0f : 1f,
                (Mathf.Approximately(Mathf.Abs(normal.z), 1f)) ? 0f : 1f);
        }

        public static Vector3 Ceil(Vector3 vertex, float snpVal)
        {
            return new Vector3(
                snpVal * Mathf.Ceil(vertex.x / snpVal),
                snpVal * Mathf.Ceil(vertex.y / snpVal),
                snpVal * Mathf.Ceil(vertex.z / snpVal));
        }

        public static Vector3 Floor(Vector3 vertex, float snpVal)
        {
            // snapValue is a global setting that comes from ProGrids
            return new Vector3(
                snpVal * Mathf.Floor(vertex.x / snpVal),
                snpVal * Mathf.Floor(vertex.y / snpVal),
                snpVal * Mathf.Floor(vertex.z / snpVal));
        }

        internal static Vector3 SnapValueOnRay(Ray ray, float distance, float snap, Vector3Mask mask)
        {
            var nearest = k_MaxRaySnapDistance;
            var snapped = ray.origin + ray.direction * distance;

            var forwardRay = new Ray(ray.origin, ray.direction);
            var backwardsRay = new Ray(ray.origin, -ray.direction);

            for (int i = 0; i < 3; i++)
            {
                if (mask[i] > 0f)
                {
                    var dir = new Vector3Mask(new Vector3Mask((byte) (1 << i)));

                    var prj = Vector3.Project(
                        ray.direction * Math.MakeNonZero(distance),
                        dir * Mathf.Sign(ray.direction[i]));

                    var pnt = ray.origin + prj;

                    var forwardPlane = new Plane(dir, Ceil(pnt, snap));
                    var backwardPlane = new Plane(dir, Floor(pnt, snap));
                    float d;

                    if (forwardPlane.Raycast(forwardRay, out d) && Mathf.Abs(d) < nearest)
                        nearest = d;
                    if (forwardPlane.Raycast(backwardsRay, out d) && Mathf.Abs(d) < nearest)
                        nearest = -d;
                    if (backwardPlane.Raycast(forwardRay, out d) && Mathf.Abs(d) < nearest)
                        nearest = d;
                    if (backwardPlane.Raycast(backwardsRay, out d) && Mathf.Abs(d) < nearest)
                        nearest = -d;

                    if (Event.current.type == EventType.Repaint)
                    {
                        UnityEditor.Handles.color = Color.yellow;
                        UnityEditor.Handles.DrawLine(ray.origin, ray.origin + ray.direction * 100f);

                        UnityEditor.Handles.color = Color.red;
                        UnityEditor.Handles.DrawLine(ray.origin, ray.origin + prj);

                        UnityEditor.Handles.color = Color.white;

                        var verts = new Vector3[]
                        {
                            (Quaternion.LookRotation(prj) * new Vector3(-.5f, -.5f)) + SnapValue(ray.origin + prj, snap),
                            (Quaternion.LookRotation(prj) * new Vector3(-.5f,  .5f)) + SnapValue(ray.origin + prj, snap),
                            (Quaternion.LookRotation(prj) * new Vector3(.5f, .5f)) + SnapValue(ray.origin + prj, snap),
                            (Quaternion.LookRotation(prj) * new Vector3(.5f,  -.5f)) + SnapValue(ray.origin + prj, snap)
                        };

                        UnityEditor.Handles.DrawSolidRectangleWithOutline(verts, new Color(.1f, .1f, .1f, .5f), Color.black);
                    }
                }
            }

            return ray.origin + ray.direction * (nearest > k_MaxRaySnapDistance ? distance : nearest);
        }
    }
}
