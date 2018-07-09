using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores the resulting vertices and indices
/// of an isosurface extraction
/// </summary>
public class IsosurfaceMesh
{
    /// <summary>
    /// The vertices making up the mesh
    /// </summary>
    public List<Vector3> Vertices { get; }

    /// <summary>
    /// The normals
    /// </summary>
    public List<Vector3> Normals { get; }

    /// <summary>
    /// The indices making up the faces of the mesh
    /// </summary>
    public List<int> Indices { get; }

    /// <summary>
    /// The index of the last vertex added.
    /// </summary>
    public int IndexOfLastVertex => Vertices.Count - 1;

    /// <summary>
    /// Initializes a new empty <see cref="IsosurfaceMesh"/>.
    /// </summary>
    public IsosurfaceMesh()
    {
        Vertices = new List<Vector3>();
        Normals = new List<Vector3>();
        Indices = new List<int>();
    }

    /// <summary>
    /// Initializes a new <see cref="IsosurfaceMesh"/> from a list of
    /// vertices and indices.
    /// </summary>
    public IsosurfaceMesh(List<Vector3> vertices, List<int> indices) : this(vertices, null, indices)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="IsosurfaceMesh"/> from a list of
    /// vertices, normals, and indices.
    /// </summary>
    public IsosurfaceMesh(List<Vector3> vertices, List<Vector3> normals, List<int> indices)
    {
        Vertices = vertices;
        Normals = normals;
        Indices = indices;
    }

    /// <summary>
    /// Adds a vertex to the <see cref="IsosurfaceMesh"/>.
    /// </summary>
    public void AddVertex(Vector3 vertex) => Vertices.Add(vertex);

    /// <summary>
    /// Adds a normal to the <see cref="IsosurfaceMesh"/>.
    /// </summary>
    public void AddNormal(Vector3 normal) => Normals.Add(normal);

    /// <summary>
    /// Adds an index to the <see cref="IsosurfaceMesh"/>.
    /// </summary>
    public void AddIndex(int index) => Indices.Add(index);
}