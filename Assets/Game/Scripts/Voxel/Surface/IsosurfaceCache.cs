using UnityEngine;

public struct IsosurfaceCacheCell
{
    public int[] Vertices { get; set; }

    public IsosurfaceCacheCell(int size)
    {
        Vertices = new int[size];
        for (int i = 0; i < size; i++)
        {
            Vertices[i] = -1;
        }
    }
}

public class IsosurfaceCache
{
    public IsosurfaceCacheCell this[int x, int y, int z]
    {
        set
        {
            Debug.Assert(x >= 0 && y >= 0 && z >= 0);
            cache[x & 1, y * chunkSize + z] = value;
        }
    }

    public IsosurfaceCacheCell this[Vector3Int position]
    {
        set { this[position.x, position.y, position.z] = value; }
    }

    private readonly IsosurfaceCacheCell[,] cache;
    private readonly int chunkSize;

    public IsosurfaceCache(int chunkSize)
    {
        this.chunkSize = chunkSize;
        int squareChunkSize = chunkSize * chunkSize;

        cache = new IsosurfaceCacheCell[2, squareChunkSize];
        for (int i = 0; i < squareChunkSize; i++)
        {
            cache[0, i] = new IsosurfaceCacheCell(4);
            cache[1, i] = new IsosurfaceCacheCell(4);
        }
    }

    public IsosurfaceCacheCell GetCachedCell(Vector3Int position, int edgeDirection)
    {
        int ex = edgeDirection & 1;
        int ey = (edgeDirection >> 2) & 1;
        int ez = (edgeDirection >> 1) & 1;

        int dx = position.x - ex;
        int dy = position.y - ey;
        int dz = position.z - ez;

        Debug.Assert(dx >= 0 && dy >= 0 && dz >= 0);
        return cache[dx & 1, dy * chunkSize + dz];
    }

    public void SetCachedCell(Vector3Int positon, int index, int vertexIndex) => 
        cache[positon.x & 1, positon.y * chunkSize + positon.z].Vertices[index] = vertexIndex;
}
