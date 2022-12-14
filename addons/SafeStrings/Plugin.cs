#if TOOLS
namespace SafeStrings.Editor;

using System;
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }

    private const int UpdateAllToolItemId = 1, GenerateRelUsingToolItemId = 2;

    private PopupMenu _toolMenu;

    private InputActionsGenerators _inputActionsGen;
    private ResGenerator _resGen;

    public override async void _EnterTree()
    {
        Instance = this;

        AddToolSubmenuItem("SafeStrings", _toolMenu = new());

        _toolMenu.AddItem("Update All", UpdateAllToolItemId);
        _toolMenu.AddItem("Generate Rel Using", GenerateRelUsingToolItemId);
        _toolMenu.SetItemShortcut(1, new Shortcut()
        {
            Events = new()
            {
                new InputEventKey()
                {
                    CtrlPressed=true,
                    Keycode = Key.R
                }
            }
        }, true);

        _toolMenu.IdPressed += id =>
        {
            switch (id)
            {
                case UpdateAllToolItemId:
                    UpdateAll();
                    break;

                case GenerateRelUsingToolItemId:
                    RelUsingGenerator.GenerateRelUsing(GetEditorInterface().GetCurrentPath().TrimPrefix("res://"));
                    break;
            }
        };

        Settings.InitSettings();

        _inputActionsGen = new();
        _resGen = new();

        // Wait to frames so file system can initialize or something like that
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        UpdateAll();
    }

    public override void _ExitTree()
    {
        Instance = null;
        RemoveToolMenuItem("SafeStrings");

        _inputActionsGen?.Stop();
        _inputActionsGen = null;

        _resGen?.Stop();
        _resGen = null;
    }

    public override void _Process(double delta)
    {
        if (Instance == null)
        {
            Instance = this;
            OnBuilded();
        }
    }

    public static string GetFileType(string path)
    {
        string gdType = Instance.GetEditorInterface().GetResourceFilesystem().GetFileType(path);

        if (gdType is not "" and not null)
            return $"Godot.{gdType}";

        return "Godot.Resource";
    }

    private void OnBuilded()
    {
        _inputActionsGen = new();
        _resGen = new();
        CallDeferred(MethodName.UpdateAll);
    }

    private void UpdateAll()
    {
        _inputActionsGen?.Update();
        _resGen?.Update();
    }
}
#endif