namespace SafeStrings;

using System;
using System.Threading.Tasks;
using Godot;

public class ResourcePath<T>
    where T : class
{
    private string _path;
    private T _value;
    private WeakReference<T> _weakValue;

    public T Value => _value ??= GD.Load<T>(_path);

    public WeakReference<T> WeakValue => _weakValue ??= new WeakReference<T>(GD.Load<T>(_path));

    public ResourcePath(string path)
    {
        _path = path;
    }

    public void Preload() => _value = GD.Load<T>(_path);

    public async Task PreloadAsync(int updateDelayMSec = 50, bool useSubThreads = false)
    {
        if (_value != null)
            return;

        ResourceLoader.LoadThreadedRequest(this);

        while (ResourceLoader.LoadThreadedGetStatus(this) == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            await Task.Delay(updateDelayMSec);
        }

        if (ResourceLoader.LoadThreadedGetStatus(this) != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            GD.PushError($"Resource could not be loaded. Path: '{_path}', Error: {ResourceLoader.LoadThreadedGetStatus(this)}");
            return;
        }

        _value = ResourceLoader.LoadThreadedGet(this) as T;
    }

    public void ReleaseValue()
    {
        _value = null;
        _weakValue = null;
    }

    public override string ToString() => _path;

    public static implicit operator string(ResourcePath<T> from) => from._path;
    public static implicit operator ResourcePath<T>(string from) => new ResourcePath<T>(from);
    public static implicit operator T(ResourcePath<T> from) => from.Value;
}
