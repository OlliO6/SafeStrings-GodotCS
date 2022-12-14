namespace SafeStrings;

using Godot;
using System.Runtime.CompilerServices;

public class SceneNodePath<T>
    where T : class
{
    public ConditionalWeakTable<Node, T> _cache = new();

    private NodePath _path;

    public SceneNodePath(NodePath path)
    {
        _path = path;
    }

    public T Get(Node root) => root.GetNode<T>(_path);

    public T GetCached(Node root)
    {
        if (_cache.TryGetValue(root, out T result))
            return result;

        _cache.Add(root, Get(root));

        return GetCached(root);
    }

    public System.Type GetNodeType() => typeof(T);

    public static implicit operator NodePath(SceneNodePath<T> from) => from._path;
    public static implicit operator SceneNodePath<T>(NodePath from) => new SceneNodePath<T>(from);
    public static implicit operator SceneNodePath<T>(string from) => new SceneNodePath<T>(from);
}
