using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Common.Specifications
{
    public interface ISpecificationXmlSource
    {
        string DefaultExpressionLanguage { get; }
        XmlElement GetSpecificationXml(string id);
        IDictionary<string, XmlElement> GetAllSpecificationsXml();
    }
}
