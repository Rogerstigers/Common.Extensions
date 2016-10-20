//using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;

namespace Common.Extensions
{
    public static class LambdaExtensions
    {
        public static void SetPropertyValue<T>(this T target, Expression<Func<T, object>> memberLamda, object value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null) return;

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null) return;

            property.SetValue(target, value, null);
        }

        public static object GetPropertyValue<T>(this T target, Expression<Func<T, object>> memberLamda)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null) return null;

            var property = memberSelectorExpression.Member as PropertyInfo;
            return property == null ? null : property.GetValue(target);
        }
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null) throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            var propType = propInfo.ReflectedType;

            if (propType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propType))
                throw new ArgumentException($"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }

    }
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts the object to a serialized xml string using the types FullName as the default namespace
        /// </summary>
        /// <param name="o">The <see cref="object"/> to serialize.</param>
        /// <param name="includeTypeFullNameInDefaultNamespace">if set to <c>true</c> [include type full name in default namespace].</param>
        /// <returns></returns>
        public static string ToXml(this object o, bool includeTypeFullNameInDefaultNamespace = false)
        {
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = includeTypeFullNameInDefaultNamespace ?
                                        new XmlSerializer(o.GetType(), o.GetType().FullName) :
                                        new XmlSerializer(o.GetType());
            }
            catch 
            {
                return $"Could not serialize the object of type {o.GetType()}";
            }

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, o);
                return stringWriter.ToString();
            }
        }
        public static object MapObjects(object source, object target)
        {
            foreach (var sourceProp in source.GetType().GetProperties())
            {
                var targetProp = target.GetType().GetProperties().FirstOrDefault(p => p.Name == sourceProp.Name);
                if (targetProp != null && targetProp.GetType().Name == sourceProp.GetType().Name)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
        }
    }
}
