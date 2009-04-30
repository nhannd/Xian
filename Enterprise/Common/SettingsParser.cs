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
using System.Xml;
using System.IO;

namespace ClearCanvas.Enterprise.Common
{
    class SettingsParser
    {
        internal void FromXml(string xml, IDictionary<string, string> values)
        {
            if (xml != null)
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
                {
                    ReadXml(reader, values);
                }
            }
        }

        internal string ToXml(IDictionary<string, string> values)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteXml(writer, values);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Overwrites any values stored in this instance with values having the same key
        /// from a previous version
        /// </summary>
        /// <param name="previousVersion">The previous version from which to copy settings values</param>
        internal string UpgradeFromPrevious(string currentVersionXml, string previousVersionXml)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            FromXml(currentVersionXml, values);

            // overwrite any values with those from the previous version
            FromXml(previousVersionXml, values);

            // return the document that represents the upgrade
            return ToXml(values);
        }
        
        #region XML de/serialization

        private void ReadXml(XmlReader reader, IDictionary<string, string> values)
        {
            string settingName = "";

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "setting":
                            settingName = reader.GetAttribute("name");
                            break;
                        case "value":
                            values[settingName] = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
        }

        private void WriteXml(XmlWriter writer, IDictionary<string, string> values)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("settings");
            foreach (KeyValuePair<string, string> kvp in values)
            {
                writer.WriteStartElement("setting");
                writer.WriteAttributeString("name", kvp.Key);

                writer.WriteStartElement("value");
                writer.WriteCData(kvp.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        #endregion
    }
}
