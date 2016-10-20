using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Quisitive.Framework.Extensions
{
    public static class HttpContextExtensions
    {
        public static void AddContextListItem<T>(this HttpContext context, string ListName, T item) {
            if (!context.Items.Contains(ListName)) {
                context.Items[ListName] = new List<T>();
            }

            if (!(context.Items[ListName] as List<T>).Contains(item)) {
                (context.Items[ListName] as List<T>).Add(item);
            }
        }

        public static List<T> ContextListItems<T>(this HttpContext context, string ListName)
        {
            if (!context.Items.Contains(ListName))
            {
                context.Items[ListName] = new List<T>();
            }

            return context.Items[ListName] as List<T>;
        }
    }
}
