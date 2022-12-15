namespace SafeStrings;

using System;
using System.Threading.Tasks;
using Godot;

public class ResourcePath<T>
    where T : class
{
    private string _path;
    private T _strongValue;
    private WeakReference _weakValue;

    public T StrongValue => _strongValue ??= GD.Load<T>(_path);

    public T Value
    {
        get
        {
            if (_weakValue == null || !_weakValue.IsAlive)
                _weakValue = new WeakReference(GD.Load(_path));

            return _weakValue.Target as T;
        }
    }

    public ResourcePath(string path)
    {
        _path = path;
    }

    public T Load(bool setWeakCache = true, bool setStrongCache = false)
    {
        T value = GD.Load<T>(_path);

        if (setWeakCache)
            _weakValue = new(value);

        if (setStrongCache)
            _strongValue = value;

        return value;
    }

    public async Task<T> PreloadAsync(bool setWeakCache = true, bool setStrongCache = false, int updateDelayMSec = 50, bool useSubThreads = false)
    {
        ResourceLoader.LoadThreadedRequest(this);

        while (ResourceLoader.LoadThreadedGetStatus(this) == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            await Task.Delay(updateDelayMSec);
        }

        if (ResourceLoader.LoadThreadedGetStatus(this) != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            GD.PushError($"Resource could not be loaded. Path: '{_path}', Error: {ResourceLoader.LoadThreadedGetStatus(this)}");
            return null;
        }

        T value = ResourceLoader.LoadThreadedGet(this) as T;

        if (setWeakCache)
            _weakValue = new(value);

        if (setStrongCache)
            _strongValue = value;

        return value;
    }

    public void ReleaseStrongValue()
    {
        _strongValue = null;
    }

    public override string ToString() => _path;

    public static implicit operator string(ResourcePath<T> from) => from._path;
    public static implicit operator ResourcePath<T>(string from) => new ResourcePath<T>(from);
    public static implicit operator T(ResourcePath<T> from) => from.Value;
}
