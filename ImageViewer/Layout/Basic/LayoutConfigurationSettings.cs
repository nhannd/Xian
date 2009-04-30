#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[SettingsGroupDescription("Stores the user's initial layout preferences for studies opened in the viewer")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class LayoutConfigurationSettings
	{
		public static readonly int DefaultImageBoxRows = 1;
		public static readonly int DefaultImageBoxColumns = 2;
		public static readonly int DefaultTileRows = 1;
		public static readonly int DefaultTileColumns = 1;

		public static readonly int MaximumImageBoxRows = 4;
		public static readonly int MaximumImageBoxColumns = 8;
		public static readonly int MaximumTileRows = 4;
		public static readonly int MaximumTileColumns = 4;

		private LayoutConfigurationSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public StoredLayoutConfiguration DefaultConfiguration
		{
			get
			{
				return this.GetDefaultLayoutConfiguration(this.LayoutConfigurations);
			}
		}
		
		public IList<StoredLayoutConfiguration> LayoutConfigurations
		{
			get
			{
				List<StoredLayoutConfiguration> layoutConfigurations = new List<StoredLayoutConfiguration>();
				XmlDocument document = GetLayoutSettingsDocument(this.LayoutConfigurationSettingsXml);

				StoredLayoutConfiguration defaultConfiguration = null;

				XmlNodeList layoutConfigurationNodes = document.SelectNodes("//layout");
				foreach (XmlElement layoutConfigurationNode in layoutConfigurationNodes)
				{
					StoredLayoutConfiguration newConfiguration = ConstructDefaultConfiguration(layoutConfigurationNode.GetAttribute("modality"));

					newConfiguration.ImageBoxRows = Convert.ToInt32(layoutConfigurationNode.GetAttribute("image-box-rows"), System.Globalization.CultureInfo.InvariantCulture);
					newConfiguration.ImageBoxColumns = Convert.ToInt32(layoutConfigurationNode.GetAttribute("image-box-columns"), System.Globalization.CultureInfo.InvariantCulture);
					newConfiguration.TileRows = Convert.ToInt32(layoutConfigurationNode.GetAttribute("tile-rows"), System.Globalization.CultureInfo.InvariantCulture);
					newConfiguration.TileColumns = Convert.ToInt32(layoutConfigurationNode.GetAttribute("tile-columns"), System.Globalization.CultureInfo.InvariantCulture);

					//push the default to the end.
					if (newConfiguration.IsDefault)
					{
						//make sure there's only one default!
						if (defaultConfiguration == null)
							defaultConfiguration = newConfiguration;

						continue;
					}

					layoutConfigurations.Add(newConfiguration);
				}

				if (defaultConfiguration == null)
					defaultConfiguration = ConstructDefaultConfiguration();

				layoutConfigurations.Add(defaultConfiguration);

				return layoutConfigurations;
			}
			set
			{
				XmlDocument document = GetLayoutSettingsDocument("");
				XmlElement layoutConfigurationsNode = (XmlElement)document.SelectSingleNode("//layouts");

				if (value != null)
				{
					foreach (StoredLayoutConfiguration configuration in value)
					{
						XmlElement newLayoutConfigurationNode = document.CreateElement("layout");
						
						newLayoutConfigurationNode.SetAttribute("modality", configuration.Modality);
						newLayoutConfigurationNode.SetAttribute("image-box-rows", configuration.ImageBoxRows.ToString(System.Globalization.CultureInfo.InvariantCulture));
						newLayoutConfigurationNode.SetAttribute("image-box-columns", configuration.ImageBoxColumns.ToString(System.Globalization.CultureInfo.InvariantCulture));
						newLayoutConfigurationNode.SetAttribute("tile-rows", configuration.TileRows.ToString(System.Globalization.CultureInfo.InvariantCulture));
						newLayoutConfigurationNode.SetAttribute("tile-columns", configuration.TileColumns.ToString(System.Globalization.CultureInfo.InvariantCulture));

						layoutConfigurationsNode.AppendChild(newLayoutConfigurationNode);
					}
				}

				this.LayoutConfigurationSettingsXml = document.OuterXml;
				this.Save();
			}
		}

		public StoredLayoutConfiguration GetLayoutConfiguration(string modality)
		{
			IList<StoredLayoutConfiguration> layoutConfigurations = this.LayoutConfigurations;

			foreach (StoredLayoutConfiguration configuration in layoutConfigurations)
			{
				if (!configuration.IsDefault && configuration.Modality == modality)
					return configuration;
			}

			return GetDefaultLayoutConfiguration(layoutConfigurations);
		}

		public StoredLayoutConfiguration GetLayoutConfiguration(ImageSop imageSop)
		{
			if (imageSop == null)
				return this.DefaultConfiguration;

			return GetLayoutConfiguration(imageSop.Modality);
		}

		public StoredLayoutConfiguration GetLayoutConfiguration(IImageSopProvider imageSopProvider)
		{
			if (imageSopProvider == null)
				return this.DefaultConfiguration;

			return GetLayoutConfiguration(imageSopProvider.ImageSop);
		}

		public static StoredLayoutConfiguration GetMinimumConfiguration()
		{
			return new StoredLayoutConfiguration("", 1, 1, 1, 1);
		}

		private StoredLayoutConfiguration ConstructDefaultConfiguration()
		{
			return ConstructDefaultConfiguration("");
		}

		private static StoredLayoutConfiguration ConstructDefaultConfiguration(string modality)
		{
			return new StoredLayoutConfiguration(modality, DefaultImageBoxRows,
														DefaultImageBoxColumns,
														DefaultTileRows,
														DefaultTileColumns);
		}

		private StoredLayoutConfiguration GetDefaultLayoutConfiguration(IList<StoredLayoutConfiguration> layoutConfigurations)
		{
			foreach (StoredLayoutConfiguration configuration in layoutConfigurations)
			{
				if (configuration.IsDefault)
					return configuration;
			}

			return ConstructDefaultConfiguration();
		}

		private XmlDocument GetLayoutSettingsDocument(string layoutConfigurationSettingsXml)
		{
			XmlDocument document = new XmlDocument();

			if (!String.IsNullOrEmpty(layoutConfigurationSettingsXml))
			{
				document.LoadXml(layoutConfigurationSettingsXml);
			}
			else
			{
				XmlElement root = document.CreateElement("layout-configuration");
				root.AppendChild(document.CreateElement("layouts"));
				document.AppendChild(root);
			}

			return document;
		}
	}
}
