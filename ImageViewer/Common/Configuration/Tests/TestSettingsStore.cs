#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.Configuration.Tests
{

    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class TestSystemConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(ISystemConfigurationSettingsStore))
                return null;

            return TestSettingsStore.Instance;
        }

        #endregion
    }

    class TestSettingsStore : ISystemConfigurationSettingsStore
    {
        public static TestSettingsStore Instance = new TestSettingsStore();
        public static string TestString = "Prior Setting";

        private class Setting
        {
            public SettingsGroupDescriptor Group;
            public Dictionary<string, string> Values;

        }

        private List<Setting> _settingsList = new List<Setting>();

        public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor @group)
        {
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    var newGroup = new SettingsGroupDescriptor(@group.Name, new Version(0, 9), @group.Description,
                                                               @group.AssemblyQualifiedTypeName,
                                                               @group.HasUserScopedSettings);
                    var newSetting = new Setting
                                         {
                                             Group = newGroup,
                                             Values = new Dictionary<string, string>()
                                         };
                    _settingsList.Add(newSetting);

                    foreach (var key in setting.Values.Keys)
                    {
                        var val = setting.Values[key];

                        newSetting.Values[key] = val + TestString;
                    }
                    return newGroup;
                }
            }
            return null;
        }

        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey)
        {
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    return setting.Values;
                }
            }
            return new Dictionary<string, string>();
        }

        public void PutSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
        {
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    foreach (var key in dirtyValues.Keys)
                    {
                        string val = dirtyValues[key];
                        setting.Values[key] = val;
                    }

                    return;
                }
            }

            var newSetting = new Setting
                                 {
                                     Group = @group, 
                                     Values = dirtyValues
                                 };
            _settingsList.Add(newSetting);
        }

        public void Reset()
        {
            _settingsList = new List<Setting>();
        }
    }
}

#endif
