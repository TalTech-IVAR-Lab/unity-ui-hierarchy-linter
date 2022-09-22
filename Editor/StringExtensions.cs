namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    internal static class StringExtensions
    {
        internal static string Ellipsize(this string value, int maxChars) { return value.Length <= maxChars ? value : value[..maxChars] + "…"; }

        internal static string RemovePrefix(this string text, string prefix) { return text.StartsWith(prefix) ? text[prefix.Length..] : text; }
    }
}