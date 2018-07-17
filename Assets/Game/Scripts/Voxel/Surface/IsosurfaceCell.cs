public struct IsosurfaceCell
{
    public byte[] Indices { get; }

    public int VertexCount => geometryCount >> 4;
    public int TriangleCount => geometryCount & 0x0F;

    private readonly byte geometryCount;

    public IsosurfaceCell(byte geometryCount, byte[] indices)
    {
        this.geometryCount = geometryCount;
        Indices = indices;
    }
}
