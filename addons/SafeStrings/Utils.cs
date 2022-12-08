namespace SafeStrings.Editor;

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
}