#if TOOLS
namespace SafeStrings.Editor;

using System.IO;
using System.Linq;
using System.Text;
using Godot;

public class ResGenerator
{
    private FileSystemWatcher watcher;

    public void Start()
    {
        watcher = new FileSystemWatcher();
        watcher.Path = Settings.GlobalRootPath;
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

        watcher.Created += OnSomethingChanged;
        watcher.Deleted += OnSomethingChanged;
        watcher.Renamed += OnSomethingChanged;

        Update();
    }

    private void OnSomethingChanged(object sender, FileSystemEventArgs e)
    {
        var relativePath = e.Name.Replace('\\', '/');
        var name = e.Name.GetFile();

        // make sure nothing starts with '.'
        if (name.StartsWith('.') || e.Name.StartsWith('.') || relativePath.Contains("/."))
            return;

        var extension = name.GetExtension();

        if (extension != "" && extension != name && Settings.ExcludedExtensions.Any(excludedExt => extension == excludedExt) ||
            Settings.ExcludedFolers.Any(excludedPath => relativePath.StartsWith(excludedPath)))
            return;

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
            .Append("public static class Res\n")
            .Append("{\n");

        var root = DirAccess.Open("res://");

        AppendDir(sb, root, "    ");

        sb.Append("}");

        File.WriteAllText("addons/SafeStrings/Generated/Res.g.cs", sb.ToString(), Encoding.UTF8);
    }

    private void AppendDir(StringBuilder sb, DirAccess dir, string indent)
    {
        foreach (var file in dir.GetFiles())
        {
            if (file.StartsWith('.'))
                continue;

            var extension = file.GetExtension();

            if (extension != "" && extension != file &&
                Settings.ExcludedExtensions.Any(excludedExt => extension == excludedExt))
                continue;

            string filePath = dir.GetCurrentDir().PathJoin(file);
            string fileType = Plugin.GetFileType(filePath);

            sb.Append(indent)
                .Append("public static readonly ResourcePath")
                .Append("<")
                .Append(fileType)
                .Append("> ")
                .Append(((char.IsLetter(file.First()) || file.StartsWith('_')) ? file : file.Insert(0, "_")).Replace('.', '_').Replace(' ', '_'))
                .Append(" = \"")
                .Append(filePath)
                .Append("\";\n");
        }

        foreach (var subDir in dir.GetDirectories())
        {
            if (subDir.StartsWith('.'))
                continue;

            var pathToSubDir = dir.GetCurrentDir().PathJoin(subDir);

            if (Settings.ExcludedFolers.Any(excludedPath => pathToSubDir == "res://" + excludedPath))
                continue;

            sb.Append(indent)
                .Append("public static class ")
                .Append(((char.IsLetter(subDir.First()) || subDir.StartsWith('_')) ? subDir : subDir.Insert(0, "_")).Replace('.', '_').Replace(' ', '_'))
                .Append("\n")
                .Append(indent)
                .Append("{\n");

            AppendDir(sb, DirAccess.Open(pathToSubDir), indent + "    ");

            sb.Append(indent)
                .Append("}\n");
        }
    }
}

#endif