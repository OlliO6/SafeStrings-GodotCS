#if TOOLS
namespace SafeStrings.Editor;

using Godot;
using Godot.Collections;

public static class Settings
{
    public static readonly string GlobalRootPath = ProjectSettings.GlobalizePath("res://");

    private const string ExcludedFolersPath = "SafeStrings/resources/excluded_folers";
    private const string ExcludedExtensionsPath = "SafeStrings/resources/excluded_formats";

    public static Array<string> ExcludedFolers => GetOrAddSetting(ExcludedFolersPath, new Array<string>
    {
        "addons"
    }).AsGodotArray<string>();

    public static Array<string> ExcludedExtensions => GetOrAddSetting(ExcludedExtensionsPath, new Array<string>
    {
        "tmp",
        "TMP",
        "godot",
        "csproj",
        "sln"
    }).AsGodotArray<string>();

    public static void InitSettings()
    {
        _ = ExcludedFolers;
        _ = ExcludedExtensions;
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
            { "advanced", false }
        });

        return @default;
    }
}
#endif