using UnityEngine;

public class Chunk
{
    public Vector3 Position { get; set; }
    public IsosurfaceMesh Mesh { get; set; }
    public Bounds Bounds { get; set; }
    public int LevelOfDistance { get; set; }
}