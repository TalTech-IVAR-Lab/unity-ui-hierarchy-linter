namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using UnityEngine;

    internal static class StringExtensions
    {
        private const char Ellipsis = '…';
        
        /// <summary>
        /// Truncates the string down to the length of <see cref="maxChars"/> and appends an ellipsis to it.
        /// </summary>
        /// <param name="value">String to ellipsize.</param>
        /// <param name="maxChars">Maximum number of characters to leave in the string.</param>
        /// <returns>Ellipsized string.</returns>
        private static string Ellipsize(this string value, int maxChars) { return value.Length <= maxChars ? value : value[..maxChars] + Ellipsis; }

        /// <summary>
        /// Truncates the string down to the length of <see cref="maxChars"/> and appends an ellipsis to it.
        /// </summary>
        /// <param name="value">String to ellipsize.</param>
        /// <param name="maxChars">Maximum number of characters to leave in the string.</param>
        /// <returns>Ellipsized string.</returns>
        internal static string EllipsizeMultiline(this string value, int maxChars)
        {
            if (!value.Contains('\n')) return value.Ellipsize(maxChars);
            Debug.Log($"AAAA {value}");
            string firstLine = value.Split('\n')[0];
            if (firstLine.Length <= maxChars) return firstLine + Ellipsis;

            return firstLine.Ellipsize(maxChars);
        }

        /// <summary>
        /// Removes the given prefix from the beginning of the given string.
        /// </summary>
        /// <param name="text">String to remove prefix from.</param>
        /// <param name="prefix">Prefix to remove.</param>
        /// <returns>New string without the prefix.</returns>
        internal static string RemovePrefix(this string text, string prefix) { return text.StartsWith(prefix) ? text[prefix.Length..] : text; }

        /// <summary>
        /// Removes all matching characters in the given prefix from the beginning of the given string.
        /// </summary>
        /// <param name="text">String to remove prefix from.</param>
        /// <param name="prefix">Prefix to remove.</param>
        /// <returns>New string without the prefix.</returns>
        internal static string RemoveUpToFullPrefix(this string text, string prefix)
        {
            int lengthToRemove = 0;

            for (int i = 0; i < prefix.Length; i++)
            {
                // Continue increasing to-be-removed length until we hit a mismatching character or the end of string
                if (i >= text.Length) break;
                if (prefix[i] != text[i]) break;
                lengthToRemove++;
            }

            // Remove prefix
            return text[lengthToRemove..];
        }
    }
}