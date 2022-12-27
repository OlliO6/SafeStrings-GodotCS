#if TOOLS
namespace SafeStrings.Editor;

using System;
using Godot;
using Godot.Collections;

public static class Settings
{
    public static readonly string GlobalRootPath = ProjectSettings.GlobalizePath("res://");

    private const string ExcludedFolersPath = "SafeStrings/resources/excluded_folers";
    private const string ExcludedExtensionsPath = "SafeStrings/resources/excluded_formats";
    private const string SceneAssociationsPath = "SafeStrings/scenes/scene_associations";

    public static Array<string> ExcludedFolers => GetOrAddSetting(ExcludedFolersPath, new Array<string>
    {
        "addons"
    }).AsGodotArray<string>();

    public static Array<string> ExcludedExtensions => GetOrAddSetting(ExcludedExtensionsPath, new Array<string>
    {
        "tmp",
        "TMP",
        "import",
        "godot",
        "csproj",
        "sln"
    }).AsGodotArray<string>();

    public static Dictionary<string, string> SceneAssociations => GetOrAddSetting(SceneAssociationsPath,
        new Dictionary<string, string>()).AsGodotDictionary<string, string>();

    public static void InitSettings()
    {
        _ = ExcludedFolers;
        _ = ExcludedExtensions;
        _ = SceneAssociations;
    }

    private static Variant GetOrAddSetting(string path, Variant @default)
    {
        if (ProjectSettings.HasSetting(path))
            return ProjectSettings.GetSetting(path);

        ProjectSettings.SetSetting(path, @default);
        ProjectSettings.SetInitialValue(path, @default);
        ProjectSettings.AddPropertyInfo(new Dictionary
        {
            { "name", path },
            { "type", (int)@default.VariantType },
        });

        return @default;
    }

    public static void AddSceneAssociation(string scenePath, string scriptPath)
    {
        Settings.SceneAssociations[scenePath] = scriptPath;

        ProjectSettings.SetSetting(SceneAssociationsPath,
            new Dictionary<string, string>(Settings.SceneAssociations));

        ProjectSettings.Save();
    }
}

#endif