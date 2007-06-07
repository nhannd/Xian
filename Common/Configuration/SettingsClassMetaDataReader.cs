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
    [Serializable]
    public enum SettingScope
    {
        Application,
        User
    }

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
		/// Obtains the default value of a setting from the <see cref="DefaultSettingValueAttribute"/>.
		/// If translate is true, and the value is the name of an embedded resource, it is automatically translated.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="translate"></param>
		/// <returns></returns>
		public static string GetDefaultValue(PropertyInfo property, bool translate)
		{
			DefaultSettingValueAttribute a = CollectionUtils.FirstElement<DefaultSettingValueAttribute>(
				property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false));

			if (a == null)
				return "";

			if (!translate)
				return a.Value;

			return TranslateDefaultValue(property.ReflectedType, a.Value);
		}
		
		/// <summary>
        /// Obtains the default value of a setting from the <see cref="DefaultSettingValueAttribute"/>.
        /// If the value is the name of an embedded resource, it is automatically translated.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDefaultValue(PropertyInfo property)
        {
			return GetDefaultValue(property, true);
        }

        /// <summary>
        /// If the specified raw value is the name of an embedded resource (embedded in the same
        /// assembly as the specified settings class), the contents of the resource are returned.
        /// Otherwise, the raw value is simply returned.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        public static string TranslateDefaultValue(Type settingsClass, string rawValue)
        {
            // does the raw value look like it could be an embedded resource?
            if (Regex.IsMatch(rawValue, @"^([\w]+\.)+\w+$"))
            {
                try
                {
                    // try to open the resource
                    IResourceResolver resolver = new ResourceResolver(settingsClass.Assembly);
                    using (Stream resourceStream = resolver.OpenResource(rawValue))
                    {
                        StreamReader r = new StreamReader(resourceStream);
                        return r.ReadToEnd();
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // guess it was not an embedded resource, so return the raw value
                    return rawValue;
                }
            }
            else
            {
                return rawValue;
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
        /// Returns a string describing the scope of the property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static SettingScope GetScope(PropertyInfo property)
        {
            if(IsAppScoped(property))
                return SettingScope.Application;
            if(IsUserScoped(property))
                return SettingScope.User;

            throw new Exception("Settings scope not defined");
        }

        /// <summary>
        /// Returns the name of the settings property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetName(PropertyInfo property)
        {
            return property.Name;
        }

        /// <summary>
        /// Returns the <see cref="Type"/> of the settings property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Type GetType(PropertyInfo property)
        {
            return property.PropertyType;
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
