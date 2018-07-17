using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    [SerializeField]
    private Material material;
    [SerializeField]
    private int volumeSize = 128;
    [SerializeField]
    private int radius = 10;

    private Transvoxel transvoxel;
    private List<MeshInstance> meshInstances;

    private void Start()
    {
        meshInstances = new List<MeshInstance>();
        transvoxel = new Transvoxel
        {
            Volume =
            {
                Size = volumeSize
            }
        };

        for (int x = 0; x <= volumeSize; x++)
        {
            for (int y = 0; y <= volumeSize; y++)
            {
                for (int z = 0; z <= volumeSize; z++)
                {
                    float density = x * x + y * y + z * z - radius * radius;
                    transvoxel.Volume[x, y, z] = (sbyte)density;
                }
            }
        }

        Generate();
    }

    private void Generate()
    {
        SpatialChunk<sbyte>[] spatialChunks = ((SpatialVolume<sbyte>) transvoxel.Volume).Chunks;
        foreach (SpatialChunk<sbyte> spatialChunk in spatialChunks)
        {
            transvoxel.GenerateMesh(spatialChunk);
        }

        List<Chunk> chunks = transvoxel.Chunks.Values.ToList();
        for (int i = 0; i < chunks.Count; i++)
        {
            MeshInstance meshInstance = new MeshInstance($"MESH_{i}", transform);
            meshInstances.Add(meshInstance);

            meshInstance.FromSurfaceResult(chunks[i].IsosurfaceMesh, material);
        }
    }
}
