#if TOOLS
namespace SafeStrings;

using System;
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }

    private InputActionsWriter inputActionsWatcher;

    public override void _EnterTree()
    {
        Instance = this;

        AddToolMenuItem("SafeStringsFullUpdate", new Callable(this, MethodName.FullUpdate));

        inputActionsWatcher = new();
        inputActionsWatcher.Start();
    }

    public override void _ExitTree()
    {
        Instance = null;
        RemoveToolMenuItem("SafeStringsFullUpdate");

        inputActionsWatcher?.Stop();
        inputActionsWatcher = null;
    }

    public override void _Process(double delta)
    {
        if (Instance == null)
        {
            Instance = this;
            OnBuilded();
        }
    }

    private void OnBuilded()
    {
        inputActionsWatcher = new();
        inputActionsWatcher.Start();
    }

    private void FullUpdate()
    {
        inputActionsWatcher?.FullUpdate();
    }
}
#endif
