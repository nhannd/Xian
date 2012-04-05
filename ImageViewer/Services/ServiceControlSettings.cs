#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Services
{
	[SettingsGroupDescription("Provides settings for control of services.")]
	[SettingsProvider(typeof(ApplicationCriticalSettingsProvider))]
	internal sealed partial class ServiceControlSettings
	{
		private ServiceControlSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}