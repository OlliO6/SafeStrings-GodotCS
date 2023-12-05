#if TOOLS
namespace SafeStrings.Editor;

using Godot;
using System;

public partial class AssociateSceneDialog : ConfirmationDialog
{
    private LineEdit _scenePathEdit, _scriptPathEdit;

    public string SelectedScenePath => _scenePathEdit.Text;

    public string SelectedScriptPath => _scriptPathEdit.Text;

    public override void _Ready()
    {
        // Construct the window
        Title = "Associate Scene To Script";
        Theme = Plugin.Instance.GetEditorInterface().GetBaseControl().Theme;

        this.WithChilds(
            new PanelContainer().WithChilds(
                new VBoxContainer()
                {
                    SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                }.WithChilds(
                    new HBoxContainer()
                    {
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                    }.WithChilds(
                        new Label()
                        {
                            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                            Text = "Scene Path:"
                        },
                        _scenePathEdit = new LineEdit()
                        {
                            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                            SizeFlagsStretchRatio = 2
                        }),
                    new HBoxContainer()
                    {
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                    }.WithChilds(
                        new Label()
                        {
                            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                            Text = "CS Script Path:"
                        },
                        _scriptPathEdit = new LineEdit()
                        {
                            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                            SizeFlagsStretchRatio = 2
                        }))));
    }

    public void Open()
    {
        PopupCenteredRatio(0.2f);

        TryGetWantedFiles(out string mayWantedScenePath, out string mayWantedScriptPath);

        _scenePathEdit.Text = mayWantedScenePath;
        _scriptPathEdit.Text = mayWantedScriptPath;
    }

    private void TryGetWantedFiles(out string scenePath, out string scriptPath)
    {
        Node root = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot();

        if (root == null)
        {
            scenePath = "";
            scriptPath = "";
            return;
        }

        scenePath = root.SceneFilePath;
        scriptPath = (root.GetScript().Obj as CSharpScript)?.ResourcePath;
    }
}

#endif