#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Help.UpdateTools
{
	[SettingsGroupDescription("Settings for the update tools.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class Settings
	{
		private Settings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
