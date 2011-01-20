#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Externals.Config
{
	[SettingsGroupDescription("Settings for external applications.")]
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	internal sealed partial class ExternalsConfigurationSettings
	{
		private event EventHandler _externalsChanged;

		public event EventHandler ExternalsChanged
		{
			add { _externalsChanged += value; }
			remove { _externalsChanged -= value; }
		}

		protected override void OnSettingChanging(object sender, SettingChangingEventArgs e)
		{
			base.OnSettingChanging(sender, e);

			if (e.SettingName == "Externals")
			{
				EventsHelper.Fire(_externalsChanged, this, EventArgs.Empty);
			}
		}
	}
}