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
                        _scenePathEdit = new LineEdit()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill,
                            SizeFlagsStretchRatio = 2
                        }),
                    new HBoxContainer()
                    {
                        SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill
                    }.WithChilds(
                        new Label()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill,
                            Text = "CS Script Path:"
                        },
                        _scriptPathEdit = new LineEdit()
                        {
                            SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill,
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
        scenePath = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot()?.SceneFilePath;
        scriptPath = "";

        string selectedPath = Plugin.Instance.GetEditorInterface().GetCurrentPath();

        if (selectedPath.EndsWith(".cs"))
        {
            scriptPath = selectedPath;
        }
    }
}

#endif