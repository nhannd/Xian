#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[SettingsGroupDescription("Provides settings for control of services.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	internal sealed partial class ServiceControlSettings : IMigrateSettings
	{
		private ServiceControlSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			if (migrationValues.PropertyName == "TimeoutSeconds")
				migrationValues.CurrentValue = migrationValues.PreviousValue;
	}

		#endregion
	}
}