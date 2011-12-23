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

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[SettingsGroupDescription("Allows users to configure certain characteristics of the DICOM Explorer.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class DicomExplorerConfigurationSettings
	{
		private DicomExplorerConfigurationSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}