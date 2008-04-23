using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy.ActionPlugins
{
    [ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<EditStudyContext>))]
    public class SetValueOperator : IXmlActionCompilerOperator<EditStudyContext>
    {
        public string OperatorTag
        {
            get { return "set"; }
        }

        public IActionItem<EditStudyContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["tag"] == null || xmlNode.Attributes["tag"].Value == "")
                throw new XmlActionCompilerException("Unexpected missing tag attribute or its value for settag action");

            if (xmlNode.Attributes["value"] == null || xmlNode.Attributes["value"].Value == "")
                throw new XmlActionCompilerException("Unexpected missing value attribute or its value for settag action");

            string[] tagValues = xmlNode.Attributes["tag"].Value.Split('\\');
            uint[] tags = new uint[tagValues.Length];
            int i = 0;
            foreach (string t in tagValues)
            {
                tags[i++] = uint.Parse(t, NumberStyles.HexNumber, null);
            }

            string value = xmlNode.Attributes["value"].Value;

            try
            {
                return new SetValueActionItem(tags, value);
            }
            catch(Exception e)
            {
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                xmlNode.WriteTo(xw);
                
                Platform.Log(LogLevel.Error, "Error occured when compiling the action specification {0}", sw.ToString());
                throw;
            }
        }


        #region IXmlActionCompilerOperator<EditStudyContext> Members


        public System.Xml.Schema.XmlSchemaElement GetSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "tag";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "value";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);


            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "set";
            element.SchemaType = type;

            return element;
        }

        #endregion
    }


}
