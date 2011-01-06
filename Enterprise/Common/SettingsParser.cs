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
