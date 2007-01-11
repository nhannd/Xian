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
    /// ConfigSettingsInstance entity
    /// </summary>
	public partial class ConfigSettingsInstance : Entity
	{
        private Dictionary<string, string> _values;
        private bool _unsaved;

        /// <summary>
        /// Constructor creates a new settings instance for the specified group
        /// </summary>
        /// <param name="group"></param>
        public ConfigSettingsInstance(ConfigSettingsGroup group, string user, string instanceKey)
        {
            _user = user;
            _instanceKey = instanceKey;
            _group = group;
            _unsaved = true;
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public bool Unsaved
        {
            get { return _unsaved; }
        }

        public void PrepareSave()
        {
            if (_values != null)
            {
                WriteXml();
            }
        }

        public string this[string name]
        {
            get
            {
                if (_values == null)
                {
                    ReadXml();
                }

                return _values.ContainsKey(name) ? _values[name] : this.Group.Settings[name].DefaultValue;
            }
            set
            {
                if (_values == null)
                {
                    ReadXml();
                }

                _values[name] = value;
            }
        }


        #region XML de/serialization

        private void ReadXml()
        {
            _values = new Dictionary<string, string>();
            if (_settingsValuesXml != null)
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(_settingsValuesXml)))
                {
                    ReadXml(reader);
                }
            }
        }

        private void ReadXml(XmlReader reader)
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
                            _values[settingName] = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
        }

        private void WriteXml()
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteXml(writer);
                _settingsValuesXml = sw.ToString();
            }
        }

        private void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("settings");
            foreach (KeyValuePair<string, string> kvp in _values)
            {
                writer.WriteStartElement("setting");
                writer.WriteAttributeString("name", kvp.Key);
                writer.WriteElementString("value", kvp.Value);
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