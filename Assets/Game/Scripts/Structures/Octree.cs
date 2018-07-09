public class Octree<T>
{
    public Octree<T>[] Children { get; set; }
    public T Value { get; set; } = default(T);
}
