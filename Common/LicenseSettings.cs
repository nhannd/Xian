#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Common
{
	[SettingsGroupDescription("Settings that store the license information.")]
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class LicenseSettings {}
}