using System.Collections.Generic;
using UnityEngine;

public class IsosurfaceMesh
{
    public List<Vector3> Vertices { get; }
    public List<int> Indices { get; }

    public int VertexCount { get; private set; }
    public int IndiceCount{ get; private set; }
    public int FaceCount { get; private set; }

    public IsosurfaceMesh()
    {
        Vertices = new List<Vector3>();
        Indices = new List<int>();
    }

    public int AddVertex(Vector3 vertex)
    {
        VertexCount++;
        Vertices.Add(vertex);

        return VertexCount - 1;
    }

    public int AddFace(Face face)
    {
        FaceCount++;
        IndiceCount += 3;

        Indices.Add(face.Index0);
        Indices.Add(face.Index1);
        Indices.Add(face.Index2);

        return FaceCount - 1;
    }

    public void Clear()
    {
        Vertices.Clear();
        Indices.Clear();
    }
}