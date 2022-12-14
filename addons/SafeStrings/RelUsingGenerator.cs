#if TOOLS
namespace SafeStrings.Editor;

using System;
using System.IO;
using System.Text;
using Godot;

public static class RelUsingGenerator
{
    public static void GenerateRelUsing(string pathToScript)
    {
        if (pathToScript.GetExtension() != "cs")
        {
            GD.Print("Select a CSharp script first.");
            return;
        }

        var scriptReader = new StreamReader(pathToScript, Encoding.UTF8);
        var scriptBuilder = new StringBuilder();

        string fileScopedNamespaceLine = "";

        while (true)
        {
            string line = scriptReader.ReadLine();

            if (line == null)
                break;

            if (line.StartsWith("using Rel = "))
                continue;

            if (line.Contains("namespace") && line.Contains(';'))
            {
                fileScopedNamespaceLine = line;
                continue;
            }

            scriptBuilder.AppendLine(line);
        }

        scriptBuilder.Insert(0, $"using Rel = {GetRelCSPath(pathToScript.GetBaseDir())};\n");

        if (fileScopedNamespaceLine != "")
            scriptBuilder.Insert(0, $"{fileScopedNamespaceLine}\n\n");

        scriptReader.Close();
        File.WriteAllText(pathToScript, scriptBuilder.ToString(), Encoding.UTF8);

        GD.Print("Added Rel using to ", pathToScript);

        string GetRelCSPath(string pathToFolder)
        {
            var sb = new StringBuilder("SafeStrings.Res");

            foreach (string folder in pathToFolder.Split('/'))
            {
                sb.Append(".")
                    .Append(Utils.ConvertNameToCSName(folder));
            }

            return sb.ToString();
        }
    }
}

#endif