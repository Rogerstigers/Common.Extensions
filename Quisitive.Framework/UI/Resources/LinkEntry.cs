using System.Web.Mvc;

namespace Quisitive.Framework.UI.Resources
{
    public class LinkEntry
    {
        #region Private Variables 

        private readonly TagBuilder _builder = new TagBuilder("link");

        #endregion


        #region Public Methods

        #region GetTag
        public string GetTag()
        {
            var tag = _builder.ToString(TagRenderMode.SelfClosing);
            if (!string.IsNullOrEmpty(Condition))
            {
                return string.Format("<!--[if {0}]>{1}<![endif]-->", Condition, tag);
            }
            return tag;
        }
        #endregion

        #region AddAttribute
        public LinkEntry AddAttribute(string name, string value)
        {
            _builder.MergeAttribute(name, value);
            return this;
        }
        #endregion

        #region AddAttributes
        public LinkEntry AddAttributes(object attributes)
        {
            foreach (var property in attributes.GetType().GetProperties())
            {
                _builder.MergeAttribute(property.Name, property.GetValue(attributes).ToString());
            }
            return this;
        }
        #endregion

        #region SetAttribute
        public LinkEntry SetAttribute(string name, string value)
        {
            _builder.MergeAttribute(name, value, true);
            return this;
        }
        #endregion

        #endregion


        #region Public Properties

        public string Condition { get; set; }

        public string Rel
        {
            get
            {
                string value;
                _builder.Attributes.TryGetValue("rel", out value);
                return value;
            }
            set { SetAttribute("rel", value); }
        }

        public string Type
        {
            get
            {
                string value;
                _builder.Attributes.TryGetValue("type", out value);
                return value;
            }
            set { SetAttribute("type", value); }
        }

        public string Title
        {
            get
            {
                string value;
                _builder.Attributes.TryGetValue("title", out value);
                return value;
            }
            set { SetAttribute("title", value); }
        }

        public string Href
        {
            get
            {
                string value;
                _builder.Attributes.TryGetValue("href", out value);
                return value;
            }
            set { SetAttribute("href", value); }
        }

        #endregion
    }
}
