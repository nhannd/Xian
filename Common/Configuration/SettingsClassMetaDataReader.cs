using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using ClearCanvas.Common.Utilities;
using System.Text.RegularExpressions;
using System.IO;
using System.Resources;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Utility class for reading meta-data associated with a settings class
    /// (a subclass of <see cref="SettingsBase"/>)
    /// </summary>
    public static class SettingsClassMetaDataReader
    {
        /// <summary>
        /// Obtains the version of the settings class, which is always the version of the assembly
        /// in which the class is contained.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <returns></returns>
        public static Version GetVersion(Type settingsClass)
        {
            return settingsClass.Assembly.GetName().Version;
        }

        /// <summary>
        /// Obtains the name of the settings group, which is always the full name of the settings class.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <returns></returns>
        public static string GetGroupName(Type settingsClass)
        {
            return settingsClass.FullName;
        }

        /// <summary>
        /// Obtains the settings group description from the <see cref="SettingsGroupDescriptionAttribute"/>
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <returns></returns>
        public static string GetGroupDescription(Type settingsClass)
        {
            SettingsGroupDescriptionAttribute a = CollectionUtils.FirstElement<SettingsGroupDescriptionAttribute>(
                settingsClass.GetCustomAttributes(typeof(SettingsGroupDescriptionAttribute), false));

            return a == null ? "" : a.Description;
        }

        /// <summary>
        /// Obtains the collection of properties that represent settings.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <returns></returns>
        public static ICollection<PropertyInfo> GetSettingsProperties(Type settingsClass)
        {
            return CollectionUtils.Select<PropertyInfo>(settingsClass.GetProperties(),
                delegate(PropertyInfo property) { return IsUserScoped(property) || IsAppScoped(property); });
        }

        /// <summary>
        /// Obtains the default value of a setting from the <see cref="DefaultSettingValueAttribute"/>
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDefaultValue(PropertyInfo property)
        {
            DefaultSettingValueAttribute a = CollectionUtils.FirstElement<DefaultSettingValueAttribute>(
                property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false));

            if (a == null)
                return "";

            // does the default value look like it could be an embedded resource?
            if (Regex.IsMatch(a.Value, @"^([\w]+\.)+\w+$"))
            {
                try
                {
                    // try to open the resource
                    IResourceResolver resolver = new ResourceResolver(property.ReflectedType.Assembly);
                    using (Stream resourceStream = resolver.OpenResource(a.Value))
                    {
                        StreamReader r = new StreamReader(resourceStream);
                        return r.ReadToEnd();
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // guess it was not an embedded resource, so return the literal value
                    return a.Value;
                }
            }
            else
            {
                return a.Value;
            }
        }

        /// <summary>
        /// Obtains the setting description from the <see cref="SettingsDescriptionAttribute"/>
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDescription(PropertyInfo property)
        {
            SettingsDescriptionAttribute a = CollectionUtils.FirstElement<SettingsDescriptionAttribute>(
                property.GetCustomAttributes(typeof(SettingsDescriptionAttribute), false));

            return a == null ? "" : a.Description;
        }

        /// <summary>
        /// Returns true if the property is decorated with a <see cref="UserScopedSettingAttribute"/>
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsUserScoped(PropertyInfo property)
        {
            UserScopedSettingAttribute a = CollectionUtils.FirstElement<UserScopedSettingAttribute>(
                property.GetCustomAttributes(typeof(UserScopedSettingAttribute), false));

            return a != null;
        }

        /// <summary>
        /// Returns true if the property is decorated with a <see cref="ApplicationScopedSettingAttribute"/>
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsAppScoped(PropertyInfo property)
        {
            ApplicationScopedSettingAttribute a = CollectionUtils.FirstElement<ApplicationScopedSettingAttribute>(
                property.GetCustomAttributes(typeof(ApplicationScopedSettingAttribute), false));

            return a != null;
        }
    }
}
