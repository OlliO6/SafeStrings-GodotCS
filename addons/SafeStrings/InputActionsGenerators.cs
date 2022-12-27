#if TOOLS
namespace SafeStrings.Editor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Godot;

public class InputActionsGenerators
{
    private FileSystemWatcher watcher;

    public InputActionsGenerators()
    {
        watcher = new FileSystemWatcher();
        watcher.Path = ProjectSettings.GlobalizePath("res://");
        watcher.Filter = "project.godot";

        watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = false;

        watcher.Changed += OnProjectChanged;
    }

    private void OnProjectChanged(object sender, FileSystemEventArgs e)
    {
        Callable.From(Update).CallDeferred();
    }

    public void Stop()
    {
        watcher.EnableRaisingEvents = false;
        watcher = null;
    }

    public void Update()
    {
        ConfigFile project = new();
        var error = project.Load("res://project.godot");

        if (error != Error.Ok)
        {
            GD.PrintErr("'project.godot' couldn't be loaded.");
            return;
        }

        List<string> actionNames = new();

        if (project.HasSection("input"))
            actionNames.AddRange(project.GetSectionKeys("input"));

        var sb = new StringBuilder()
            .Append("namespace SafeStrings;\n\n")
            .Append("using Godot;\n\n")
            .Append("public static partial class InputAction\n")
            .Append("{\n");

        foreach (var action in actionNames)
        {
            sb.Append("    public static readonly StringName ")
                .Append(action.ToPascalCase())
                .Append(" = \"")
                .Append(action)
                .Append("\";\n");
        }

        sb.Append("}");

        File.WriteAllText("addons/SafeStrings/Generated/InputActions.g.cs", sb.ToString(), Encoding.UTF8);
    }
}

#endif
