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
using System.Collections.Generic;
using System.Xml;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[SettingsGroupDescription("Stores user-defined LUT (e.g. window/level) presets.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class PresetVoiLutSettings
	{
		private readonly TypeConverter _xkeysConverter;
		private PresetVoiLutGroupCollection _presetGroups;

		private PresetVoiLutSettings()
		{
			_xkeysConverter = TypeDescriptor.GetConverter(typeof(XKeys));
            // TODO (Phoenix5): Hack for RSNA that must be removed.
            //ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }

		public PresetVoiLutGroupCollection GetPresetGroups()
		{
			if (_presetGroups != null)
				return _presetGroups;

			_presetGroups = new PresetVoiLutGroupCollection();

			XmlDocument document = new XmlDocument();
			document.LoadXml(this.SettingsXml);

			XmlNodeList groupNodes = document.SelectNodes("//group");
			foreach (XmlElement groupNode in groupNodes)
			{
				PresetVoiLutGroup group = DeserializeGroup(groupNode);
				if (group != null && !_presetGroups.Contains(group))
					_presetGroups.Add(group);
			}

			return _presetGroups;
		}

		private PresetVoiLutGroup DeserializeGroup(XmlElement groupNode)
		{
			PresetVoiLutGroup group = new PresetVoiLutGroup(groupNode.GetAttribute("modality"));
			
			XmlNodeList presetNodes = groupNode.SelectNodes("presets/preset");

			DeserializeGroupPresets(group.Presets, presetNodes);

			if (group.Presets.Count == 0)
				group = null;

			return group;
		}

		private void DeserializeGroupPresets(PresetVoiLutCollection presets, XmlNodeList presetNodes)
		{
			foreach (XmlElement presetNode in presetNodes)
			{
				string keyStrokeAttribute = presetNode.GetAttribute("keystroke");
				XKeys keyStroke = XKeys.None;
				if (!String.IsNullOrEmpty(keyStrokeAttribute))
					keyStroke = (XKeys)_xkeysConverter.ConvertFromInvariantString(keyStrokeAttribute);

				string factoryName = presetNode.GetAttribute("factory");

				IPresetVoiLutOperationFactory factory = PresetVoiLutOperationFactories.GetFactory(factoryName);
				if (factory == null)
					continue;

				PresetVoiLutConfiguration configuration = PresetVoiLutConfiguration.FromFactory(factory);

				XmlNodeList configurationItems = presetNode.SelectNodes("configuration/item");
				foreach (XmlElement configurationItem in configurationItems)
					configuration[configurationItem.GetAttribute("key")] = configurationItem.GetAttribute("value");

				try 
				{
					IPresetVoiLutOperation operation = factory.Create(configuration);
					PresetVoiLut preset = new PresetVoiLut(operation);
					preset.KeyStroke = keyStroke;
					presets.Add(preset);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					continue;
				}
			}
		}

		public void SetPresetGroups(PresetVoiLutGroupCollection groups)
		{
			try
			{
				XmlDocument document = new XmlDocument();
				XmlElement rootElement = document.CreateElement("preset-voi-luts");
				document.AppendChild(rootElement);

				foreach (PresetVoiLutGroup group in groups)
				{
					if (group.Presets.Count == 0)
						continue;

					XmlElement groupNode = document.CreateElement("group");
					if (!String.IsNullOrEmpty(group.Modality))
						groupNode.SetAttribute("modality", group.Modality);

					rootElement.AppendChild(groupNode);

					XmlElement presetsElement = document.CreateElement("presets");
					groupNode.AppendChild(presetsElement);

					foreach (PresetVoiLut preset in group.Presets)
					{
						XmlElement presetElement = document.CreateElement("preset");
						presetsElement.AppendChild(presetElement);

						if (preset.KeyStroke != XKeys.None)
							presetElement.SetAttribute("keystroke", preset.KeyStroke.ToString());

						PresetVoiLutConfiguration configuration = preset.Operation.GetConfiguration();

						presetElement.SetAttribute("factory", configuration.FactoryName);

						XmlElement configurationElement = document.CreateElement("configuration");
						presetElement.AppendChild(configurationElement);
						
						foreach (KeyValuePair<string, string> configurationItem in configuration)
						{
							if (String.IsNullOrEmpty(configurationItem.Key) || String.IsNullOrEmpty(configurationItem.Value))
								continue;

							XmlElement configurationItemElement = document.CreateElement("item");
							configurationItemElement.SetAttribute("key", configurationItem.Key);
							configurationItemElement.SetAttribute("value", configurationItem.Value);

							configurationElement.AppendChild(configurationItemElement);
						}
					}	
				}

				_presetGroups = null;
				string currentSettings = this.SettingsXml;

				this.SettingsXml = document.OuterXml;
				this.Save();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}
	}
}
