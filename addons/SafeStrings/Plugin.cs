#if TOOLS
namespace SafeStrings;

using System;
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }

    private InputActionsWriter inputActionsWatcher;
    private ResWriter resWriter;

    public override void _EnterTree()
    {
        Instance = this;

        AddToolMenuItem("Update SafeStrings", new Callable(this, MethodName.Update));

        inputActionsWatcher = new();
        inputActionsWatcher.Start();

        resWriter = new();
        resWriter.Start();
    }

    public override void _ExitTree()
    {
        Instance = null;
        RemoveToolMenuItem("Update SafeStrings");

        inputActionsWatcher?.Stop();
        inputActionsWatcher = null;

        resWriter?.Stop();
        resWriter = null;
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

        resWriter = new();
        resWriter.Start();
    }

    private void Update()
    {
        inputActionsWatcher?.Update();
        resWriter?.Update();
    }
}
#endif
