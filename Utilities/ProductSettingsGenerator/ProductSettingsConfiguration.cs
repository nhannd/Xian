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
using System.Xml;

namespace ClearCanvas.Utilities.ProductSettingsGenerator
{
	public class ProductSettingsConfiguration
	{
		private readonly EncryptedProductSettings _productSettings;

		public ProductSettingsConfiguration(EncryptedProductSettings productSettings)
		{
			if (productSettings == null)
				throw new ArgumentNullException("productSettings");
			_productSettings = productSettings;
		}

		public void Save(string filename)
		{
			// create the settings section
			var section = new ClientSettingsSection();
			section.Settings.Add(new SettingElement("Component", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.Component)});
			section.Settings.Add(new SettingElement("Product", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.Product)});
			section.Settings.Add(new SettingElement("Edition", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.Edition)});
			section.Settings.Add(new SettingElement("Release", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.Release)});
			section.Settings.Add(new SettingElement("Version", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.Version)});
			section.Settings.Add(new SettingElement("VersionSuffix", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.VersionSuffix)});
			section.Settings.Add(new SettingElement("Copyright", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.Copyright)});
			section.Settings.Add(new SettingElement("License", SettingsSerializeAs.String) {Value = CreateValueElement(_productSettings.License)});

			// open the configuration file
			var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap {ExeConfigFilename = filename}, ConfigurationUserLevel.None);

			// get the application settings section group
			var sectionGroup = configuration.GetSectionGroup("applicationSettings");
			if (sectionGroup == null)
				configuration.SectionGroups.Add("applicationSettings", sectionGroup = new ApplicationSettingsGroup());

			// remove a pre-existing settings section if necessary
			sectionGroup.Sections.Remove("ClearCanvas.Common.ProductSettings");

			// add the updated setting ssection
			sectionGroup.Sections.Add("ClearCanvas.Common.ProductSettings", section);

			// save it
			configuration.Save(ConfigurationSaveMode.Full);
		}

		private static SettingValueElement CreateValueElement(byte[] encryptedBytes)
		{
			var xmlDoc = new XmlDocument();
			var valueNode = xmlDoc.CreateElement("value");
			if (encryptedBytes != null)
				valueNode.InnerText = Convert.ToBase64String(encryptedBytes);
			else
				valueNode.IsEmpty = true;
			return new SettingValueElement {ValueXml = valueNode};
		}
	}
}