using System;
using System.Configuration;
using System.Collections.Generic;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// TODO this stuff should be moved into dictionaries, not settings
    /// </summary>
    [SettingsGroupDescriptionAttribute("Provides lists of countries and provinces")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class AddressEditorComponentSettings
    {

        private AddressEditorComponentSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }

        public ICollection<string> CountryChoices
        {
            get
            {
                return CollectionUtils.Map<string, string>(this.Countries.Split(','),
                   delegate(string s) { return s.Trim(); });
            }
        }

        public ICollection<string> ProvinceChoices
        {
            get
            {
                return CollectionUtils.Map<string, string>(this.Provinces.Split(','),
                   delegate(string s) { return s.Trim(); });
            }
        }



    }
}
