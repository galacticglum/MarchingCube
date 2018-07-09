using UnityEngine;

public class IsosurfaceProcessor
{
    private static readonly Vector3Int[] PointIndexToDelta =
    {
        new Vector3Int(0, 1, 1),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 0, 1)
    };

    private static readonly byte[] PointIndexToFlag =
    {
        1 << 0,
        1 << 1,
        1 << 2,
        1 << 3,
        1 << 4,
        1 << 5,
        1 << 6,
        1 << 7
    };

    private static readonly int[,] EdgeVertexIndex =
    {
        {0, 1}, {1, 2},
        {2, 3}, {3, 0},
        {4, 5}, {5, 6},
        {6, 7}, {7, 4},
        {0, 4}, {1, 5},
        {2, 6}, {3, 7}
    };

    private static readonly IntQuad[] CubeEdgeMapTable =
    {
        new IntQuad(0,1,0,1),
        new IntQuad(0,0,0,0),
        new IntQuad(0,0,0,1),
        new IntQuad(0,0,1,0),
            
        new IntQuad(1,1,0,1),
        new IntQuad(1,0,0,0),
        new IntQuad(1,0,0,1),
        new IntQuad(1,0,1,0),
            
        new IntQuad(0,1,1,2),
        new IntQuad(0,1,0,2),
        new IntQuad(0,0,0,2),
        new IntQuad(0,0,1,2)
    };

    public float Isolevel { get; set; }

    private float[] data;
    private int width;
    private int height;
    private int depth;

    public IsosurfaceMesh Generate(float[] data, int width, int height, int depth)
    {
        this.data = data;
        this.width = width;
        this.height = height;
        this.depth = depth;

        IsosurfaceMesh mesh = new IsosurfaceMesh();
        Triangle[] triangles = new Triangle[6];
        TriangleNetHashTable triangleHashTable = new TriangleNetHashTable(0, 0, width, height);

        for (int k = 0; k <= depth - 1; k++)
        {
            for (int j = 0; j <= height - 1; j++)
            {
                for (int i = 0; i <= width - 1; i++)
                {
                    byte edgeFlag = 0;
                    for (int v = 0; v < 8; v++)
                    {
                        int ix = i + PointIndexToDelta[v].x;
                        int iy = j + PointIndexToDelta[v].y;
                        int iz = k + PointIndexToDelta[v].z;

                        if (PointInRange(ix, iy, iz) && IsInsideIsoSurface(ix, iy, iz))
                        {
                            edgeFlag |= PointIndexToFlag[v];
                        }
                    }

                    if (edgeFlag == 0) continue;

                    int triangleCount = ExtractTriangles(edgeFlag, i, j, k, triangles);
                    for (int triangleIndex = 0; triangleIndex < triangleCount; triangleIndex++)
                    {
                        MergeTriangleIntoMesh(mesh, triangleHashTable, triangles[triangleIndex]);
                    }
                }
            }

            triangleHashTable.IncrementIndex();
        }

        return mesh;
    }

    private static int ExtractTriangles(byte value, int i, int j, int k, Triangle[] result)
    {
        int triangleCount = 0;
        if (IsosurfaceTable.TriangleConnectionTable[value, 0] == -1) return triangleCount;
        
        int index = 0;
        while (IsosurfaceTable.TriangleConnectionTable[value, index] != -1)
        {
            int e0index = IsosurfaceTable.TriangleConnectionTable[value, index];
            int e1index = IsosurfaceTable.TriangleConnectionTable[value, index + 1];
            int e2index = IsosurfaceTable.TriangleConnectionTable[value, index + 2];

            result[triangleCount] = new Triangle(new Vector3Int(i, j, k), e0index, e1index, e2index);
            triangleCount++;
            index += 3;
        }

        return triangleCount;
    }

    private void MergeTriangleIntoMesh(IsosurfaceMesh mesh, TriangleNetHashTable triangleTable, Triangle triangle)
    {
        int e0i = CubeEdgeMapTable[triangle.Edge0].D;
        int p0x = triangle.Point.x + CubeEdgeMapTable[triangle.Edge0].A;
        int p0y = triangle.Point.y + CubeEdgeMapTable[triangle.Edge0].B;
        int p0z = triangle.Point.z + CubeEdgeMapTable[triangle.Edge0].C;

        int e1i = CubeEdgeMapTable[triangle.Edge1].D;
        int p1x = triangle.Point.x + CubeEdgeMapTable[triangle.Edge1].A;
        int p1y = triangle.Point.y + CubeEdgeMapTable[triangle.Edge1].B;
        int p1z = triangle.Point.z + CubeEdgeMapTable[triangle.Edge1].C;

        int e2i = CubeEdgeMapTable[triangle.Edge2].D;
        int p2x = triangle.Point.x + CubeEdgeMapTable[triangle.Edge2].A;
        int p2y = triangle.Point.y + CubeEdgeMapTable[triangle.Edge2].B;
        int p2z = triangle.Point.z + CubeEdgeMapTable[triangle.Edge2].C;

        int p0i;
        int p1i;
        int p2i;
        int index = triangleTable.Get(p0x, p0y, p0z, e0i);
        if (index == -1)
        {
            Vector3 interp = GetIntersection(triangle.Point.x, triangle.Point.y, triangle.Point.z, triangle.Edge0);
            p0i = mesh.AddVertex(interp);
            triangleTable.Set(p0x, p0y, p0z, e0i, p0i);
        }
        else
        {
            p0i = index;
        }

        index = triangleTable.Get(p1x, p1y, p1z, e1i);
        if (index == -1)
        {
            Vector3 interp = GetIntersection(triangle.Point.x, triangle.Point.y, triangle.Point.z, triangle.Edge1);
            p1i = mesh.AddVertex(interp);
            triangleTable.Set(p1x, p1y, p1z, e1i, p1i);
        }
        else
        {
            p1i = index;
        }

        index = triangleTable.Get(p2x, p2y, p2z, e2i);
        if (index == -1)
        {
            Vector3 interp = GetIntersection(triangle.Point.x, triangle.Point.y, triangle.Point.z, triangle.Edge2);
            p2i = mesh.AddVertex(interp);
            triangleTable.Set(p2x, p2y, p2z, e2i, p2i);
        }
        else
        {
            p2i = index;
        }

        Face face = new Face(p0i, p1i, p2i);
        mesh.AddFace(face);
    }

    private Vector3 GetIntersection(int cx, int cy, int cz, int ei)
    {
        int p0i = EdgeVertexIndex[ei, 0];
        int p1i = EdgeVertexIndex[ei, 1];

        int p0X = cx + PointIndexToDelta[p0i].x;
        int p0Y = cy + PointIndexToDelta[p0i].y;
        int p0Z = cz + PointIndexToDelta[p0i].z;

        int p1X = cx + PointIndexToDelta[p1i].x;
        int p1Y = cy + PointIndexToDelta[p1i].y;
        int p1Z = cz + PointIndexToDelta[p1i].z;
        float v0, v1;

        if (PointInRange(p0X, p0Y, p0Z))
        {
            int n = p0X + p0Y * width + p0Z * width * height;
            v0 = data[n];
        }
        else
        {
            v0 = 0;
        }

        if (PointInRange(p1X, p1Y, p1Z))
        {
            int n = p1X + p1Y * width + p1Z * width * height;
            v1 = data[n];
        }
        else
        {
            v1 = 0;
        }

        float lmb = (v0 - Isolevel) / (v0 - v1);
        float x = p1X * lmb + (1 - lmb) * p0X;
        float y = p1Y * lmb + (1 - lmb) * p0Y;
        float z = p1Z * lmb + (1 - lmb) * p0Z;

        return new Vector3(x, y, z);
    }

    private bool IsInsideIsoSurface(int x, int y, int z) => data[x + width * y + width * height * z] <= Isolevel;
    private bool PointInRange(int x, int y, int z) => x < width && x >= 0 && y < height && y >= 0 && z < depth && z >= 0;
}
