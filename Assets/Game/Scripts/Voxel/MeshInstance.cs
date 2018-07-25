using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshInstance
{
    public GameObject GameObject { get; }
    public Transform Transform => GameObject.transform;

    public MeshFilter MeshFilter { get; }
    public MeshRenderer MeshRenderer { get; }

    public Mesh Mesh => MeshFilter.sharedMesh;

    public MeshInstance(string name)
    {
        GameObject = new GameObject(name);

        MeshFilter = GameObject.AddComponent<MeshFilter>();
        MeshRenderer = GameObject.AddComponent<MeshRenderer>();
    }

    public void FromSurfaceResult(IsosurfaceMeshResult surfaceResult)
    {
        MeshFilter.sharedMesh = new Mesh();
        Mesh.SetVertices(surfaceResult.Vertices);
        Mesh.SetTriangles(surfaceResult.Indices, 0);

        if (surfaceResult.Normals != null && surfaceResult.Normals.Count > 0)
        {
            Mesh.SetNormals(surfaceResult.Normals);
        }
        else
        {
            Mesh.RecalculateNormals();
        }

        Mesh.RecalculateBounds();
    }

    public bool Raycast(Ray ray, out Vector3 point)
    {
        point = Vector3.zero;
        if (Mesh == null) return false;

        int[] triangles = Mesh.triangles;
        Vector3[] vertices = Mesh.vertices;

        Vector3 intersection = Vector3.zero;
        float minimumDistance = Mathf.Infinity;

        Vector3 position = Transform.position;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i]] + position;
            Vector3 p2 = vertices[triangles[i + 1]] + position;
            Vector3 p3 = vertices[triangles[i + 2]] + position;

            if (!Intersect(p1, p2, p3, ray, out point)) continue;

            float distance = Vector3.Distance(ray.origin, point);
            if (distance < minimumDistance)
            {
                intersection = point;
                minimumDistance = distance;
            }
        }

        point = intersection;
        return minimumDistance != Mathf.Infinity;
    }

    public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray, out Vector3 point)
    {
        point = Vector3.zero;

        Vector3 e1 = p2 - p1;
        Vector3 e2 = p3 - p1;

        Vector3 p = Vector3.Cross(ray.direction, e2);
        float det = Vector3.Dot(e1, p);

        if (det > -Mathf.Epsilon && det < Mathf.Epsilon) return false;
        float inverseDet = 1.0f / det;

        Vector3 t = ray.origin - p1;
        float u = Vector3.Dot(t, p) * inverseDet;

        if (u < 0 || u > 1) return false;

        Vector3 q = Vector3.Cross(t, e1);
        float v = Vector3.Dot(ray.direction, q) * inverseDet;

        if (v < 0 || u + v > 1) return false;

        point = p1 + u * e1 + v * e2;
        return Vector3.Dot(e2, q) * inverseDet > Mathf.Epsilon;
    }
}