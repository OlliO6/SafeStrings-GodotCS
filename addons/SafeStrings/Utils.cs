namespace SafeStrings.Editor;

using System.IO;
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

    public static void GetCsTypeFromScript(CSharpScript script, out string @namespace, out string @class)
    {
        const string Namespace = "namespace ";
        const string ClassBegin = "class ";
        const string Class = " class ";

        StreamReader sourceReader = new(ProjectSettings.GlobalizePath(script.ResourcePath));

        @namespace = "";
        @class = "";

        while (true)
        {
            string line = sourceReader.ReadLine();

            if (line == null)
                break;

            if (line.StartsWith("//"))
                continue;

            if (line.StartsWith(Namespace))
            {
                @namespace = GetNamespace(line);
                continue;
            }

            if (line.StartsWith(ClassBegin))
            {
                @class = GetClass(line, fromBegin: true);
                continue;
            }

            if (line.Contains(" class "))
            {
                @class = GetClass(line, fromBegin: false);
            }
        }

        sourceReader.Close();

        static string GetNamespace(string line)
        {
            string @namespace = line.Substring(Namespace.Length);

            if (@namespace.Contains(' ') || @namespace.Contains(';'))
            {
                int lenght = @namespace.IndexOfAny(new char[] { ' ', ';' });

                @namespace = @namespace.Substring(0, lenght);
            }

            return @namespace;
        }

        static string GetClass(string line, bool fromBegin)
        {
            string @class;

            if (fromBegin)
                @class = line.Substring(ClassBegin.Length);
            else
                @class = line.Substring(line.IndexOf(Class) + Class.Length);

            if (@class.Contains(':'))
                @class = @class.Substring(0, @class.IndexOf(':'));

            return @class.StripEdges();
        }
    }
}