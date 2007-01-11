using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Configuration
{
    public static class SettingsClassMetaDataReader
    {
        public static string GetVersion(Type settingsClass)
        {
            return settingsClass.Assembly.GetName().Version.ToString();
        }

        public static string GetGroupDescription(Type settingsClass)
        {
            SettingsGroupDescriptionAttribute a = CollectionUtils.FirstElement<SettingsGroupDescriptionAttribute>(
                settingsClass.GetCustomAttributes(typeof(SettingsGroupDescriptionAttribute), false));

            return a == null ? "" : a.Description;
        }

        public static string GetGroupName(Type settingsClass)
        {
            return settingsClass.FullName;
        }

        public static string GetDefaultValue(PropertyInfo property)
        {
            DefaultSettingValueAttribute a = CollectionUtils.FirstElement<DefaultSettingValueAttribute>(
                property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false));

            return a == null ? "" : a.Value;
        }

        public static string GetDescription(PropertyInfo property)
        {
            SettingsDescriptionAttribute a = CollectionUtils.FirstElement<SettingsDescriptionAttribute>(
                property.GetCustomAttributes(typeof(SettingsDescriptionAttribute), false));

            return a == null ? "" : a.Description;
        }

        public static bool IsUserScoped(PropertyInfo property)
        {
            UserScopedSettingAttribute a = CollectionUtils.FirstElement<UserScopedSettingAttribute>(
                property.GetCustomAttributes(typeof(UserScopedSettingAttribute), false));

            return a != null;
        }

        public static bool IsAppScoped(PropertyInfo property)
        {
            ApplicationScopedSettingAttribute a = CollectionUtils.FirstElement<ApplicationScopedSettingAttribute>(
                property.GetCustomAttributes(typeof(ApplicationScopedSettingAttribute), false));

            return a != null;
        }
    }
}
