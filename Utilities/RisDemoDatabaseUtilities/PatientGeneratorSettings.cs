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
    public class PatientGeneratorSettings : EntityGeneratorSettingsList, IEntityGeneratorSettingsList
    {
        public PatientGeneratorSettings()
        {
            _settings = new List<EntityGeneratorSetting>();

            _settings.Add(new EntityGeneratorSetting("SexEnumValues", GetSettingsValueFromArray(new object[2] { Sex.M, Sex.F })));
            _settings.Add(new EntityGeneratorSetting("AssigningAuthorityEnumValues", GetSettingsValueFromArray(new object[4] { "UHN", "UHN", "UHN", "MSH" })));
            _settings.Add(new EntityGeneratorSetting("AddressStreetDirectionEnumValues", GetSettingsValueFromArray(new object[10] { "", "N.", "S.", "E.", "W.", "North", "South", "East", "West", "" })));
            _settings.Add(new EntityGeneratorSetting("MinAddresses", 1));
            _settings.Add(new EntityGeneratorSetting("MaxAddresses", 3));
            _settings.Add(new EntityGeneratorSetting("MinTelephoneNumbers", 1));
            _settings.Add(new EntityGeneratorSetting("MaxTelephoneNumbers", 3));
        }

        public override bool SettingsValid()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IEntityGeneratorSettingsList Copy()
        {
            PatientGeneratorSettings patientSettings = new PatientGeneratorSettings();
            foreach (EntityGeneratorSetting setting in this.Settings)
            {
                patientSettings._settings.Add(new EntityGeneratorSetting(setting.Name, setting.Setting));
            }

            return patientSettings;
        }

        public Sex[] SexEnumValues
        {
            get { return (Sex[]) GetArrayFromEnumArraySettingsValue("SexEnumValues", typeof(Sex)); }
        }

        public string[] AssigningAuthorityEnumValues
        {
            get { return (string[])GetArrayFromStringArraySettingsValue("AssigningAuthorityEnumValues"); }
        }

        public string[] AddressStreetDirectionEnumValues
        {
            get { return (string[])GetArrayFromStringArraySettingsValue("AddressStreetDirectionEnumValues"); }
        }

        public int MinAddresses
        {
            get { return GetIntFromSettingsValue("MinAddresses"); }
        }

        public int MaxAddresses
        {
            get { return GetIntFromSettingsValue("MaxAddresses"); }
        }

        public int MinTelephoneNumbers
        {
            get { return GetIntFromSettingsValue("MinTelephoneNumbers"); }
        }

        public int MaxTelephoneNumbers
        {
            get { return GetIntFromSettingsValue("MaxTelephoneNumbers"); }
        }
    }
}
