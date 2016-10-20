using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        #region Private Variables

        private static readonly Dictionary<char, string> CharLookup =
                new Dictionary<char, string>
                {
                    { char.ConvertFromUtf32(230)[0] , "ae"},// æ
                    { char.ConvertFromUtf32(198)[0] , "Ae"},// Æ
                    { char.ConvertFromUtf32(240)[0] , "d"}, // ð
                    { char.ConvertFromUtf32(208)[0] , "D"}, // Ð
                    { char.ConvertFromUtf32(248)[0] , "o"}, // ø
                    { char.ConvertFromUtf32(216)[0] , "O"}, // Ø
                    { char.ConvertFromUtf32(254)[0] , "ye"}, // þ
                    { char.ConvertFromUtf32(222)[0] , "Ye"}  // Þ
                };

        #endregion

        #region Public Methods

        #region EscapeQuote
        public static string EscapeQuote(this string source) => source.IsNullOrWhiteSpace() ? source : source.Replace("\"", "\\\"");

        #endregion

        #region EscapeSingleAndDoubleQuote
        public static string EscapeSingleAndDoubleQuote(this string source) => source.IsNullOrWhiteSpace() ? string.Empty : source.Replace("\'", "&#39;").Replace("\"", "&#x22;");

        #endregion

        #region GetBytes
        public static byte[] GetBytes(this string str) => str.IsNotNullOrWhiteSpace() ? Encoding.ASCII.GetBytes(str) : null;

        #endregion

        #region HasAnyText
        public static bool HasAnyText(this string source, params string[] textValues) => textValues.HasItems() && textValues.Any(source.Contains);

        #endregion

        #region IsNumber
        public static bool IsNumber(this string value)
        {
            if (value.IsNotNullOrWhiteSpace())
            {
                value = value.Trim();
                return Regex.Match(value, @"^\d+$").Success;
            }
            return false;
        }
        #endregion

        #region NormalizeText
        public static string NormalizeText(this string source) => source.IsNullOrWhiteSpace() ? source : source.Trim().StripDoubleQuote().StripHtmlTags();

        #endregion

        #region Prepend
        public static string Prepend(this string str, string prefix) => prefix.IsNotNullOrWhiteSpace() ? string.Concat(prefix, str) : str;

        #endregion

        #region RemoveAccent
        public static string RemoveAccent(this string input) => Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(input));
        #endregion

        #region RemoveDiacritics
        /// <summary>
        /// Remove accents and other charcters from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string input)
        {
            if (input == null)
                return null;

            var formD = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc == UnicodeCategory.NonSpacingMark) continue;
                if (CharLookup.ContainsKey(t))
                    sb.Append(CharLookup[t]);
                else
                    sb.Append(t);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion

        #region Replace
        /// <summary>
        /// Returns a new string in which all occurrences of specified Unicode characters in this instance are replaced with another specified Unicode character.
        /// </summary>
        /// <param name="input">The string contains the oldChars to be replaced.</param>
        /// <param name="oldChars">An array of Unicode characters to be replaced.</param>
        /// <param name="newChar">The Unicode character to replace all occurrences of oldChars.</param>
        /// <returns>A string that is equivalent to this instance except that all instances of oldChars are replaced with newChar.</returns>
        public static string Replace(this string input, char[] oldChars, char newChar)
        {
            // if not data, then return untouched
            if (input.IsNullOrWhiteSpace()) return input;

            var array = new char[input.Length];
            var arrayIndex = 0;

            foreach (var c in input)
            {
                // check if the character should be replaced
                array[arrayIndex] = oldChars.Contains(c)
                        ? newChar
                        : c;
                arrayIndex++;
            }
            return new string(array, 0, arrayIndex);
        }
        #endregion

        #region Replace

        /// <summary>
        /// Returns a new string in which all occurrences of specified strings in this instance are replaced with another string.
        /// </summary>
        /// <param name="input">The string contains the oldChars to be replaced.</param>
        /// <param name="oldValues">An array of strings to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of oldValues.</param>
        /// <returns>A string that is equivalent to this instance except that all instances of oldValues are replaced with newValue.</returns>
        public static string Replace(this string input, string[] oldValues, string newValue) =>
            input.IsNullOrWhiteSpace()
                ? input
                : oldValues.Aggregate(input, (current, oldValue) => current.Replace(oldValue, newValue));
        #endregion

        #region Slugify
        public static string Slugify(this string value, string defaultValue) => value.Slugify() ?? defaultValue.Slugify();

        public static string Slugify(this string value)
        {
            // return null if value is null or empty
            if (value.IsNullOrWhiteSpace()) return null;

            // remove html tags
            value = value.StripHtmlTags();

            // html decode string
            value = WebUtility.HtmlDecode(value);

            // first to lower case
            value = value.ToLowerInvariant();

            // remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            // replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            // remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            // trim dashes from end
            value = value.Trim('-', '_');

            // replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value.IsNotNullOrWhiteSpace() ? value : null;
        }
        #endregion

        #region StripDoubleQuote
        public static string StripDoubleQuote(this string source) => source.IsNullOrWhiteSpace() ? source : source.Replace("\"", "");
        #endregion

        #region StripHtmlTags
        /// <summary>
        /// Strip out HTML tags from a string
        /// </summary>
        /// <param name="input">The string</param>
        /// <returns></returns>
        /// <see cref="http://www.dotnetperls.com/remove-html-tags"/>
        public static string StripHtmlTags(this string input)
        {
            // if not data, then return untouched
            if (input.IsNullOrWhiteSpace()) return input;

            // add extra space after DIV and P
            input = input.Replace(new[] { "</div>", "</p>" }, " ");

            var array = new char[input.Length];
            var arrayIndex = 0;
            var inside = false;
            var insideQuoted = false;


            foreach (var c in input)
            {
                if (insideQuoted)
                {
                    if (c == '"' || c == '\'')
                    {
                        insideQuoted = false;
                    }
                    continue;
                }

                // if angle brackets are inside of quotes, don't mark as the end of the tag
                if ((c == '"' || c == '\'') && inside)
                {
                    insideQuoted = true;
                    continue;
                }

                switch (c)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }
                if (inside) continue;

                array[arrayIndex] = c;
                arrayIndex++;
            }
            return new string(array, 0, arrayIndex).Trim();
        }
        #endregion

        #region StripHtmlTagsAndText
        /// <summary>
        /// Removes HTML tags and its "inner" content from a string.
        /// </summary>
        /// <param name="input">The string of html</param>
        /// <param name="tags">An array of stringsformatted without brackets (e.g, [a, div, ul])</param>
        /// <returns>A string of html equivilent to the input except the html tags and its contents indicated in the tags param are removed</returns>
        public static string StripHtmlTagsAndText(this string input, string[] tags)
        {
            // if not data, then return untouched
            if (input.IsNullOrWhiteSpace()) return input;

            // if empty array, then return untouched
            if (tags == null || tags.Length == 0)
                return input;

            foreach (var tag in tags)
            {
                input = Regex.Replace(input, "<" + tag + ".*(/)?>.*(</" + tag + ">){1}", string.Empty);
                input = Regex.Replace(input, "(<" + tag + ".*(/)?>){1}", string.Empty);
            }

            return input;

        }
        #endregion

        #region StripMaliciousUrlParams
        /// <summary>
        /// Strip out malicious url params tags, quotatoins marks, and other possibly malicious characters from a string
        /// </summary>
        /// <param name="input">The string</param>
        /// <returns></returns>
        public static string StripMaliciousUrlParams(this string input)
        {
            // if not data, then return untouched
            if (input.IsNullOrWhiteSpace()) return input;

            var output = input.StripHtmlTags().Replace(new[] { "<", ">", "\"", "'", "(", ")" }, string.Empty);
            return output;
        }
        #endregion

        #region Truncate
        /// <summary>
        /// Truncates the string if it is longer than the maxLength
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="maxLength">The maxLength of the returned string including the suffix length.</param>
        /// <param name="suffix">Text to add to the end of the string</param>
        /// <param name="searchWordBoundary">If true, will not cut off a word in the middle when truncating</param>
        /// <returns></returns>
        public static string Truncate(this string str, int maxLength, string suffix = "...", bool searchWordBoundary = false)
        {
            if (maxLength <= suffix.Length) return suffix;

            if (str.Length + suffix.Length < maxLength) return str;

            str = searchWordBoundary
                ? str.Substring(0, maxLength - suffix.Length).RemoveLastWord()
                : str.Substring(0, maxLength - suffix.Length);

            return $"{str}{suffix}";
        }
        #endregion


        #region SplitToEnumerable
        public static IEnumerable<string> SplitToEnumerable(this string original, char splitChar) =>
                        original.Split(splitChar).ToList().Where(value => value.IsNotNullOrEmpty()).ToList();

        #endregion

        #region GetSafeString
        public static string GetSafeString(this string value) => value.IsNullOrEmpty() ? "" : value;

        #endregion

        #region IsNullOrEmpty
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
        #endregion

        #region IsNotNullOrEmpty
        public static bool IsNotNullOrEmpty(this string value) => !string.IsNullOrEmpty(value);
        #endregion

        #region IsNullOrWhiteSpace
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
        #endregion

        #region IsNotNullOrWhiteSpace
        public static bool IsNotNullOrWhiteSpace(this string value) => !string.IsNullOrWhiteSpace(value);
        #endregion

        #region CoalesceNullOrEmpty
        public static string CoalesceNullOrEmpty(this string value, string replaceValue) => value.IsNullOrEmpty() ? replaceValue : value;

        #endregion

        #region CoalesceNullOrWhitespace
        public static string CoalesceNullOrWhitespace(this string value, string replaceValue) => value.IsNullOrWhiteSpace() ? replaceValue : value;
        #endregion

        #region FormatWith
        public static string Format(this string value, params object[] args) => string.Format(value, args);
        #endregion

        #region Base64String
        public static string ToBase64String(this string value) => Convert.ToBase64String(Encoding.ASCII.GetBytes(value));

        public static string FromBase64String(this string value) => Encoding.ASCII.GetString(Convert.FromBase64String(value));
        #endregion

        #region RemoveLastWord
        /// <summary>
        /// If the string has more than one word in it, then return substring up to the last space;
        /// else delete string since there's only one word to remove
        /// </summary>
        /// <param name="input">The string this will be performed on.</param>
        /// <returns>String without last word if it had more than 1 word; else returns empty string</returns>
        public static string RemoveLastWord(this string input)
        {
            if (input.IsNullOrWhiteSpace())
                return string.Empty;

            var strPos = input.LastIndexOf(" ", StringComparison.Ordinal);
            return strPos > 0 ? input.Substring(0, strPos) : string.Empty;
        }
        #endregion

        #endregion
    }
}
