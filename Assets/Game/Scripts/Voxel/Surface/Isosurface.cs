using UnityEngine;

public class Isosurface
{
    public bool UseCache { get; set; }

    private readonly Volume<sbyte> volume;
    private readonly IsosurfaceCache cache;

    public Isosurface(Volume<sbyte> volume)
    {
        this.volume = volume;

        UseCache = true;
        cache = new IsosurfaceCache(volume.Size * 10);
    }

    public IsosurfaceMesh Generate(SpatialChunk<sbyte> chunk)
    {
        IsosurfaceMesh isosurfaceMesh = new IsosurfaceMesh();

        // Level of distance is a scaling factor
        // applied to the mesh
        const int levelOfDistance = 1;

        for (int x = 0; x < volume.Size; x++)
        {
            for (int y = 0; y < volume.Size; y++)
            {
                for (int z = 0; z < volume.Size; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    PolygoniseCell(chunk.Position, position, levelOfDistance, ref isosurfaceMesh);
                }
            }
        }

        return isosurfaceMesh;
    }

    private void PolygoniseCell(Vector3Int offset, Vector3Int position, int levelOfDistance, ref IsosurfaceMesh isosurfaceMesh)
    {
        Debug.Assert(levelOfDistance >= 1, "levelOfDistance >= 1");
        offset += position * levelOfDistance;

        int directionMask = (position.x > 0 ? 1 : 0) | ((position.z > 0 ? 1 : 0) << 1) | ((position.y > 0 ? 1 : 0) << 2);

        sbyte[] cube = new sbyte[8];
        for (int i = 0; i < 8; i++)
        {
            cube[i] = volume[offset + VoxelTables.CubeVertexOffsets[i] * levelOfDistance];
        }

        int edgeFlagIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            edgeFlagIndex |= (cube[i] >> (7 - i)) & (1 << i);
        }

        // If this is equal to zero then it means that there
        // is no triangulation needed for this edge flag index.
        if ((edgeFlagIndex ^ ((cube[7] >> 7) & 255)) == 0) return;

        Vector3[] vertexNormals = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3Int p = offset + VoxelTables.CubeVertexOffsets[i] * levelOfDistance;
            vertexNormals[i].x = (volume[p + Vector3Int.right] - volume[p - Vector3Int.right]) * 0.5f;
            vertexNormals[i].y = (volume[p + Vector3Int.up] - volume[p - Vector3Int.up]) * 0.5f;
            vertexNormals[i].z = (volume[p + MathsHelper.Vector3IntForward] - volume[p - MathsHelper.Vector3IntForward]) * 0.5f;
            vertexNormals[i].Normalize();
        }

        byte edgeIndex = VoxelTables.EdgeTable[edgeFlagIndex];
        ushort[] vertices = VoxelTables.VertexTriangleTable[edgeFlagIndex];

        IsosurfaceCell cell = VoxelTables.CellData[edgeIndex];
        int[] indices = new int[cell.Indices.Length];

        for (int i = 0; i < cell.VertexCount; i++)
        {
            int edge = vertices[i] >> 8;
            int cacheVertexIndex = edge & 0xf;
            int edgeDirection = edge >> 4;

            int v0 = (vertices[i] >> 4) & 0x0f;
            int v1 = vertices[i] & 0x0f;

            sbyte density0 = cube[v0];
            sbyte density1 = cube[v1];

            Debug.Assert(v1 > v0);

            int alpha = (density1 << 8) / (density1 - density0);
            int u = 0x0100 - alpha;

            float t0 = alpha / 256f;
            float t1 = u / 255f;

            int index = -1;
            if (UseCache && v1 != 7 && (edgeDirection & directionMask) == edgeDirection)
            {
                Debug.Assert(cacheVertexIndex != 0);
                IsosurfaceCacheCell cacheCell = cache.GetCachedCell(position, edgeDirection);
                index = cacheCell.Vertices[cacheVertexIndex];
            }

            // If the index is still -1 then either we are not
            // using the cache or something horribly went wrong
            // with the cache
            if (index == -1)
            {
                Vector3 normal = vertexNormals[v0] * t0 + vertexNormals[v1] * t1;
                Vector3 vertex = GenerateVertex(offset, v0, v1, alpha, levelOfDistance);

                isosurfaceMesh.AddVertex(vertex);
                isosurfaceMesh.AddNormal(normal);
                index = isosurfaceMesh.IndexOfLastVertex;
            }

            if ((edgeDirection & 8) != 0)
            {
                cache.SetCachedCell(position, cacheVertexIndex, isosurfaceMesh.IndexOfLastVertex);
            }

            indices[i] = index;
        }

        // Process the indices
        for (int i = 0; i < cell.TriangleCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                isosurfaceMesh.AddIndex(indices[cell.Indices[i * 3 + j]]);
            }
        }
    }

    /// <summary>
    /// Generate a vertex from two density values.
    /// </summary>
    private static Vector3 GenerateVertex(Vector3Int offset, int value0, int value1, long alpha, int levelOfDistance)
    {
        Vector3 p0 = offset + VoxelTables.CubeVertexOffsets[value0] * levelOfDistance;
        Vector3 p1 = offset + VoxelTables.CubeVertexOffsets[value1] * levelOfDistance;
        return InterpolateVertex(p0, p1, alpha);
    }

    /// <summary>
    /// Interpolate two vertices based on an alpha value.
    /// </summary>
    private static Vector3 InterpolateVertex(Vector3 p0, Vector3 p1, long alpha)
    {
        const float s = 0.00390625F;
        long u = 0x0100 - alpha;

        Vector3 q = (p0 * alpha + p1 * u) * s;
        return q;
    }
}