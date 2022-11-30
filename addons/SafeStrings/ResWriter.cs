#if TOOLS
namespace SafeStrings;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Godot;

public class ResWriter
{
    private FileSystemWatcher watcher;

    public void Start()
    {
        watcher = new FileSystemWatcher();
        watcher.Path = ProjectSettings.GlobalizePath("res://");
        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = true;

        watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

        watcher.Created += (_, _) => Update();
        watcher.Deleted += (_, _) => Update();
        watcher.Renamed += (_, _) => Update();

        Update();
    }

    public void Stop()
    {
        watcher.EnableRaisingEvents = false;
        watcher = null;
    }

    public void Update()
    {
        var sb = new StringBuilder()
            .Append("namespace SafeStrings;\n\n")
            .Append("using Godot;\n\n")
            .Append("public static class Res\n")
            .Append("{\n");

        var root = DirAccess.Open("res://");

        AppendDir(sb, root, "    ");

        sb.Append("}");

        File.WriteAllText("addons/SafeStrings/Generated/Res.g.cs", sb.ToString(), Encoding.UTF8);
    }

    private void AppendDir(StringBuilder sb, DirAccess dir, string indent)
    {
        dir.IncludeHidden = false;
        dir.IncludeNavigational = false;

        foreach (var file in dir.GetFiles())
        {
            if (file.StartsWith('.'))
                continue;

            sb.Append(indent)
                .Append("public static readonly StringName ")
                .Append(((char.IsLetter(file.First()) || file.StartsWith('_')) ? file : file.Insert(0, "_")).Replace('.', '_').Replace(' ', '_'))
                .Append(" = \"")
                .Append(dir.GetCurrentDir().PathJoin(file))
                .Append("\";\n");
        }

        foreach (var subDir in dir.GetDirectories())
        {
            if (subDir.StartsWith('.'))
                continue;

            sb.Append(indent)
                .Append("public static class ")
                .Append(((char.IsLetter(subDir.First()) || subDir.StartsWith('_')) ? subDir : subDir.Insert(0, "_")).Replace('.', '_').Replace(' ', '_'))
                .Append("\n")
                .Append(indent)
                .Append("{\n");

            AppendDir(sb, DirAccess.Open(dir.GetCurrentDir().PathJoin(subDir)), indent + "    ");

            sb.Append(indent)
                .Append("}\n");
        }
    }
}

#endif