using System;
using System.Configuration;
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
		private LayoutConfigurationSettings()
		{
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
					StoredLayoutConfiguration newConfiguration = new StoredLayoutConfiguration(layoutConfigurationNode.GetAttribute("modality"));
					
					newConfiguration.ImageBoxRows = Convert.ToInt32(layoutConfigurationNode.GetAttribute("image-box-rows"));
					newConfiguration.ImageBoxColumns = Convert.ToInt32(layoutConfigurationNode.GetAttribute("image-box-columns"));
					newConfiguration.TileRows = Convert.ToInt32(layoutConfigurationNode.GetAttribute("tile-rows"));
					newConfiguration.TileColumns = Convert.ToInt32(layoutConfigurationNode.GetAttribute("tile-columns"));

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
					defaultConfiguration = new StoredLayoutConfiguration();

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
						newLayoutConfigurationNode.SetAttribute("image-box-rows", configuration.ImageBoxRows.ToString());
						newLayoutConfigurationNode.SetAttribute("image-box-columns", configuration.ImageBoxColumns.ToString());
						newLayoutConfigurationNode.SetAttribute("tile-rows", configuration.TileRows.ToString());
						newLayoutConfigurationNode.SetAttribute("tile-columns", configuration.TileColumns.ToString());

						layoutConfigurationsNode.AppendChild(newLayoutConfigurationNode);
					}
				}

				this.LayoutConfigurationSettingsXml = document.OuterXml;
				this.Save();
			}
		}

		private StoredLayoutConfiguration GetDefaultLayoutConfiguration(IList<StoredLayoutConfiguration> layoutConfigurations)
		{
			foreach (StoredLayoutConfiguration configuration in layoutConfigurations)
			{
				if (configuration.IsDefault)
					return configuration;
			}

			return new StoredLayoutConfiguration();
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
