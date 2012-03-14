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

namespace ClearCanvas.ImageViewer.Common
{
	[SettingsGroupDescription("Application settings for memory management on the local machine.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	internal sealed partial class MemoryManagementSettings
	{
		private MemoryManagementSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
