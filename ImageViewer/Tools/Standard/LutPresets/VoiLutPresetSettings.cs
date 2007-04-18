using System;
using System.Configuration;
using System.Collections.Generic;
using System.Xml;
using System.ComponentModel;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Configuration;
using System.IO;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[SettingsGroupDescription("Stores Lut Preset settings for each user")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class VoiLutPresetSettings
	{
		private VoiLutPresetSettings()
		{
			ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }

		~VoiLutPresetSettings()
		{
			ApplicationSettingsRegister.Instance.UnregisterInstance(this);
		}

		public VoiLutPresetConfigurationCollection GetVoiLutPresetConfigurations()
		{
			VoiLutPresetConfigurationCollection collection = new VoiLutPresetConfigurationCollection();

			XmlDocument document = new XmlDocument();
			document.LoadXml(this.SettingsXml);

			XmlNodeList lutPresetGroupNodes = document.SelectNodes("//voilut-preset-group");
			foreach (XmlElement groupNode in lutPresetGroupNodes)
			{
				string lutPresetModalityFilter = groupNode.GetAttribute("modality");

				XmlNodeList voiLutPresetNodes = groupNode.SelectNodes("voilut-preset");
				foreach (XmlElement voiLutPresetNode in voiLutPresetNodes)
				{
					string lutPresetName = voiLutPresetNode.GetAttribute("name");
					string keyStroke = voiLutPresetNode.GetAttribute("keystroke");
					XmlElement voiLutApplicatorConfigurationNode = (XmlElement)voiLutPresetNode.SelectSingleNode("voilut-applicator-configuration");

					Dictionary<string, string> voiLutApplicatorConfigurationValues = new Dictionary<string, string>();
					string factoryKey = voiLutApplicatorConfigurationNode.GetAttribute("factory-key");

					XmlNodeList voiLutApplicatorConfigurationValueNodes = voiLutApplicatorConfigurationNode.GetElementsByTagName("configuration-value");
					foreach (XmlElement voiLutApplicatorConfigurationValueNode in voiLutApplicatorConfigurationValueNodes)
						voiLutApplicatorConfigurationValues.Add(voiLutApplicatorConfigurationValueNode.GetAttribute("key"), voiLutApplicatorConfigurationValueNode.GetAttribute("value"));

					XKeys keyStrokeValue = XKeys.None;
					if (!String.IsNullOrEmpty(keyStroke))
					{
						EnumConverter converter = new EnumConverter(typeof(XKeys));
						keyStrokeValue = (XKeys)converter.ConvertFromInvariantString(keyStroke);
					}

					VoiLutPresetConfiguration configuration = new VoiLutPresetConfiguration(lutPresetModalityFilter, keyStrokeValue, lutPresetName, 
						new VoiLutPresetApplicatorConfiguration(factoryKey, voiLutApplicatorConfigurationValues));

					collection.UnsafeUpdate(configuration);
				}
			}

			return collection;
		}

		public void SetVoiLutPresetGroupConfigurations(VoiLutPresetConfigurationCollection presetConfigurations)
		{
			XmlDocument document = new XmlDocument();
			XmlElement rootElement = document.CreateElement("voilut-presets");
			document.AppendChild(rootElement);

			IDictionary<string, IEnumerable<VoiLutPresetConfiguration>> presetConfigurationsByModality =
				VoiLutPresetConfigurationsHelper.GetConfigurationsByModality(presetConfigurations);

			foreach (KeyValuePair<string, IEnumerable<VoiLutPresetConfiguration>> modalityConfigurationGroup in presetConfigurationsByModality)
			{
				XmlElement voiLutPresetGroupNode = (XmlElement)document.CreateElement("voilut-preset-group");
				voiLutPresetGroupNode.SetAttribute("modality", modalityConfigurationGroup.Key);

				foreach (VoiLutPresetConfiguration configuration in modalityConfigurationGroup.Value)
				{ 
					XmlElement voiLutPresetNode = (XmlElement)document.CreateElement("voilut-preset");
					voiLutPresetGroupNode.AppendChild(voiLutPresetNode);
					
					voiLutPresetNode.SetAttribute("name", configuration.Name);
					if (configuration.KeyStroke != XKeys.None)
						voiLutPresetNode.SetAttribute("keystroke", configuration.KeyStroke.ToString());

					XmlElement voiLutApplicatorNode = (XmlElement)document.CreateElement("voilut-applicator-configuration");
					voiLutPresetNode.AppendChild(voiLutApplicatorNode);

					voiLutApplicatorNode.SetAttribute("factory-key", configuration.VoiLutPresetApplicatorConfiguration.FactoryKey);
					foreach (KeyValuePair<string, string> configurationValues in configuration.VoiLutPresetApplicatorConfiguration.ConfigurationValues)
					{
						XmlElement configurationElementNode = document.CreateElement("configuration-value");
						voiLutApplicatorNode.AppendChild(configurationElementNode);

						configurationElementNode.SetAttribute("key", configurationValues.Key);
						configurationElementNode.SetAttribute("value", configurationValues.Value);
					}
				}

				rootElement.AppendChild(voiLutPresetGroupNode);
			}

			this.SettingsXml = document.OuterXml;
			this.Save();
		}
	}
}
