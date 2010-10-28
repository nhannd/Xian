#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    [SettingsGroupDescriptionAttribute("Configures the Address Editor component.")]
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
