using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Net;

using HtmlAgilityPack;

namespace Quisitive.Framework.Extensions
{
    public static class StringExtensions
    {
        #region Private Variables

        private static readonly Dictionary<char, string> CharLookup =
                new Dictionary<char, string>
                {
                    { Char.ConvertFromUtf32(230)[0] , "ae"},// æ
                    { Char.ConvertFromUtf32(198)[0] , "Ae"},// Æ
                    { Char.ConvertFromUtf32(240)[0] , "d"}, // ð
                    { Char.ConvertFromUtf32(208)[0] , "D"}, // Ð
                    { Char.ConvertFromUtf32(248)[0] , "o"}, // ø
                    { Char.ConvertFromUtf32(216)[0] , "O"}, // Ø
                    { Char.ConvertFromUtf32(254)[0] , "ye"}, // þ
                    { Char.ConvertFromUtf32(222)[0] , "Ye"}  // Þ
                };

        #endregion

        #region Public Methods

        #region EncloseInTag
        public static string EncloseInTag(this string source, string tag)
        {
            if (!source.HasText())
                return string.Empty;

            if (!tag.HasText())
                return source.HasText() ? source : string.Empty;

            if (tag.Contains(" "))
                tag = tag.Replace(" ", "");

            return string.Format("<{0}>{1}</{0}>", tag, source);
        }
        #endregion

        #region EncloseInTagWithAttributes
        /// <summary>
        /// Wraps text in an html tag with attributes of the developer's choosing. 
        /// Allows for an override to not display the tag if there are any empty attributes (all-or-nothing setting)
        /// </summary>
        public static string EncloseInTagWithAttributes(this string source, string tag, Dictionary<string, string> attributeValues, bool displayTagWithNoAttributes = true)
        {
            if (!source.HasText())
                return string.Empty;

            if (!tag.HasText())
                return source.HasText() ? source : string.Empty;

            if (attributeValues == null)
                return source.HasText() ? source : string.Empty;

            if (tag.Contains(" "))
                tag = tag.Replace(" ", "");

            /// add the tag attributes
            var tagAttributesAndValues = "";

            /// special case:
            /// tag is anchor and href key does not exist
            if (tag == "a" && !attributeValues.ContainsKey("href"))
                /// return the original source
                return source.HasText() ? source : string.Empty;

            /// loop over dictionary object
            foreach (var attributeValue in attributeValues)
            {
                /// if the strict tag display mode is on and the attribute value is empty
                if (!displayTagWithNoAttributes && string.IsNullOrEmpty(attributeValue.Value))
                {
                    /// return the original source, exiting the loop
                    return source.HasText() ? source : string.Empty;
                }
                /// if the attribute key is empty, return
                if (!attributeValue.Key.HasText())
                {
                    /// return the original source, exiting the loop
                    return source.HasText() ? source : string.Empty;
                }
                /// construct the attribute key-value pairs
                /// Only constructs this attribute if Attribute has text and Attribute Value is not Empty
                if (attributeValue.Key.HasText() && !string.IsNullOrEmpty(attributeValue.Value))
                {
                    tagAttributesAndValues = string.Format("{0} {1}=\"{2}\"", tagAttributesAndValues, attributeValue.Key, attributeValue.Value);
                }
            }

            /// If the strict tag display mode is on and the attributes are empty
            /// or if this is an anchor tag and the href has no value
            if ((!tagAttributesAndValues.HasText() && !displayTagWithNoAttributes) || (tag == "a" && !attributeValues["href"].HasText()))
            {
                /// return the original source
                return source.HasText() ? source : string.Empty;
            }
            else
            {
                /// return the formatted tag
                return string.Format("<{0}{2}>{1}</{0}>", tag, source, tagAttributesAndValues);
            }
        }
        #endregion

        #region EscapeQuote
        public static string EscapeQuote(this string source)
        {
            return !source.HasText()
                ? string.Empty
                : source.Replace("\"", "\\\"");
        }
        #endregion

        #region EscapeSingleAndDoubleQuote
        public static string EscapeSingleAndDoubleQuote(this string source)
        {
            return !source.HasText()
                ? string.Empty
                : source.Replace("\'", "&#39;").Replace("\"", "&#x22;");
        }
        #endregion

        #region GetBytes
        public static byte[] GetBytes(this string str)
        {
            return str.HasText() ? Encoding.ASCII.GetBytes(str) : null;
        }
        #endregion

        #region HasText
        public static bool HasText(this string source)
        {
            return !string.IsNullOrWhiteSpace(source);
        }
        #endregion

        #region HasAnyText
        public static bool HasAnyText(this string source, params string[] textValues)
        {
            if (!textValues.HasItems())
                return false;

            return textValues.Any(source.Contains);
        }
        #endregion

        #region IsNumber
        public static bool IsNumber(this string value)
        {
            if (!value.HasText())
            {
                return false;
            }

            value = value.Trim();
            return Regex.Match(value, @"^\d+$").Success;
        }
        #endregion

        #region NormalizeText
        public static string NormalizeText(this string source)
        {
            return !source.HasText()
                ? string.Empty
                : source.Trim().StripDoubleQuote().StripHtmlTags();
        }
        #endregion

        #region Prepend
        public static string Prepend(this string str, string prefix)
        {
            return prefix.HasText()
                       ? string.Concat(prefix, str)
                       : str;
        }
        #endregion

        #region RemoveAccent
        public static string RemoveAccent(this string input)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(input);
            return Encoding.ASCII.GetString(bytes);
        }
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
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    if (CharLookup.ContainsKey(t))
                        sb.Append(CharLookup[t]);
                    else
                        sb.Append(t);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
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
            if (string.IsNullOrWhiteSpace(input))
                return input;

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
        public static string Replace(this string input, string[] oldValues, string newValue)
        {
            // if not data, then return untouched
            return string.IsNullOrWhiteSpace(input)
                    ? input
                    : oldValues.Aggregate(input, (current, oldValue) => current.Replace(oldValue, newValue));
        }
        #endregion

        #region Slugify
        public static string Slugify(this string value, string defaultValue)
        {
            return value.Slugify() ?? defaultValue.Slugify();
        }

        public static string Slugify(this string value)
        {
            // return null if value is null or empty
            if (!value.HasText())
                return null;

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

            return value.HasText() ? value : null;
        }
        #endregion

        #region StripDoubleQuote
        public static string StripDoubleQuote(this string source)
        {
            return !source.HasText()
                ? string.Empty
                : source.Replace("\"", "");
        }
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
            if (string.IsNullOrWhiteSpace(input))
                return input;

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
                    if (!insideQuoted)
                    {
                        insideQuoted = true;
                    }
                    continue;
                }

                if (c == '<')
                {
                    inside = true;
                    continue;
                }
                if (c == '>')
                {
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
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // if empty array, then return untouched
            if (tags == null || tags.Length == 0)
                return input;

            foreach (string tag in tags)
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
            if (string.IsNullOrWhiteSpace(input))
                return input;

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
            if (maxLength <= suffix.Length)
                return suffix;

            if (str.Length + suffix.Length < maxLength)
            {
                return str;
            }

            str = searchWordBoundary
                ? str.Substring(0, maxLength - suffix.Length).RemoveLastWord()
                : str.Substring(0, maxLength - suffix.Length);

            return string.Format("{0}{1}", str, suffix);
        }
        #endregion

        #region TruncateHtml
        /// <summary>
        /// Truncates the HTML string if it is longer than the maxLength
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="maxLength">The maxLength of the returned string including the suffix length.</param>
        /// <param name="suffix">Text to add to the end of the string</param>
        /// <param name="stripTags"></param>
        /// <param name="searchWordBoundary">If true, will not cut off a word in the middle when truncating</param>
        /// <returns></returns>
        public static string TruncateHtml(this string str, int maxLength, string suffix = "...", bool stripTags = false, bool searchWordBoundary = false)
        {
            const string placeHolderSuffix = @"{placeholdersuffix}";

            // your data, probably comes from somewhere, or as params to a method
            var xml = new XmlDocument
            {
                PreserveWhitespace = false,
                XmlResolver = null
            };
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(str);
                htmlDoc.OptionOutputAsXml = true;

                xml.LoadXml(htmlDoc.DocumentNode.InnerHtml);
            }
            catch
            {
                return str.Truncate(maxLength, suffix, searchWordBoundary);
            }

            if (xml.DocumentElement == null)
                return str;

            // if stripTags is true, then use Truncate on just the text
            if (stripTags)
                return xml.DocumentElement.InnerXml.StripHtmlTags().Truncate(maxLength, suffix, searchWordBoundary);

            // if the text (without tags) is under maxLength, then return as is
            if (xml.DocumentElement.InnerText.Length <= maxLength)
                return str;

            // maxLength needs to include suffix length
            maxLength = maxLength - suffix.Length;

            // create a navigator, this is our primary tool
            var navigator = xml.CreateNavigator();
            XPathNavigator breakPoint = null;

            // find the text node we need:
            while (navigator.MoveToFollowing(XPathNodeType.Text))
            {
                var lastText = navigator.Value.Substring(0, Math.Min(maxLength, navigator.Value.Length));
                maxLength -= navigator.Value.Length;

                if (maxLength <= 0)
                {
                    if (searchWordBoundary)
                        lastText = lastText.RemoveLastWord();

                    if (!lastText.HasText())
                    {
                        //If removing the last word caused the current node to become empty,
                        //we don't want to insert the suffix here because the current HTML tags 
                        //should be abandoned.  Rewind to the last text node in the xml that wasn't empty.
                        //Put the suffix there and declare it the breakpoint.
                        navigator.SetValue(lastText);
                        while (navigator.NodeType != XPathNodeType.Root)
                        {
                            if (navigator.NodeType == XPathNodeType.Text && navigator.Value != string.Empty)
                                break;

                            //If we can't move to a previous sibling, move up to the parent.
                            if (!navigator.MoveToPrevious())
                            {
                                navigator.MoveToParent();
                                navigator.MoveToPrevious();
                            }

                            //Try to find a XPathNodeType.Text from the new location
                            if (navigator.Value.HasText())
                            {
                                if (navigator.HasChildren)
                                {
                                    // get the text node inside the last child element if any
                                    var childNavigator = GetLastChildPosition(navigator);
                                    childNavigator.MoveToFollowing(XPathNodeType.Text);
                                    if (childNavigator.Value.HasText())
                                        navigator = childNavigator;
                                }
                            }
                        }

                        //replace with actual suffix after xml processing
                        navigator.InsertAfter(placeHolderSuffix);

                        breakPoint = navigator.Clone();
                        break;
                    }

                    navigator.SetValue(lastText);
                    navigator.InsertAfter(placeHolderSuffix); //replace with actual suffix after xml processing
                    breakPoint = navigator.Clone();
                    break;
                }
            }

            // first remove text nodes, because Microsoft unfortunately merges them without asking
            while (navigator.MoveToFollowing(XPathNodeType.Text))
                if (navigator.ComparePosition(breakPoint) == XmlNodeOrder.After)
                    navigator.DeleteSelf();   // moves to parent

            // then move the rest
            navigator.MoveTo(breakPoint);
            while (navigator.MoveToFollowing(XPathNodeType.Element))
                if (navigator.ComparePosition(breakPoint) == XmlNodeOrder.After)
                    navigator.DeleteSelf();   // moves to parent

            // then remove *all* empty nodes to clean up (not necessary): 
            // TODO, add empty elements like <br />, <img /> as exclusion
            navigator.MoveToRoot();
            while (navigator.MoveToFollowing(XPathNodeType.Element))
                while (!navigator.HasChildren && (navigator.Value ?? "").Trim() == "")
                    navigator.DeleteSelf();  // moves to parent

            navigator.MoveToRoot();

            //apply the suffix as a string replace to avoid having the navigator evaluate the HTML number to its symbol
            return WebUtility.HtmlDecode(navigator.InnerXml).Replace(placeHolderSuffix, suffix);
        }

        /// <summary>
        /// Sets the XPathNavigator to the position of the last child element
        /// </summary>
        /// <param name="navigator">An XPathNavigator object</param>
        /// <returns>Returns the navigator at the position of the last child element</returns>
        private static XPathNavigator GetLastChildPosition(XPathNavigator navigator)
        {
            if (navigator.HasChildren)
            {
                XPathNavigator nextNode = navigator.Clone();
                nextNode.MoveToNext();

                do
                {
                } while (navigator.MoveToFollowing(XPathNodeType.Element, nextNode));
            }

            return navigator;
        }

        #endregion

        #region SplitToEnumerable
        public static IEnumerable<String> SplitToEnumerable(this string original, char splitChar)
        {
            var result = new List<String>();
            foreach (var value in original.Split(splitChar).ToList())
                if (!String.IsNullOrEmpty(value))
                    result.Add(value);
            return result;
        }
        #endregion

        #region GetSafeString
        public static string GetSafeString(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return "";
            else
                return value;
        }
        #endregion

        #region IsNullOrEmpty
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }
        #endregion

        #region IsNotNullOrEmpty
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }
        #endregion

        #region IsNullOrWhiteSpace
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }
        #endregion

        #region IsNotNullOrWhiteSpace
        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }
        #endregion

        #region CoalesceNullOrEmpty
        public static string CoalesceNullOrEmpty(this string value, string replaceValue)
        {
            var retval = value;
            if (string.IsNullOrEmpty(value)) retval = replaceValue;
            return retval;
        }
        #endregion

        #region CoalesceNullOrWhitespace
        public static string CoalesceNullOrWhitespace(this string value, string replaceValue)
        {
            var retval = value;
            if (string.IsNullOrWhiteSpace(value)) retval = replaceValue;
            return retval;
        }
        #endregion

        #region FormatWith
        public static string FormatWith(this string value, string arg1)
        {
            return String.Format(value, arg1);
        }

        public static string FormatWith(this string value, string arg1, string arg2)
        {
            return String.Format(value, arg1, arg2);
        }
        public static string FormatWith(this string value, string arg1, string arg2, string arg3)
        {
            return String.Format(value, arg1, arg2, arg3);
        }
        public static string FormatWith(this string value, params string[] args)
        {
            return String.Format(value, args);
        }
        #endregion

        #region Base64String
        public static string ToBase64String(this string value)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public static string FromBase64String(this string value)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(value);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
        #endregion

        #endregion // Public Methods

        #region Private Methods

        #region RemoveLastWord
        /// <summary>
        /// If the string has more than one word in it, then return substring up to the last space;
        /// else delete string since there's only one word to remove
        /// </summary>
        /// <param name="input">The string this will be performed on.</param>
        /// <returns>String without last word if it had more than 1 word; else returns empty string</returns>
        private static string RemoveLastWord(this string input)
        {
            if (!input.HasText())
                return string.Empty;

            var strPos = input.LastIndexOf(" ", StringComparison.Ordinal);
            return strPos > 0 ? input.Substring(0, strPos) : string.Empty;
        }
        #endregion

        #endregion
    }
}
