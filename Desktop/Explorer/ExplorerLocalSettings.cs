#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	[SettingsGroupDescription("Local settings for how this client installation should launch the Explorer.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class ExplorerLocalSettings
	{
		private ExplorerLocalSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
