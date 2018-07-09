using UnityEngine;

public abstract class Volume<T>
{
    public abstract T this[int x, int y, int z] { get; set; }

    public T this[Vector3Int coordinate]
    {
        get { return this[coordinate.x, coordinate.y, coordinate.z]; }
        set { this[coordinate.x, coordinate.y, coordinate.z] = value; }
    }

    public abstract int Size { get; set; }
}
