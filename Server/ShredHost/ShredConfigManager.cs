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
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
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
