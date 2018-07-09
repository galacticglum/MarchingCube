using UnityEngine;

public class SpatialChunk<T> where T : struct
{
    public Vector3Int Position { get; }
    public int Size { get; }

    private readonly T[] data;

    public SpatialChunk(Vector3Int position, int size)
    {
        Position = position;
        Size = size;

        data = new T[size * size * size];
    }

    public T this[int x, int y, int z]
    {
        get { return data[x + y * Size + z * Size * Size]; }
        set { data[x + y * Size + z * Size * Size] = value; }
    }

    public T this[Vector3Int position]
    {
        get { return this[position.x, position.y, position.z]; }
        set { this[position.x, position.y, position.z] = value; }
    }
}
