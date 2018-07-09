using System;
using System.Linq;
using UnityEngine;

public class OctreeNode<T>
{
    public int Depth => relativeDepth + depthOffset;
    public int LevelOfDistance => 32 - Depth;
    public int Size => 1 << LevelOfDistance;
    public bool HasChildren => Children.Any(node => node != null);

    public Vector3Int Position
    {
        get
        {
            int mask = (int)BitHelpers.GetIterativeMask(0, relativeDepth - 1);
            int x = relativePoint.x & mask;
            int y = relativePoint.y & mask;
            int z = relativePoint.z & mask;

            return new Vector3Int(x, y, z);
        }
    }

    public Vector3Int Centre
    {
        get
        {
            int halfSize = Size / 2;
            return new Vector3Int(halfSize, halfSize, halfSize);
        }
    }

    public T Value { get; private set; }
    public OctreeNode<T> Parent { get; private set; }
    public OctreeNode<T>[] Children { get; }

    private readonly Vector3Int relativePoint;
    private int relativeDepth;

    private int depthOffset;

    public OctreeNode(OctreeNode<T> parent, Vector3Int relativePoint)
    {
        Parent = parent;
        this.relativePoint = relativePoint;

        Children = new OctreeNode<T>[8];

        relativeDepth = 0;
        depthOffset = 0;
    }

    /// <summary>
    /// Initializes a child <see cref="OctreeNode{T}"/> at the specified
    /// index given a coordinate.
    /// </summary>
    public OctreeNode<T> InitializeChild(int index, Vector3Int coordinate)
    {
        OctreeNode<T> child = new OctreeNode<T>(this, coordinate)
        {
            Value = default(T)
        };

        SetChild(index, child);
        return child;
    }

    public OctreeNode<T> InitializeLeaf(int index, Vector3Int coordinate)
    {
        OctreeNode<T> leaf = InitializeChild(index, coordinate);

        leaf.relativeDepth = relativeDepth + depthOffset + 1;
        leaf.depthOffset = 32 - leaf.relativeDepth;
        return leaf;
    }

    /// <summary>
    /// Get the child <see cref="OctreeNode{T}"/> at the specified <see cref="coordinate"/>
    /// starting from the <paramref name="minimumDepth"/>.
    /// </summary>
    public OctreeNode<T> Get(Vector3Int coordinate, int minimumDepth)
    {
        // We have to go at least deeper than this node
        // That is, we have to AT LEAST go to the depth
        // of our children. 
        //Thus, let's return the instance to this node 
        if (Depth == minimumDepth) return this;

        int equalDepthOffset = GetEqualDepthOffset(coordinate);
        int currentDepth = relativeDepth;

        if (equalDepthOffset == depthOffset)
        {
            currentDepth += depthOffset;
        }
        else
        {
            // If we reach this point, that means there is no
            // possible way we can reach the minimum depth
            // that was requested---return null.
            return null;
        }

        int childIndex = BitHelpers.BitIndexFromCoordinate(coordinate, currentDepth);
        if (!HasChildren || Children[childIndex] == null) return null;

        return Children[childIndex].Get(coordinate, minimumDepth);
    }

    /// <summary>
    /// Sets the value of a child <see cref="OctreeNode{T}"/> at the specified
    /// <paramref name="coordinate"/> starting from the <paramref name="minimumDepth"/>.
    /// </summary>
    public void Set(Vector3Int coordinate, T value, int minimumDepth)
    {
        int equalthDepthOffset = GetEqualDepthOffset(coordinate);
        if (equalthDepthOffset == depthOffset)
        {
            if (Depth == minimumDepth)
            {
                Value = value;
                return;
            }

            int index = BitHelpers.BitIndexFromCoordinate(coordinate, Depth);
            if (!HasChildren || Children[index] == null)
            {
                OctreeNode<T> leaf = InitializeLeaf(index, coordinate);
                leaf.Value = value;
            }
            else
            {
                Children[index].Set(coordinate, value, minimumDepth);
            }
        }
        else
        {
            OctreeNode<T> node = Parent.InitializeChild(Parent.GetChildIndex(this), coordinate);
            node.depthOffset = equalthDepthOffset;
            node.relativeDepth = relativeDepth;

            int index = BitHelpers.BitIndexFromCoordinate(coordinate, node.Depth);
            OctreeNode<T> leaf = InitializeLeaf(index, coordinate);
            leaf.Value = value;
        }
    }

    /// <summary>
    /// Gets the number of equal bits between the relative depth of
    /// this <see cref="OctreeNode{T}"/> and the depth offset of this
    /// <see cref="OctreeNode{T}"/> given a coordinate.
    /// </summary>
    private int GetEqualDepthOffset(Vector3Int coordinate)
    {
        int ex = BitHelpers.EqualBits(relativePoint.x, coordinate.x, relativeDepth, depthOffset);
        int ey = BitHelpers.EqualBits(relativePoint.y, coordinate.y, relativeDepth, depthOffset);
        int ez = BitHelpers.EqualBits(relativePoint.z, coordinate.z, relativeDepth, depthOffset);

        return Mathf.Min(ex, ey, ez);
    }

    /// <summary>
    /// Sets the child value of this <see cref="OctreeNode{T}"/>
    /// at a given index.
    /// </summary>
    /// <exception cref="NullReferenceException">The <paramref name="child"/> node was null.</exception>
    private void SetChild(int index, OctreeNode<T> child)
    {
        if (child == null)
        {
            throw new NullReferenceException($"OctreeNode<{nameof(T)}>::SetChild: child node was null.");
        }

        Children[index] = child;
        child.Parent = this;
    }

    /// <summary>
    /// Gets the index of a child node.
    /// </summary>
    public int GetChildIndex(OctreeNode<T> node) => Array.IndexOf(Children, node);

    /// <summary>
    /// Executes an operation on each of the children <see cref="OctreeNode{T}"/>.
    /// </summary>
    public void ForeachChild(Action<OctreeNode<T>> operation)
    {
        if (!HasChildren) return;
        foreach (OctreeNode<T> child in Children)
        {
            operation(child);
        }
    }
}
