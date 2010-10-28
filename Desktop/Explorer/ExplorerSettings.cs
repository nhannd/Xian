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

namespace ClearCanvas.Desktop.Explorer
{
	[SettingsGroupDescription("User settings for how/when the explorer is launched.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	[UserSettingsMigrationDisabled]
	internal sealed partial class ExplorerSettings
	{
		private ExplorerSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public override void Upgrade()
		{
	}
	}
}
