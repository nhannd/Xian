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
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
	//TODO (CR Sept 2010): Turn these into ApplicationSettingsBase classes and use the ApplicationSettingsExtensions to set the shared property values.
    public abstract class ShredConfigSection : ConfigurationSection, ICloneable
    {
        //[ConfigurationProperty("sampleProperty", DefaultValue="test")]
        //public string SampleProperty
        //{
        //    get { return (string)this["sampleProperty"]; }
        //    set { this["sampleProperty"] = value; }
        //}

        // Need to implement a clone to fix a .Net bug in ConfigurationSectionCollection.Add
        public abstract object Clone();
    }
    
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
