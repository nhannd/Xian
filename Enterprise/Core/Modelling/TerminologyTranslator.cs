using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Provides methods for translating domain object class and property names into user-friendly
    /// equivalents.
    /// </summary>
    public static class TerminologyTranslator
    {
        /// <summary>
        /// Translates the name of the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string Translate(PropertyInfo property)
        {
            return Translate(property.ReflectedType, property.Name);
        }

        /// <summary>
        /// Translates the name of the specified property on the specified domain class.
        /// </summary>
        /// <param name="domainClass"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string Translate(Type domainClass, string propertyName)
        {
            IResourceResolver resolver = new ResourceResolver(domainClass.Assembly);

            string key = domainClass.Name + propertyName;
            string localized = resolver.LocalizeString(key);
            if (localized == key)
                localized = resolver.LocalizeString(propertyName);

            return localized;
        }

        /// <summary>
        /// Translates the name of the specified domain class.
        /// </summary>
        /// <param name="domainClass"></param>
        /// <returns></returns>
        public static string Translate(Type domainClass)
        {
            IResourceResolver resolver = new ResourceResolver(domainClass.Assembly);
            return resolver.LocalizeString(domainClass.Name);
        }
    }
}
