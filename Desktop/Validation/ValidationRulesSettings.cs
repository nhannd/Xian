#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
	// TODO (Stewart): Made this internal because as it is stated below, this is experimental code not in use right now.

    /// <summary>
    /// Supports management of mapping of validation rules from an embedded xml document to a settings class.
    /// </summary>
    /// <remarks>
	/// Derive from this class to create a settings class that is populated dynamically with rules
	/// obtained from an xml document. NOTE: this is just experimental code, not actually in use right now.
    /// </remarks>
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal abstract class ValidationRulesSettings : ApplicationSettingsBase
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

            #region ISpecificationXmlSource Members

            public string DefaultExpressionLanguage
            {
                get { throw new Exception("The method or operation is not implemented."); }
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
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
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
