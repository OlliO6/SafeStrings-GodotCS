namespace SafeStrings.Editor;

using System.Linq;
using Godot;

public static class Utils
{
    public static T WithChilds<T>(this T from, params Node[] childNodes)
        where T : Node
    {
        for (int i = 0; i < childNodes.Length; i++)
            from.AddChild(childNodes[i]);

        return from;
    }


    public static string ConvertNameToCSName(string filespaceName)
    {
        return ((char.IsLetter(filespaceName.First()) || filespaceName.StartsWith('_')) ? filespaceName : filespaceName
            .Insert(0, "_")).Replace('.', '_').Replace(' ', '_');
    }

    public static string GetCsTypeFullNameFromScript(CSharpScript script)
    {
        GD.Print(script.SourceCode);

        return "";
    }
}