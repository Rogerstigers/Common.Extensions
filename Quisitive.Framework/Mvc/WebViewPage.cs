using System;
using System.IO;

using Quisitive.Framework.UI.Resources;

namespace Quisitive.Framework.Mvc
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        #region Private Variables

        #endregion


        #region Constructors

        #endregion


        #region Public Properties

        public TextWriter Writer
        {
            get { return Html.ViewContext.Writer; }
        }

        #endregion


        #region Public Methods

        #region HasText
        public bool HasText(object thing)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(thing));
        }
        #endregion

        #region SetMeta
        public void SetMeta(string name = null, string content = null, string httpEquiv = null, string charset = null,
            object attributes = null)
        {
            var metaEntry = new MetaEntry();

            if (!String.IsNullOrEmpty(name))
            {
                metaEntry.Name = name;
            }

            if (!String.IsNullOrEmpty(content))
            {
                metaEntry.Content = content;
            }

            if (!String.IsNullOrEmpty(httpEquiv))
            {
                metaEntry.HttpEquiv = httpEquiv;
            }

            if (!String.IsNullOrEmpty(charset))
            {
                metaEntry.Charset = charset;
            }

            if (attributes != null)
                metaEntry.AddAttributes(attributes);

            Writer.WriteLine(metaEntry.GetTag());
        }
        #endregion

        #region RegisterLink
        public void RegisterLink(string rel = null, string type = null, string title = null, string href = null,
            string condition = null, object attributes = null)
        {
            var linkEntry = new LinkEntry();

            if (!String.IsNullOrEmpty(rel))
            {
                linkEntry.Rel = rel;
            }

            if (!String.IsNullOrEmpty(type))
            {
                linkEntry.Type = type;
            }

            if (!String.IsNullOrEmpty(title))
            {
                linkEntry.Title = title;
            }

            if (!String.IsNullOrEmpty(href))
            {
                linkEntry.Href = href;
            }

            if (!String.IsNullOrEmpty(condition))
            {
                linkEntry.Condition = condition;
            }

            if (attributes != null)
                linkEntry.AddAttributes(attributes);

            Writer.WriteLine(linkEntry.GetTag());
        }
        #endregion

        #region Tag
        public Tag Tag(string tagId, string tagName, object attributes = null)
        {
            var tag = new Tag(tagName);
            tag.GenerateId(tagId);

            if (attributes != null)
                tag.AddAttributes(attributes);

            return tag;
        }
        #endregion

        #endregion
    }

    public abstract class WebViewPage : WebViewPage<dynamic> { }
}
