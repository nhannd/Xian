using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Configuration
{
    internal class SettingsImporter
    {
        public void Import(Type settingsClass, ConfigSettingsGroup group)
        {
            if (!settingsClass.IsSubclassOf(typeof(SettingsBase)))
                throw new ArgumentException(string.Format("Must be a subclass of {0}", typeof(SettingsBase).FullName));

            group.GroupName = SettingsClassMetaDataReader.GetGroupName(settingsClass);
            group.Description = SettingsClassMetaDataReader.GetGroupDescription(settingsClass);
            group.PluginVersion = SettingsClassMetaDataReader.GetVersion(settingsClass);

            group.Settings.Clear();
            foreach (PropertyInfo fi in settingsClass.GetProperties())
            {
                bool userScope = SettingsClassMetaDataReader.IsUserScoped(fi);
                bool appScope = SettingsClassMetaDataReader.IsAppScoped(fi);

                if (userScope && appScope)
                {
                    throw new Exception();  // TODO elaborate - can't be both
                }

                if (userScope || appScope)
                {
                    ConfigSettingScope scope = userScope ? ConfigSettingScope.User : ConfigSettingScope.Application;
                    string name = fi.Name;
                    string description = SettingsClassMetaDataReader.GetDescription(fi);
                    string defaultValue = SettingsClassMetaDataReader.GetDefaultValue(fi);

                    ConfigSetting setting = new ConfigSetting(name, description, scope, defaultValue);
                    group.Settings.Add(setting.Name, setting);
                }
            }
        }

    }
}
