using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using System.Xml;
using System.IO;


namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// Stores a set of settings keys and values for a given settings group.  Used internally by the framework.
    /// </summary>
	public partial class ConfigSettingsInstance : Entity
	{
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        /// <summary>
        /// Adds the set of values stored in this <see cref="ConfigSettingsInstance"/> to the specified dictionary.
        /// </summary>
        /// <param name="values"></param>
        public void GetValues(IDictionary<string, string> values)
        {
            ReadXml(values);
        }

        /// <summary>
        /// Assigns the specified dictionary of settigns values to this <see cref="ConfigSettingsInstance"/>,
        /// removing any existing settings.
        /// </summary>
        /// <param name="values"></param>
        public void SetValues(IDictionary<string, string> values)
        {
            WriteXml(values);
        }

        /// <summary>
        /// Overwrites any values stored in this instance with values having the same key
        /// from a previous version
        /// </summary>
        /// <param name="previousVersion">The previous version from which to copy settings values</param>
        public void UpgradeFrom(ConfigSettingsInstance previousVersion)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            this.GetValues(values);

            // overwrite any values with those from the previous version
            previousVersion.GetValues(values);

            // re-write the values into this object
            this.SetValues(values);
        }

        /// <summary>
        /// Clears all stored values
        /// </summary>
        public void Clear()
        {
            _settingsValuesXml = null;
        }

        #region XML de/serialization

        private void ReadXml(IDictionary<string, string> values)
        {
            if (_settingsValuesXml != null)
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(_settingsValuesXml)))
                {
                    ReadXml(reader, values);
                }
            }
        }

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

        private void WriteXml(IDictionary<string, string> values)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteXml(writer, values);
                _settingsValuesXml = sw.ToString();
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
		
		#region Object overrides
		
		public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion
    }
}