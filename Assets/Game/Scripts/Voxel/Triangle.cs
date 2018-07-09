using UnityEngine;

public struct Triangle
{
    public Vector3Int Point { get; }
    public int Edge0 { get; }
    public int Edge1 { get; }
    public int Edge2 { get; }

    public Triangle(Vector3Int point, int edge0, int edge1, int edge2)
    {
        Point = point;
        Edge0 = edge0;
        Edge1 = edge1;
        Edge2 = edge2;
    }
}