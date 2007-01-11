using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Enterprise.Configuration
{
    public class ConfigSetting
    {
        private string _name;
        private string _description;
        private ConfigSettingScope _scope;
        private string _defaultValue;

        public ConfigSetting()
        {

        }

        public ConfigSetting(string name, string description, ConfigSettingScope scope, string defaultValue)
        {
            _name = name;
            _description = description;
            _defaultValue = defaultValue;
            _scope = scope;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public ConfigSettingScope Scope
        {
            get { return _scope; }
        }

        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("setting");
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("scope", _scope.ToString());
            writer.WriteElementString("description", _description);
            writer.WriteElementString("value", _defaultValue);
            writer.WriteEndElement();
        }

        public void ReadXml(XmlReader reader)
        {
            _name = reader.GetAttribute("name");
            _scope = (ConfigSettingScope)Enum.Parse(typeof(ConfigSettingScope), reader.GetAttribute("scope"));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch(reader.LocalName)
                    {
                        case "description":
                            _description = reader.ReadElementContentAsString();
                            break;
                        case "value":
                            _defaultValue = reader.ReadElementContentAsString();
                            break;
                    }
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;
            }
        }
	
    }
}
