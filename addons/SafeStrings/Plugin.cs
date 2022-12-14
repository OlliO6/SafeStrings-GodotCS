#if TOOLS
namespace SafeStrings.Editor;

using System;
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }

    private const int UpdateAllToolItemId = 1, GenerateRelUsingToolItemId = 2, AssociateSceneToScriptToolItemId = 3;

    private PopupMenu _toolMenu;
    private AssociateSceneDialog _associateSceneDialog;
    private InputActionsGenerators _inputActionsGen;
    private ResGenerator _resGen;
    private SceneGenerator _sceneGen;

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
                    CtrlPressed = true,
                    AltPressed = true,
                    Keycode = Key.R
                }
            }
        }, true);
        _toolMenu.AddItem("Associate Scene To Script", AssociateSceneToScriptToolItemId);
        _toolMenu.SetItemShortcut(2, new Shortcut()
        {
            Events = new()
            {
                new InputEventKey()
                {
                    CtrlPressed = true,
                    AltPressed = true,
                    Keycode = Key.S
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

                case AssociateSceneToScriptToolItemId:
                    _associateSceneDialog.Open();
                    break;
            }
        };

        AddChild(_associateSceneDialog = new());
        _associateSceneDialog.Confirmed += OnAssociateSceneDialogConfirmed;

        Settings.InitSettings();

        _inputActionsGen = new();
        _resGen = new();
        _sceneGen = new();

        // Wait to frames so file system can initialize or something like that
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        UpdateAll();
    }

    public override void _ExitTree()
    {
        Instance = null;
        RemoveToolMenuItem("SafeStrings");

        _associateSceneDialog?.QueueFree();

        _inputActionsGen?.Stop();
        _inputActionsGen = null;

        _resGen?.Stop();
        _resGen = null;

        _sceneGen?.Stop();
        _sceneGen = null;
    }

    public override void _Process(double delta)
    {
        if (Instance == null)
        {
            Instance = this;
            CallDeferred(MethodName.OnBuilded);
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
        if (IsInstanceValid(_associateSceneDialog))
            _associateSceneDialog.QueueFree();

        AddChild(_associateSceneDialog = new());
        _associateSceneDialog.Confirmed += OnAssociateSceneDialogConfirmed;

        _inputActionsGen = new();
        _resGen = new();
        _sceneGen = new();
        CallDeferred(MethodName.UpdateAll);
    }

    void OnAssociateSceneDialogConfirmed()
    {
        string scenePath = _associateSceneDialog.SelectedScenePath;
        string scriptPath = _associateSceneDialog.SelectedScriptPath;

        if (scenePath.GetExtension() != "tscn")
        {
            GD.PrintErr("Scene needs to be a tscn file.");
            return;
        }
        if (scriptPath.GetExtension() != "cs")
        {
            GD.PrintErr("Script needs to be a cs file.");
            return;
        }

        AddSceneAssiciation(scenePath, scriptPath);
    }

    public void AddSceneAssiciation(string scenePath, string scriptPath)
    {
        Settings.AddSceneAssociation(scenePath, scriptPath);
        _sceneGen.UpdateScene(scenePath);
    }

    private void UpdateAll()
    {
        _inputActionsGen?.Update();
        _resGen?.Update();
        _sceneGen?.UpdateAll();
    }
}

#endif