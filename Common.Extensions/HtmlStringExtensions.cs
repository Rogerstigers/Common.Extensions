using System;
using System.Web;

namespace Common.Extensions
{
   public static class HtmlStringExtensions
   {
      public static bool IsNullOrEmpty(this HtmlString value) => string.IsNullOrEmpty(value?.ToString());

      public static bool IsNotNullOrEmpty(this HtmlString value)=> !string.IsNullOrEmpty(value?.ToString());
   }
}
