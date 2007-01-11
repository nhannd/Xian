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
    /// ConfigSettingsGroup entity
    /// </summary>
	public partial class ConfigSettingsGroup : Entity
	{
        private Dictionary<string, ConfigSetting> _settings;
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public IDictionary<string, ConfigSetting> Settings
        {
            get
            {
                if (_settings == null)
                {
                    ReadXml();
                }
                return _settings;
            }
        }

        public void PrepareSave()
        {
            if (_settings != null)
            {
                WriteXml();
            }
        }

        #region XML de/serialization

        private void ReadXml()
        {
            _settings = new Dictionary<string, ConfigSetting>();
            if (_settingsXml != null)
            {
                using (XmlTextReader reader = new XmlTextReader(new StringReader(_settingsXml)))
                {
                    ReadXml(reader);
                }
            }
        }

        private void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "setting")
                {
                    ConfigSetting setting = new ConfigSetting();
                    setting.ReadXml(reader);
                    _settings[setting.Name] = setting;
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
                _settingsXml = sw.ToString();
            }
        }

        private void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("settings");
            foreach (ConfigSetting setting in _settings.Values)
            {
                setting.WriteXml(writer);
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