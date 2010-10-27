#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class SpecificationFactory : ISpecificationProvider
    {
        class SingleDocumentSource : ISpecificationXmlSource
        {
            private XmlDocument _xmlDoc;

            public SingleDocumentSource(Stream xml)
            {
                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(xml);
            }

            public SingleDocumentSource(TextReader xml)
            {
                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(xml);
            }



            #region ISpecificationXmlSource Members

            public string DefaultExpressionLanguage
            {
                get
                {
                    string exprLang = _xmlDoc.DocumentElement.GetAttribute("expressionLanguage");

                    // if not specified, assume jscript
                    return string.IsNullOrEmpty(exprLang) ? "jscript" : exprLang;
                }
            }

            public XmlElement GetSpecificationXml(string id)
            {
                XmlElement specNode = (XmlElement)CollectionUtils.SelectFirst(_xmlDoc.GetElementsByTagName("spec"),
                    delegate(object node) { return ((XmlElement)node).GetAttribute("id") == id; });

                if (specNode == null)
                    throw new UndefinedSpecificationException(id);

                return specNode;
            }

            public IDictionary<string, XmlElement> GetAllSpecificationsXml()
            {
                Dictionary<string, XmlElement> specs = new Dictionary<string, XmlElement>();
                foreach (XmlElement specNode in _xmlDoc.GetElementsByTagName("spec"))
                {
                    specs.Add(specNode.GetAttribute("id"), specNode);
                }
                return specs;
            }

            #endregion
        }



        private XmlSpecificationCompiler _builder;
        private ISpecificationXmlSource _xmlSource;

        private Dictionary<string, ISpecification> _cache;

        public SpecificationFactory(Stream xml)
            :this(new SingleDocumentSource(xml))
        {
        }

        public SpecificationFactory(TextReader xml)
            : this(new SingleDocumentSource(xml))
        {
        }


        public SpecificationFactory(ISpecificationXmlSource xmlSource)
        {
            _builder = new XmlSpecificationCompiler(this, xmlSource.DefaultExpressionLanguage);
            _cache = new Dictionary<string, ISpecification>();
            _xmlSource = xmlSource;
        }

        public ISpecification GetSpecification(string id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            else
            {
                XmlElement specNode = _xmlSource.GetSpecificationXml(id);
                return _cache[id] = _builder.Compile(specNode, false);
            }
        }

        public IDictionary<string, ISpecification> GetAllSpecifications()
        {
            Dictionary<string, ISpecification> specs = new Dictionary<string, ISpecification>();
            foreach (KeyValuePair<string, XmlElement> kvp in _xmlSource.GetAllSpecificationsXml())
            {
                specs.Add(kvp.Key, _builder.Compile(kvp.Value, false));
            }
            return specs;
        }
    }
}
