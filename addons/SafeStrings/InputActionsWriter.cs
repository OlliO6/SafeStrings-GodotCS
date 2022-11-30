#if TOOLS
namespace SafeStrings;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Godot;

public class InputActionsWriter
{
    private FileSystemWatcher watcher;

    public void Start()
    {
        watcher = new FileSystemWatcher();
        watcher.Path = ProjectSettings.GlobalizePath("res://");
        watcher.Filter = "project.godot";

        FullUpdate();
    }

    public void Stop()
    {
        watcher = null;
    }

    public void FullUpdate()
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

        StringBuilder sb = new();
        sb.Append("namespace SafeStrings;\n\n")
            .Append("using Godot;\n\n")
            .Append("public static partial class InputAction\n")
            .Append("{\n");

        foreach (var action in actionNames)
        {
            sb.Append("    public static readonly StringName ")
                .Append(action.ToPascalCase())
                .Append(" = ")
                .Append(action)
                .Append(";\n");
        }

        sb.Append("}");

        GD.Print(sb.ToString());
    }
}

#endif