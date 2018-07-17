using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpatialVolume<T> : Volume<T> where T : struct
{
    public SpatialChunk<T>[] Chunks => data.Values.ToArray();

    public const int ChunkSize = 64;
    private readonly Dictionary<Vector3Int, SpatialChunk<T>> data;

    public SpatialVolume()
    {
        data = new Dictionary<Vector3Int, SpatialChunk<T>>();
    }

    public override T this[int x, int y, int z]
    {
        get
        {
            if (x < 0 || y < 0 || z < 0) return default(T);

            Vector3Int chunkCoordinate = ChunkToVolumeCoordinate(x, y, z);
            if (!data.ContainsKey(chunkCoordinate)) return default(T);

            SpatialChunk<T> chunk = data[chunkCoordinate];
            return chunk[x % ChunkSize, y % ChunkSize, z % ChunkSize];

        }
        set
        {
            Vector3Int chunkCoordinate = ChunkToVolumeCoordinate(x, y, z);
            SpatialChunk<T> chunk;

            if (data.ContainsKey(chunkCoordinate))
            {
                chunk = data[chunkCoordinate];
            }
            else
            {
                chunk = new SpatialChunk<T>(chunkCoordinate, ChunkSize);
                data[chunkCoordinate] = chunk;
            }

            chunk[x % ChunkSize, y % ChunkSize, z % ChunkSize] = value;
        }
    }

    public override int Size { get; set; } = ChunkSize;

    /// <summary>
    /// Tranposes a local chunk coordinate to a spatial volume coordinate.
    /// </summary>
    public Vector3Int ChunkToVolumeCoordinate(int x, int y, int z) =>
        new Vector3Int(x / ChunkSize, y / ChunkSize, z / ChunkSize) * ChunkSize;

    /// <summary>
    /// Tranposes a local chunk coordinate to a spatial volume coordinate.
    /// </summary>
    public Vector3Int ChunkToVolumeCoordinate(Vector3Int localCoordinate) => 
        new Vector3Int(localCoordinate.x / ChunkSize, localCoordinate.y / ChunkSize, localCoordinate.z / ChunkSize) * ChunkSize;
}