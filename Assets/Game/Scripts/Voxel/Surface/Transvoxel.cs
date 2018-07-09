using System.Collections.Concurrent;
using UnityEngine;

public class Transvoxel
{
    public ConcurrentDictionary<Vector3, Chunk> Chunks { get; }
    public Volume<sbyte> Volume { get; }

    private readonly Isosurface isosurface;

    public Transvoxel()
    {
        Chunks = new ConcurrentDictionary<Vector3, Chunk>();
        Volume = new SpatialVolume<sbyte>();
        isosurface = new Isosurface(Volume);
    }

    public void GenerateMesh(SpatialChunk<sbyte> spatialChunk)
    {
        int levelOfDistance = 1;

        IsosurfaceMesh chunkMesh = isosurface.Generate(spatialChunk);
        Chunk chunk = new Chunk
        {
            Bounds = new Bounds(spatialChunk.Position,
                spatialChunk.Position + new Vector3Int(spatialChunk.Size, spatialChunk.Size, spatialChunk.Size)),
            Position = spatialChunk.Position,
            LevelOfDistance = levelOfDistance,
            Mesh = chunkMesh
        };

        if (Chunks.ContainsKey(spatialChunk.Position))
        {
            Chunk removed;
            Chunks.TryRemove(spatialChunk.Position, out removed);
        }

        Chunks.TryAdd(spatialChunk.Position, chunk);
    }
}
