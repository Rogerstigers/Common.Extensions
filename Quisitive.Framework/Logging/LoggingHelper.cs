using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quisitive.Framework.Logging
{
    public static class LoggingHelper
    {
        public static List<String> BuildErrorMessageCollection(Exception exception)
        {
            List<String> errors = new List<string>();
            if (exception != null)
            {
                errors.Add(exception.Message);

                Exception innerException = exception.InnerException;
                while (innerException != null)
                {
                    errors.Add(innerException.Message);
                    innerException = innerException.InnerException;
                }
            }
            return errors;
        }
        public static string StringifyErrorCollection(List<string> errors)
        {
            var builder = new StringBuilder();
            foreach (var error in errors)
                builder.AppendFormat("{0};", error);
            return builder.ToString();
        }
    }
}
