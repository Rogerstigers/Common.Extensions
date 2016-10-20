using System.Web;
using System.Web.Mvc;

namespace Quisitive.Framework.UI.Resources
{
    public class Tag : TagBuilder
    {
        #region Constructors

        public Tag(string tagName) : base(tagName) { }

        #endregion


        #region Public Properties

        public IHtmlString StartElement
        {
            get { return new HtmlString(ToString(TagRenderMode.StartTag)); }
        }

        public IHtmlString EndElement
        {
            get { return new HtmlString(ToString(TagRenderMode.EndTag)); }
        }

        #endregion


        #region Public Methods

        public Tag AddAttributes(object attributes)
        {
            foreach (var property in attributes.GetType().GetProperties())
            {
                MergeAttribute(property.Name, property.GetValue(attributes).ToString());
            }

            return this;
        }

        #endregion
    }
}
