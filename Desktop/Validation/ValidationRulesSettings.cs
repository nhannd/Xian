using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using System.IO;
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Supports management of mapping of validation rules from an embedded xml document to a settings class.
    /// Derive from this class to create a settings class that is populated dynamically with rules
    /// obtained from an xml doc. NOTE: this is just experimental code, not actually in use right now.
    /// </summary>
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    public abstract class ValidationRulesSettings : ApplicationSettingsBase
    {
        class SpecXmlSource : ISpecificationXmlSource
        {
            private ValidationRulesSettings _owner;
            public SpecXmlSource(ValidationRulesSettings owner)
            {
                _owner = owner;
            }

            #region ISpecificationXmlSource Members

            public XmlElement GetSpecificationXml(string id)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml((string)_owner[id]);
                return xmlDoc.DocumentElement;
            }

            public IDictionary<string, XmlElement> GetAllSpecificationsXml()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        private SettingsProvider _provider;

        public ValidationRulesSettings(string resourceName, Assembly asm)
			: this()
        {
            _provider = new ClearCanvas.Common.Configuration.StandardSettingsProvider();
            _provider.Initialize(null, null);
            this.Providers.Add(_provider);
            ProcessXml(resourceName, asm);
        }

		protected ValidationRulesSettings()
		{
			ApplicationSettingsRegister.Instance.RegisterInstance(this);
		}

		~ValidationRulesSettings()
		{
			ApplicationSettingsRegister.Instance.UnregisterInstance(this);
		}

        public void GetRules()
        {
            SpecificationFactory specFactory = new SpecificationFactory(new SpecXmlSource(this));
            foreach (SettingsProperty prop in this.Properties)
            {
                new ValidationRule(prop.Name, specFactory.GetSpecification(prop.Name));
            }
        }

        private void ProcessXml(string resourceName, Assembly asm)
        {
            ResourceResolver rr = new ResourceResolver(asm);
            try
            {
                using (Stream xmlStream = rr.OpenResource(resourceName))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlStream);
                    XmlNodeList specNodes = xmlDoc.GetElementsByTagName("spec");
                    foreach (XmlElement specNode in specNodes)
                    {
                        AddProperty(specNode.GetAttribute("id"), specNode.OuterXml);
                    }
                }
            }
            catch (Exception)
            {
                // no cfg file for this component
            }
        }

        private void AddProperty(string name, string xml)
        {
            this.Properties.Add(
               new SettingsProperty(
                   name,
                   typeof(string),
                   _provider,
                   true,
                   xml,
                   SettingsSerializeAs.String,
                   null,
                   false,
                   false));
        }

    }
}
