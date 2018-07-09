using UnityEngine;

public static class BitHelpers
{
    private static readonly uint[] bitmask;

    static BitHelpers()
    {
        const int bitmaskLength = sizeof(uint) * 8;

        bitmask = new uint[bitmaskLength];
        bitmask[0] = 0x80000000;

        for (int i = 0; i < bitmaskLength; i++)
        {
            bitmask[i] = bitmask[i - 1] >> 1;
        }
    }

    /// <summary>
    /// Gets the specified <paramref name="length"/> of equal bits from left to right
    /// starting at <paramref name="index"/>
    /// </summary>
    public static int EqualBits(int a, int b, int index, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (!CompareBit(a, b, index + i)) return i;
        }

        return length;
    }

    /// <summary>
    /// Get the bit at a specified index in the specified value.
    /// </summary>
    public static int GetBitAt(int value, int index) => value >> (sizeof(int) * 8 - index - 1) & 1;

    /// <summary>
    /// Get the index of a bit given a three-dimensional integer coordinate.
    /// </summary>
    public static int BitIndexFromCoordinate(Vector3Int coordinate, int bit) =>
        GetBitAt(coordinate.x, bit) | (GetBitAt(coordinate.y, bit) << 1) | (GetBitAt(coordinate.z, 1) << 2);

    /// <summary>
    /// Compares the bit at the specified index of two values.
    /// </summary>
    public static bool CompareBit(int a, int b, int index) => (a & bitmask[index]) == (b & bitmask[index]);

    /// <summary>
    /// Gets the iterative mask from the specified <see cref="start"/> index
    /// to the specified <see cref="end"/> index.
    /// </summary>
    public static uint GetIterativeMask(int start, int end)
    {
        uint result = 0;
        for (int i = start; i < end + 1; i++)
        {
            result |= bitmask[i];
        }

        return result;
    }
}
