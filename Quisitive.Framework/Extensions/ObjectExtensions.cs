//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Quisitive.Framework.Extensions
{
    public static class LambdaExtensions
    {
        public static void SetPropertyValue<T>(this T target, Expression<Func<T, object>> memberLamda, object value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }
        }
        public static object GetPropertyValue<T>(this T target, Expression<Func<T, object>> memberLamda)
        {
            object o = null;
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    o = property.GetValue(target);
                }
            }
            return o;
        }
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

    }
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts the object to a serialized xml string
        /// </summary>
        /// <param name="o">The <see cref="object"/> to serialize</param>
        /// <returns></returns>
        public static string ToXml(this object o)
        {
            return ToXml(o, false);
        }

        /// <summary>
        /// Converts the object to a serialized xml string using the types FullName as the default namespace
        /// </summary>
        /// <param name="o">The <see cref="object"/> to serialize.</param>
        /// <param name="includeTypeFullNameInDefaultNamespace">if set to <c>true</c> [include type full name in default namespace].</param>
        /// <returns></returns>
        public static string ToXml(this object o, bool includeTypeFullNameInDefaultNamespace)
        {
            XmlSerializer xmlSerializer;
            try
            {
                if (includeTypeFullNameInDefaultNamespace)
                    xmlSerializer = new XmlSerializer(o.GetType(), o.GetType().FullName);
                else
                    xmlSerializer = new XmlSerializer(o.GetType());
            }
            catch (System.Exception)
            {
                return "Could not serialize the object of type " + o.GetType();
            }

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, o);
                return stringWriter.ToString();
            }
        }
        public static object MapObjects(object source, object target)
        {
            foreach (PropertyInfo sourceProp in source.GetType().GetProperties())
            {
                PropertyInfo targetProp = target.GetType().GetProperties().Where(p => p.Name == sourceProp.Name).FirstOrDefault();
                if (targetProp != null && targetProp.GetType().Name == sourceProp.GetType().Name)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
        }

        //public static T CloneJson<T>(this T source)
        //{
        //    // Don't serialize a null object, simply return the default for that object
        //    if (Object.ReferenceEquals(source, null))
        //    {
        //        return default(T);
        //    }

        //    // initialize inner objects individually
        //    // for example in default constructor some list property initialized with some values,
        //    // but in 'source' these items are cleaned -
        //    // without ObjectCreationHandling.Replace default constructor values will be added to result
        //    var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

        //    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        //}
    }
}
