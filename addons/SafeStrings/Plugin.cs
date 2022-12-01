#if TOOLS
namespace SafeStrings.Editor;

using System;
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }

    private InputActionsGenerators inputActionsGen;
    private ResGenerator resGen;

    public override void _EnterTree()
    {
        Instance = this;

        AddToolMenuItem("Update SafeStrings", new Callable(this, MethodName.Update));
        Settings.InitSettings();

        inputActionsGen = new();
        inputActionsGen.Start();

        resGen = new();
        resGen.Start();
    }

    public override void _ExitTree()
    {
        Instance = null;
        RemoveToolMenuItem("Update SafeStrings");

        inputActionsGen?.Stop();
        inputActionsGen = null;

        resGen?.Stop();
        resGen = null;
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
        inputActionsGen = new();
        inputActionsGen.Start();

        resGen = new();
        resGen.Start();
    }

    private void Update()
    {
        inputActionsGen?.Update();
        resGen?.Update();
    }
}
#endif