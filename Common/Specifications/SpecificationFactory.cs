using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class SpecificationFactory
    {
        private Builder _builder;
        private XmlDocument _xmlDoc;

        private Dictionary<string, ISpecification> _cache;

        public SpecificationFactory(Stream xml)
        {
            _builder = new Builder(this);
            _cache = new Dictionary<string, ISpecification>();

            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(xml);
        }

        public ISpecification GetSpecification(string id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            else
            {
                XmlElement specNode = (XmlElement)CollectionUtils.SelectFirst(_xmlDoc.GetElementsByTagName("spec"),
                    delegate(object node) { return ((XmlElement)node).GetAttribute("id") == id; });

                if (specNode == null)
                    throw new UndefinedSpecificationException(id);

                return _cache[id] = _builder.BuildSpecification(specNode);
            }
        }
    }
}
