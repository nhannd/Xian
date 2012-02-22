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

namespace ClearCanvas.Desktop
{
	[SettingsGroupDescription("Configures the policy controlling which localizations are available for use in the application.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class LocalePolicy
	{
		public LocalePolicy()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}