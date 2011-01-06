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

namespace ClearCanvas.ImageViewer.Annotations
{
	[SettingsGroupDescription("Stores arbitrary text overlay configurations in a common place.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	[UserSettingsMigrationDisabled]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class AnnotationLayoutStoreSettings
	{
		private AnnotationLayoutStoreSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public override void Upgrade()
		{
	}
	}
}
