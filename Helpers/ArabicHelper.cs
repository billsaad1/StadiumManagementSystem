using System;
using System.Linq;

namespace StadiumManagementSystem.Helpers
{
    public static class ArabicHelper
    {
        /// <summary>
        /// Workaround for libraries that don't support RTL Arabic text correctly.
        /// It reverses the characters in each word.
        /// </summary>
        public static string FixArabicText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Simple check if it's Arabic (contains Arabic characters)
            bool hasArabic = text.Any(c => c >= 0x0600 && c <= 0x06FF);
            if (!hasArabic) return text;

            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
