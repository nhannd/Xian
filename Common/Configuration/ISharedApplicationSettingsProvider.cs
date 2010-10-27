#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    public interface ISharedApplicationSettingsProvider
    {
        //void ResetSharedPropertyValues();
		bool CanUpgradeSharedPropertyValues(SettingsContext context);

        void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties,
                                         string previousExeConfigFilename);

    	SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context,
    	                                                                SettingsPropertyCollection properties,
    	                                                                string previousExeConfigFilename);
		       
        SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties);

        void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values);
    }
}