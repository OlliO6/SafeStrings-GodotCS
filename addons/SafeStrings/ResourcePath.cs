using Godot;

public class ResourcePath<T>
    where T : class
{
    private string _path;
    private T _value;

    public T Value => _value;

    public ResourcePath(string path)
    {
        _path = path;
        _value = GD.Load<T>(_path);
    }

    public static implicit operator string(ResourcePath<T> from) => from._path;
    public static implicit operator ResourcePath<T>(string from) => new ResourcePath<T>(from);
}
