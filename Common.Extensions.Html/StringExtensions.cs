using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using Common.Extensions;
using HtmlAgilityPack;

namespace Common.Extensions.Html
{
    public static class StringExtensions
    {
        #region EncloseInTag
        public static string EncloseInTag(this string source, string tag)
        {
            if (source.IsNullOrWhiteSpace()) return source;

            if (tag.IsNullOrWhiteSpace()) return source.IsNotNullOrWhiteSpace() ? source : string.Empty;

            if (tag.Contains(" ")) tag = tag.Replace(" ", "");

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
            if (source.IsNullOrWhiteSpace())
                return string.Empty;

            if (tag.IsNullOrWhiteSpace())
                return source.IsNotNullOrWhiteSpace() ? source : string.Empty;

            if (attributeValues == null)
                return source.IsNotNullOrWhiteSpace() ? source : string.Empty;

            if (tag.Contains(" "))
                tag = tag.Replace(" ", "");

            // add the tag attributes
            var tagAttributesAndValues = "";

            // special case:
            // tag is anchor and href key does not exist
            if (tag == "a" && !attributeValues.ContainsKey("href"))
                // return the original source
                return source.IsNotNullOrWhiteSpace() ? source : string.Empty;

            // loop over dictionary object
            foreach (var attributeValue in attributeValues)
            {
                // if the strict tag display mode is on and the attribute value is empty
                if (!displayTagWithNoAttributes && string.IsNullOrEmpty(attributeValue.Value))
                {
                    // return the original source, exiting the loop
                    return source.IsNotNullOrWhiteSpace() ? source : string.Empty;
                }
                // if the attribute key is empty, return
                if (attributeValue.Key.IsNullOrWhiteSpace())
                {
                    // return the original source, exiting the loop
                    return source.IsNotNullOrWhiteSpace() ? source : string.Empty;
                }
                // construct the attribute key-value pairs
                // Only constructs this attribute if Attribute has text and Attribute Value is not Empty
                if (attributeValue.Key.IsNotNullOrWhiteSpace() && attributeValue.Value.IsNotNullOrWhiteSpace())
                {
                    tagAttributesAndValues = $"{tagAttributesAndValues} {attributeValue.Key}=\"{attributeValue.Value}\"";
                }
            }

            // If the strict tag display mode is on and the attributes are empty
            // or if this is an anchor tag and the href has no value
            if ((tagAttributesAndValues.IsNullOrWhiteSpace() && !displayTagWithNoAttributes) || (tag == "a" && attributeValues["href"].IsNullOrWhiteSpace()))
            {
                // return the original source
                return source.IsNotNullOrWhiteSpace() ? source : string.Empty;
            }
            else
            {
                // return the formatted tag
                return string.Format("<{0}{2}>{1}</{0}>", tag, source, tagAttributesAndValues);
            }
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

                    if (lastText.IsNullOrWhiteSpace())
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
                            if (navigator.Value.IsNotNullOrWhiteSpace())
                            {
                                if (navigator.HasChildren)
                                {
                                    // get the text node inside the last child element if any
                                    var childNavigator = GetLastChildPosition(navigator);
                                    childNavigator.MoveToFollowing(XPathNodeType.Text);
                                    if (childNavigator.Value.IsNotNullOrWhiteSpace())
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
    }
}
