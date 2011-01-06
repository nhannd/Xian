#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    class LanguageAlert : PatientProfileAlertBase
    {
		public override string Id
		{
			get { return "LanguageAlert"; }
		}
		
		public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
			AlertsSettings settings = new AlertsSettings();
            List<string> defaultLanguages = string.IsNullOrEmpty(settings.CommonSpokenLanguages)
                ? new List<string>()
                : new List<string>(settings.CommonSpokenLanguages.Replace(" ", "").Split(','));

            if (profile.PrimaryLanguage != null && !defaultLanguages.Contains(profile.PrimaryLanguage.Code))
            {
                return new AlertNotification(this.Id, new string[] { profile.PrimaryLanguage.Value });
            }

            return null;
        }
    }
}
