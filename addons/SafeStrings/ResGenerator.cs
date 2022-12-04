#if TOOLS
namespace SafeStrings.Editor;

using System;
using System.Collections.Generic;
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

        string[] fileVarNames = validFileNames.Select<string, string>((fileName, _) => ConvertNameToCSName(fileName)).ToArray();
        string[] dirClassNames = validDirNames.Select<string, string>((dirName, _) => ConvertNameToCSName(dirName)).ToArray();

        AddFiles(validFileNames);
        AddDirectories(validDirNames);
        AddPreloadMethods();

        void AddFiles(string[] validFileNames)
        {
            for (int i = 0; i < validFileNames.Length; i++)
            {
                string filePath = dir.GetCurrentDir().PathJoin(validFileNames[i]);
                string fileType = Plugin.GetFileType(filePath);

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

        void AddPreloadMethods()
        {
            string inMethodIndent = indent + "    ";

            // Add Preload Method
            sb.Append(indent)
                .Append("public static void Preload()\n")
                .Append(indent)
                .Append("{\n");

            for (int i = 0; i < fileVarNames.Length; i++)
            {
                sb.Append(inMethodIndent)
                    .Append(fileVarNames[i])
                    .Append(".Preload();\n");
            }

            for (int i = 0; i < dirClassNames.Length; i++)
            {
                sb.Append(inMethodIndent)
                    .Append(dirClassNames[i])
                    .Append(".Preload();\n");
            }

            sb.Append(indent)
                .Append("}\n");

            // Add PreloadAsync Method
            sb.Append(indent)
                .Append("public static System.Threading.Tasks.Task PreloadAsync(bool parallel, int updateDelayMSec = 50, bool useSubThreads = false)\n")
                .Append(indent)
                .Append("{\n")
                .Append(inMethodIndent);

            // Parallel
            sb.Append("if (parallel)\n")
                .Append(inMethodIndent)
                .Append("    return System.Threading.Tasks.Task.Run(() => System.Threading.Tasks.Task.WaitAll(");

            for (int i = 0; i < fileVarNames.Length; i++)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }

                sb.Append("\n");

                sb.Append(inMethodIndent)
                    .Append("        ")
                    .Append(fileVarNames[i])
                    .Append(".PreloadAsync(updateDelayMSec, useSubThreads)");
            }

            for (int i = 0; i < dirClassNames.Length; i++)
            {
                if (i != 0 || fileVarNames.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append("\n");

                sb.Append(inMethodIndent)
                    .Append("        ")
                    .Append(dirClassNames[i])
                    .Append(".PreloadAsync(parallel, updateDelayMSec, useSubThreads)");
            }

            sb.Append("));\n");

            // Serial
            sb.Append(inMethodIndent)
                .Append("return System.Threading.Tasks.Task.Run(async () =>\n")
                .Append(inMethodIndent)
                .Append("{\n");

            for (int i = 0; i < fileVarNames.Length; i++)
            {
                sb.Append(inMethodIndent)
                    .Append("    await ")
                    .Append(fileVarNames[i])
                    .Append(".PreloadAsync(updateDelayMSec, useSubThreads);\n");
            }

            for (int i = 0; i < dirClassNames.Length; i++)
            {
                sb.Append(inMethodIndent)
                    .Append("    await ")
                    .Append(dirClassNames[i])
                    .Append(".PreloadAsync(parallel, updateDelayMSec, useSubThreads);\n");
            }

            sb.Append(inMethodIndent)
                .Append("});\n");

            sb.Append(indent)
                .Append("}\n");
        }
    }

    private string ConvertNameToCSName(string filespaceName)
        => ((char.IsLetter(filespaceName.First()) || filespaceName.StartsWith('_')) ? filespaceName : filespaceName.Insert(0, "_")).Replace('.', '_').Replace(' ', '_');
}

#endif