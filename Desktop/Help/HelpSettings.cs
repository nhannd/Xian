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
using System.Configuration;
using System.Text;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.Help {
	[SettingsGroupDescription("Configures the behaviour of application Help.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	partial class HelpSettings
	{
		public HelpSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
