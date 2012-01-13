#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider, IConfigurationProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
				yield return new ConfigurationPage(MprConfigurationComponent.Path, new MprConfigurationComponent());
		}

		#endregion

		#region Implementation of IConfigurationProvider

		public string SettingsClassName
		{
			get { return typeof(MprSettings).FullName; }
		}

		public void UpdateConfiguration(Dictionary<string, string> settings)
		{
			foreach (var key in settings.Keys)
			{
				switch (key)
				{
					case "SliceSpacingFactor":
						MprSettings.Default.SliceSpacingFactor = float.Parse(settings[key]);
						break;
					case "AutoSliceSpacing":
						MprSettings.Default.AutoSliceSpacing = bool.Parse(settings[key]);
						break;
					default:
						var message = string.Format("{0} with key={1} is not implemented", this.SettingsClassName, key);
						throw new NotImplementedException(message);
				}
			}

			MprSettings.Default.Save();
		}

		#endregion
	}
}