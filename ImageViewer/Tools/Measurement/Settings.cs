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

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[SettingsGroupDescription("Stores settings for measurement tools.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class Settings
	{
		public Settings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}