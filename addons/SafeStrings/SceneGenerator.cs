#if TOOLS
namespace SafeStrings.Editor;

using Godot;
using System;
using System.IO;
using System.Linq;
using System.Text;

public class SceneGenerator
{
    private const string GeneratedSceneAssociationsPath = "res://addons/SafeStrings/Generated/Scenes";

    private FileSystemWatcher watcher;

    public SceneGenerator()
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
        if (e.Name == null)
            return;

        UpdateScene(ProjectSettings.LocalizePath(e.FullPath));
    }

    public void UpdateAll()
    {
        foreach (string file in Directory.EnumerateFiles(ProjectSettings.GlobalizePath(GeneratedSceneAssociationsPath)))
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

        // Generate Scene Content
        StringBuilder sceneBuilder = new();
        StringBuilder uniqueSceneBuilder = new();

        var sceneState = scene.GetState();

        for (int i = 1; i < sceneState.GetNodeCount(); i++)
        {
            AppendNode(i);
        }

        // Generate Source
        StringBuilder sourceBuilder = new();

        if (classNamespace != "")
            sourceBuilder.Append("namespace ")
                .Append(classNamespace)
                .AppendLine(";");

        sourceBuilder.Append("partial class ")
            .Append(className)
            .Append(" : SafeStrings.IScene<")
            .Append(className)
            .AppendLine(">")
            .AppendLine("{");

        // Add Instantiate method
        sourceBuilder.Append("    public static ")
            .Append(className)
            .AppendLine(" Instantiate()")
            .AppendLine("    {")
            .Append("        var val = SafeStrings.")
            .Append(Utils.ConvertResPathToCSPath(scenePath))
            .Append(".Value.Instantiate<")
            .Append(className)
            .AppendLine(">();")
            .Append("        ((SafeStrings.IScene<")
            .Append(className)
            .AppendLine(">)val).OnInstanced();")
            .AppendLine("        return val;")
            .AppendLine("    }");

        // Add Scene
        sourceBuilder.AppendLine("    public static class Scene\n    {")
            .Append("        ")
            .AppendLine(sceneBuilder.ToString().ReplaceLineEndings("\n        "));

        // Add UniqueScene
        sourceBuilder.AppendLine("        public static class Unique\n        {")
            .Append("            ")
            .AppendLine(uniqueSceneBuilder.ToString().ReplaceLineEndings("\n            "));

        sourceBuilder.AppendLine("        }")
            .AppendLine("    }");

        sourceBuilder.AppendLine("}");

        try
        {
            File.WriteAllText($"{Settings.GlobalRootPath}/addons/SafeStrings/Generated/Scenes/{(classNamespace == "" ? className : $"{classNamespace}.{className}")}_Scene.g.cs", sourceBuilder.ToString());
        }
        catch (System.IO.IOException)
        {
            Plugin.Instance.GetTree().ToProcessFrame().OnCompleted(() => File.WriteAllText($"{Settings.GlobalRootPath}/addons/SafeStrings/Generated/Scenes/{(classNamespace == "" ? className : $"{classNamespace}.{className}")}_Scene.g.cs", sourceBuilder.ToString()));
        }

        void AppendNode(int idx)
        {
            bool isUnique = false;
            string type = GetNodeType(idx, sceneState);

            for (int i = 0; i < sceneState.GetNodePropertyCount(idx); i++)
            {
                if (sceneState.GetNodePropertyName(idx, i) == "unique_name_in_owner" && sceneState.GetNodePropertyValue(idx, i).AsBool())
                {
                    isUnique = true;
                    break;
                }
            }

            string nodeName = sceneState.GetNodeName(idx);
            string path = sceneState.GetNodePath(idx);
            string pathToParent = ((string)sceneState.GetNodePath(idx, true)).TrimPrefix("./");
            string[] pathSegments = pathToParent == "." ? new string[0] : pathToParent.Split('/');

            for (int i = 0; i < pathSegments.Length; i++)
            {
                sceneBuilder.Append("partial class ");

                if (pathSegments.ElementAtOrDefault(i - 1) == pathSegments[i])
                    sceneBuilder.Append("_");

                sceneBuilder.Append(pathSegments[i])
                    .Append(" { ");
            }

            sceneBuilder.Append("public static partial class ");

            if (pathSegments.LastOrDefault() == nodeName)
                sceneBuilder.Append("_");

            sceneBuilder.Append(nodeName)
                .AppendLine("\n{");

            sceneBuilder.Append("    public static SafeStrings.SceneNodePath<")
                .Append(type)
                .Append("> Path = \"")
                .Append(path)
                .AppendLine("\";");

            sceneBuilder.Append("    public static ")
                .Append(type)
                .AppendLine(" Get(Godot.Node root) => Path.Get(root);");

            sceneBuilder.Append("    public static ")
                .Append(type)
                .AppendLine(" GetCached(Godot.Node root) => Path.GetCached(root);");

            for (int i = 0; i < pathSegments.Length; i++)
            {
                sceneBuilder.Append("}");
            }

            sceneBuilder.AppendLine("}");

            if (!isUnique)
                return;

            uniqueSceneBuilder.Append("public static class ")
                .Append(nodeName)
                .AppendLine("\n{");

            uniqueSceneBuilder.Append("    public static SafeStrings.SceneNodePath<")
                .Append(type)
                .Append("> Path = Scene.");

            for (int i = 0; i < pathSegments.Length; i++)
            {
                if (pathSegments.ElementAtOrDefault(i - 1) == pathSegments[i])
                    uniqueSceneBuilder.Append("_");
                uniqueSceneBuilder.Append(pathSegments[i])
                    .Append(".");
            }

            if (pathSegments.LastOrDefault() == nodeName)
                uniqueSceneBuilder.Append("_");

            uniqueSceneBuilder.Append(nodeName)
                .AppendLine(".Path;");

            uniqueSceneBuilder.Append("    public static ")
                .Append(type)
                .AppendLine(" Get(Godot.Node root) => Path.Get(root);");

            uniqueSceneBuilder.Append("    public static ")
                .Append(type)
                .AppendLine(" GetCached(Godot.Node root) => Path.GetCached(root);");

            uniqueSceneBuilder.AppendLine("}");
        }
    }

    private string GetNodeType(int idx, SceneState sceneState)
    {
        for (int i = 0; i < sceneState.GetNodePropertyCount(idx); i++)
        {
            string propName = sceneState.GetNodePropertyName(idx, i);

            if (propName == "script" && sceneState.GetNodePropertyValue(idx, i).Obj is CSharpScript)
                return Utils.GetCsFullNameFromScript((CSharpScript)sceneState.GetNodePropertyValue(idx, i));
        }

        string stateType = sceneState.GetNodeType(idx);

        if (stateType is not "" and not null)
            return Utils.ConvertGdTypeToCsType(stateType);

        PackedScene instancedScene = sceneState.GetNodeInstance(idx);

        if (instancedScene == null)
            return "Godot.Node";

        return GetNodeType(0, instancedScene.GetState());
    }
}

#endif
