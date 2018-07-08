using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores the resulting vertices and indices
/// of an iso surface extraction
/// </summary>
public struct IsosurfaceMeshResult
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
    /// Initializes a new <see cref="IsosurfaceMeshResult"/> from a list of
    /// vertices and indices.
    /// </summary>
    public IsosurfaceMeshResult(List<Vector3> vertices, List<int> indices) : this(vertices, null, indices)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="IsosurfaceMeshResult"/> from a list of
    /// vertices, normals, and indices.
    /// </summary>
    public IsosurfaceMeshResult(List<Vector3> vertices, List<Vector3> normals, List<int> indices)
    {
        Vertices = vertices;
        Normals = normals;
        Indices = indices;
    }
}