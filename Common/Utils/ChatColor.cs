using System.Runtime.CompilerServices;

namespace SanguinePact.Common.Utils;

public class ChatColor
{
    public static string Color(string hexColor, string text)
    {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
        interpolatedStringHandler.AppendLiteral("<color=");
        interpolatedStringHandler.AppendFormatted(hexColor);
        interpolatedStringHandler.AppendLiteral(">");
        interpolatedStringHandler.AppendFormatted(text);
        interpolatedStringHandler.AppendLiteral("</color>");
        return interpolatedStringHandler.ToStringAndClear();
    }
    public static string White(string text) => Color("#FFFFFF", text);
    public static string Red(string text) => Color("#E90000", text);
    public static string Green(string text) => Color("#7FE030", text);
    public static string Blue(string text) => Color("#256DFE", text);
    public static string Yellow(string text) => Color("#FFE100", text);
    public static string Gray(string text) => Color("#A9A9A9", text);
    public static string Purple(string text) => Color("#D900FF", text);
}
