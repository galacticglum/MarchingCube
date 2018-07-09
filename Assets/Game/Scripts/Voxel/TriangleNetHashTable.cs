using System.Collections.Generic;

public class TriangleNetHashTable
{
    public int CurrentLayerIndex { get; set; }

    private readonly int stx;
    private readonly int sty;
    private readonly int width;
    private readonly int height;

    private readonly List<int[,,]> map;

    /// <summary>
    /// Initializes a new <see cref="TriangleNetHashTable"/>.
    /// </summary>
    public TriangleNetHashTable(int minimumX, int minimumY, int width, int height)
    {
        stx = minimumX - 1;
        sty = minimumY - 1;

        this.width = width + 2;
        this.height = height + 2;

        map = new List<int[,,]>(2)
        {
            new int[this.width, this.height, 3],
            new int[this.width, this.height, 3]
        };

        InitializeMap(0);
        InitializeMap(1);
    }

    /// <summary>
    /// Initializes the map at the specified index
    /// with the default value of -1.
    /// </summary>
    public void InitializeMap(int index)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[index][i, j, 0] = -1;
                map[index][i, j, 1] = -1;
                map[index][i, j, 2] = -1;
            }
        }
    }

    /// <summary>
    /// Increases the index by 1.
    /// </summary>
    public void IncrementIndex()
    {
        CurrentLayerIndex++;
        InitializeMap(0);

        // Swap the map at index 0 with the map at index 1
        int[,,] temp = map[0];
        map[0] = map[1];
        map[1] = temp;
    }

    public int Get(int x, int y, int z, int d) => map[z - CurrentLayerIndex][x - stx, y - sty, d];
    public void Set(int x, int y, int z, int d, int value) => map[z - CurrentLayerIndex][x - stx, y - stx, d] = value;
}