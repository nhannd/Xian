using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Common.Specifications
{
    public interface IXmlSpecificationCompilerOperator
    {
        string OperatorTag { get; }
        Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context);
    }
}
