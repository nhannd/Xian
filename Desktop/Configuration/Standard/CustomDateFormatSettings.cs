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

namespace ClearCanvas.Desktop.Configuration.Standard
{

	[SettingsGroupDescription("Provides a list of custom date formats the user can select from to set their own preferred date format.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class CustomDateFormatSettings
	{
		private CustomDateFormatSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public System.Collections.Specialized.StringCollection AvailableFormats
		{
			get
			{
				return AvailableCustomFormats;
			}
		}
	}
}
