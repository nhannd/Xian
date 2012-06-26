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
using System.Configuration;
using System.IO;
using ClearCanvas.Common;
using System.Xml;

namespace ClearCanvas.Server.ShredHost
{
	[Obsolete("Use standard ApplicationSettingsBase-derived classes with ApplicationSettingsExtensions to set shared settings if so desired. Use LegacyShredConfigSectionAttribute and IMigrateLegacyShredConfigSection to maintain shred settings migration compatibility.")]
    public abstract class ShredConfigSection : ConfigurationSection, ICloneable
    {
    	private Dictionary<string, string> _removedProperties;

		//[ConfigurationProperty("sampleProperty", DefaultValue="test")]
        //public string SampleProperty
        //{
        //    get { return (string)this["sampleProperty"]; }
        //    set { this["sampleProperty"] = value; }
        //}

        // Need to implement a clone to fix a .Net bug in ConfigurationSectionCollection.Add
        public abstract object Clone();

		public string GetRemovedPropertyValue(string name)
		{
			if (_removedProperties == null)
				return null;

			string value;
			return _removedProperties.TryGetValue(name, out value) ? value : null;
		}

		private void AddRemovedProperty(string name, string value)
		{
			if (_removedProperties == null)
				_removedProperties = new Dictionary<string, string>();

			_removedProperties[name] = value;
		}

    	protected sealed override bool OnDeserializeUnrecognizedAttribute(string name, string value)
    	{
			if (!ShredSettingsMigrator.IsMigrating)
				return false;

    		AddRemovedProperty(name, value);
			return true;
    	}

    	protected sealed override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (!ShredSettingsMigrator.IsMigrating)
				return false;

			if (!reader.IsEmptyElement)
			{
				var subTreeReader = reader.ReadSubtree();
				subTreeReader.MoveToContent();
				AddRemovedProperty(elementName, subTreeReader.ReadOuterXml());
				subTreeReader.Close();
			}

			return true;
		}

		internal void LoadXml(string xml)
		{
			using (var stringReader = new StringReader(xml))
			using (var xmlReader = XmlReader.Create(stringReader))
			{
				base.DeserializeSection(xmlReader);
			}
		}
	}

	[Obsolete("Use standard ApplicationSettingsBase-derived classes with ApplicationSettingsExtensions to set shared settings if so desired. Use LegacyShredConfigSectionAttribute and IMigrateLegacyShredConfigSection to maintain shred settings migration compatibility.")]
    public static class ShredConfigManager
    {
        public static ConfigurationSection GetConfigSection(string sectionName)
        {
            System.Configuration.Configuration config =
                    ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);

            return (config == null ? null : config.Sections[sectionName]);
        }

        public static bool UpdateConfigSection(string sectionName, ShredConfigSection section)
        {
            try
            {
                // Get the current configuration file.
                System.Configuration.Configuration config =
                        ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);

                if (config.Sections[sectionName] == null)
                {
                    section.SectionInformation.ForceSave = true;
                    config.Sections.Add(sectionName, section);
                }
                else
                {
                    config.Sections.Remove(sectionName);
                    config.Sections.Add(sectionName, section.Clone() as ConfigurationSection);
                }

                config.Save(ConfigurationSaveMode.Full);
            }
            catch (ConfigurationErrorsException err)
            {
                Platform.Log(LogLevel.Info, err);
                return false;
            }

            return true;
        }        
    }
}
