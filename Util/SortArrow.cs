namespace Assignment01.Util;

public static class SortArrow
{
    public static string Arrow(string sortOrder, string ascKey, string descKey)
    {
        if (sortOrder == ascKey) return "▲";
        if (sortOrder == descKey) return "▼";
        return string.Empty;
    }
}