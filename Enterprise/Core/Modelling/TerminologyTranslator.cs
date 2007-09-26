using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public static class TerminologyTranslator
    {
        public static string Translate(PropertyInfo property)
        {
            return Translate(property.ReflectedType, property.Name);
        }

        public static string Translate(Type domainClass, string propertyName)
        {
            return Resolve(domainClass, domainClass.Name + propertyName);
        }

        public static string Translate(Type domainClass)
        {
            return Resolve(domainClass, domainClass.Name);
        }

        private static string Resolve(Type domainClass, string key)
        {
            return (new ResourceResolver(domainClass.Assembly)).LocalizeString(key);
        }
    }
}
