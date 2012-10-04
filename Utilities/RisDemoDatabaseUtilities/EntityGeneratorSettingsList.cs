#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    public abstract class EntityGeneratorSettingsList : IEntityGeneratorSettingsList
    {
        protected List<EntityGeneratorSetting> _settings;

        protected EntityGeneratorSettingsList()
        {
        }

        #region IEntityGeneratorSettingsList Members

        public IEnumerable<EntityGeneratorSetting> Settings
        {
            get { return _settings; }
        }

        public abstract bool SettingsValid();

        public abstract IEntityGeneratorSettingsList Copy();

        #endregion
        
        protected object GetSettingValue(string id)
        {
            EntityGeneratorSetting match = _settings.Find(delegate(EntityGeneratorSetting setting) { return setting.Name == id; });
            return match.Setting;
        }

        protected object GetSettingsValueFromArray(object[] settingsArray)
        {
            string settingvalue = null;
            foreach (object setting in settingsArray)
            {
                settingvalue += setting.ToString() + ";";
            }

            return settingvalue;
        }

        protected Array GetArrayFromEnumArraySettingsValue(string id, Type enumType)
        {
            string settingValue = GetSettingValue(id).ToString();
            //toss exception if setting to string fails
            //List<object> values = new List<object>();
            ArrayList values = new ArrayList();

            int index = settingValue.IndexOf(';');
            while (index != -1 && settingValue.Length > 0)
            {
                values.Add(Enum.Parse(enumType, settingValue.Substring(0, index)));
                settingValue = settingValue.Substring(index + 1, settingValue.Length - (index + 1));
                index = settingValue.IndexOf(';');
            }

            if (!settingValue.EndsWith(";") && settingValue.Length > 0)
                values.Add(Enum.Parse(enumType, settingValue));

            return values.ToArray(enumType);
        }

        protected int GetIntFromSettingsValue(string id)
        {
            return int.Parse(GetSettingValue(id).ToString());
        }

        protected string[] GetArrayFromStringArraySettingsValue(string id)
        {
            List<string> values = new List<string>();
            string settingValue = GetSettingValue(id).ToString();

            int index = settingValue.IndexOf(';');
            while (index != -1 && settingValue.Length > 0)
            {
                values.Add(settingValue.Substring(0, index));
                settingValue = settingValue.Substring(index + 1, settingValue.Length - (index + 1));
                index = settingValue.IndexOf(';');
            }

            if (!settingValue.EndsWith(";") && settingValue.Length > 0)
                values.Add(settingValue);

            return values.ToArray();
        }
    }
}
