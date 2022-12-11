#if TOOLS
namespace SafeStrings.Editor;

using Godot;
using System;
using System.IO;
using System.Text;

public class SceneGenerator
{
    private const string GeneratedSceneAssociationsPath = "addons/SafeStrings/Generated/Scenes";

    private FileSystemWatcher watcher;

    public void Start()
    {
        watcher = new FileSystemWatcher();
        watcher.Path = Settings.GlobalRootPath;
        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = true;

        watcher.Filter = "*.tscn";

        watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

        watcher.Created += OnSceneChanged;
        watcher.Changed += OnSceneChanged;

        watcher.Deleted += (sender, e) =>
        {
            RemoveAssociation(ProjectSettings.LocalizePath(e.FullPath));
        };

        UpdateAll();
    }

    public void Stop()
    {
        watcher.EnableRaisingEvents = false;
        watcher = null;
    }

    private void RemoveAssociation(string scenePath)
    {
        if (!Settings.SceneAssociations.ContainsKey(scenePath))
            return;

        Settings.SceneAssociations.Remove(scenePath);

        // Delete file
    }

    private void OnSceneChanged(object sender, FileSystemEventArgs e)
    {
        UpdateScene(ProjectSettings.LocalizePath(e.FullPath));
    }

    public void UpdateAll()
    {
        foreach (string file in Directory.EnumerateFiles(GeneratedSceneAssociationsPath))
        {
            File.Delete(file);
        }

        foreach (string scene in Settings.SceneAssociations.Keys)
        {
            UpdateScene(scene);
        }
    }

    public void UpdateScene(string scenePath)
    {
        GD.Print("Update ", scenePath, ", Containing: ", Settings.SceneAssociations.ContainsKey(scenePath));
        if (!Settings.SceneAssociations.ContainsKey(scenePath))
            return;

        string scriptPath = Settings.SceneAssociations[scenePath];

        PackedScene scene = GD.Load(scenePath) as PackedScene;
        CSharpScript script = GD.Load(scriptPath) as CSharpScript;

        if (scene == null)
        {
            GD.PrintErr($"Scene Associations: Path '{scenePath}' is not valid scene.");
            return;
        }
        if (script == null)
        {
            GD.PrintErr($"Scene Associations: Path '{scenePath}' is not valid script.");
            return;
        }
        Utils.GetCsTypeFromScript(script, out string classNamespace, out string className);
    }
}

#endif