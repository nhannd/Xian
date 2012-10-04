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

using ClearCanvas.Healthcare;
using System.Collections;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    public class VisitGeneratorSettings : EntityGeneratorSettingsList, IEntityGeneratorSettingsList
    {
        public VisitGeneratorSettings()
        {
            _settings = new List<EntityGeneratorSetting>();
            _settings.Add(new EntityGeneratorSetting("MinVisits", 1));
            _settings.Add(new EntityGeneratorSetting("MaxVisits", 2));
            _settings.Add(new EntityGeneratorSetting("MinVisitLocations", 1));
            _settings.Add(new EntityGeneratorSetting("MaxVisitLocations", 2));
            _settings.Add(new EntityGeneratorSetting("AssigningAuthorityEnumValues", GetSettingsValueFromArray(new object[4] { "UHN", "UHN", "UHN", "MSH" })));
        }

        public override bool SettingsValid()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IEntityGeneratorSettingsList Copy()
        {
            VisitGeneratorSettings visitSettings = new VisitGeneratorSettings();
            foreach (EntityGeneratorSetting setting in this.Settings)
            {
                visitSettings._settings.Add(new EntityGeneratorSetting(setting.Name, setting.Setting));
            }

            return visitSettings;
        }

        public string[] AssigningAuthorityEnumValues
        {
            get { return (string[])GetArrayFromStringArraySettingsValue("AssigningAuthorityEnumValues"); }
        }

        public int MinVisitLocations
        {
            get { return GetIntFromSettingsValue("MinVisitLocations"); }
        }

        public int MaxVisitLocations
        {
            get { return GetIntFromSettingsValue("MaxVisitLocations"); }
        }

        public int MinVisits
        {
            get { return GetIntFromSettingsValue("MinVisits"); }
        }

        public int MaxVisits
        {
            get { return GetIntFromSettingsValue("MaxVisits"); }
        }
    }
}
