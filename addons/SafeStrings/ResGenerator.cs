#if TOOLS
namespace SafeStrings.Editor;

using System.IO;
using System.Linq;
using System.Text;
using Godot;

public class ResGenerator
{
    private FileSystemWatcher watcher;

    public ResGenerator()
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
        string[] validFileNames = dir.GetFiles().Where(fileName =>
        {
            if (fileName.StartsWith('.'))
                return false;

            var extension = fileName.GetExtension();

            if (extension != "" && extension != fileName &&
                Settings.ExcludedExtensions.Any(excludedExt => extension == excludedExt))
                return false;

            return true;
        }).ToArray();

        string[] validDirNames = dir.GetDirectories().Where(subDir =>
        {
            if (subDir.StartsWith('.'))
                return false;

            var pathToSubDir = dir.GetCurrentDir().PathJoin(subDir);

            if (Settings.ExcludedFolers.Any(excludedPath => pathToSubDir == "res://" + excludedPath))
                return false;

            return true;
        }).ToArray();

        string[] fileVarNames = validFileNames.Select<string, string>((fileName, _) => Utils.ConvertNameToCSName(fileName)).ToArray();
        string[] dirClassNames = validDirNames.Select<string, string>((dirName, _) => Utils.ConvertNameToCSName(dirName)).ToArray();

        AddFiles(validFileNames);
        AddDirectories(validDirNames);

        void AddFiles(string[] validFileNames)
        {
            for (int i = 0; i < validFileNames.Length; i++)
            {
                string filePath = dir.GetCurrentDir().PathJoin(validFileNames[i]);
                string fileType = Plugin.GetFileType(filePath);

                if (fileType == "Godot.TextFile")
                {
                    sb.Append(indent)
                        .Append("public static readonly string ")
                        .Append(fileVarNames[i])
                        .Append(" = \"")
                        .Append(filePath)
                        .Append("\";\n");

                    continue;
                }

                sb.Append(indent)
                    .Append("public static readonly ResourcePath")
                    .Append("<")
                    .Append(fileType)
                    .Append("> ")
                    .Append(fileVarNames[i])
                    .Append(" = \"")
                    .Append(filePath)
                    .Append("\";\n");
            }
        }

        void AddDirectories(string[] validDirNames)
        {
            for (int i = 0; i < validDirNames.Length; i++)
            {
                sb.Append(indent)
                    .Append("public static class ")
                    .Append(dirClassNames[i])
                    .Append("\n")
                    .Append(indent)
                    .Append("{\n");

                AppendDir(sb, DirAccess.Open(dir.GetCurrentDir().PathJoin(validDirNames[i])), indent + "    ");

                sb.Append(indent)
                    .Append("}\n");
            }
        }
    }
}

#endif
