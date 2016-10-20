using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Quisitive.Framework.Extensions
{
   public static class HtmlStringExtensions
   {
      public static bool IsNullOrEmpty(this HtmlString value)
      {
         return value == null || (value != null && String.IsNullOrEmpty(value.ToString()));
      }

      public static bool IsNotNullOrEmpty(this HtmlString value)
      {
         return value != null && !String.IsNullOrEmpty(value.ToString());
      }
   }
}
