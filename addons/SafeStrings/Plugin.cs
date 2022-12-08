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
    private ConfirmationDialog _sceneAssociateDialog;
    private InputActionsGenerators _inputActionsGen;
    private ResGenerator _resGen;

    public override void _EnterTree()
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
                    Update();
                    break;

                case GenerateRelUsingToolItemId:
                    RelUsingGenerator.GenerateRelUsing(GetEditorInterface().GetCurrentPath().TrimPrefix("res://"));
                    break;

                case AssociateSceneToScriptToolItemId:
                    OpenSceneAssociateDialog();
                    break;
            }
        };

        _sceneAssociateDialog = new ConfirmationDialog()
        {
            Title = "Associate Scene To Script",
            Theme = GetEditorInterface().GetBaseControl().Theme
        }.WithChilds(
            new PanelContainer().WithChilds(
                new VBoxContainer()
                {
                    SizeFlagsVertical = (int)Control.SizeFlags.ExpandFill,
                    SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill
                }.WithChilds(
                    new HBoxContainer()
                    {
                        SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill
                    }.WithChilds(
                        new Label()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill,
                            Text = "Scene Path:"
                        },
                        new LineEdit()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill
                        }
                    ),
                    new HBoxContainer()
                    {
                        SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill
                    }.WithChilds(
                        new Label()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill,
                            Text = "CS Script Path:"
                        },
                        new LineEdit()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill
                        }
                    )
                )
            )
        );

        AddChild(_sceneAssociateDialog);

        _sceneAssociateDialog.Confirmed += () =>
            {
                GD.Print("Confirmed");
            };

        Settings.InitSettings();

        _inputActionsGen = new();
        _inputActionsGen.Start();

        _resGen = new();
        _resGen.Start();
    }

    public override void _ExitTree()
    {
        Instance = null;
        RemoveToolMenuItem("SafeStrings");

        _sceneAssociateDialog?.QueueFree();

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
        _inputActionsGen.Start();

        _resGen = new();
        _resGen.Start();
    }

    private void OpenSceneAssociateDialog()
    {
        _sceneAssociateDialog.PopupCenteredRatio(0.2f);
    }

    private void Update()
    {
        _inputActionsGen?.Update();
        _resGen?.Update();
    }
}

#endif