#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy.ActionPlugins
{
    /// <summary>
    /// Plugin to support "set" action in an <see cref="EditStudyCommand"/> operation
    /// </summary>
    [ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<EditStudyContext>))]
    public class SetValueOperator : IXmlActionCompilerOperator<EditStudyContext>
    {
        private const char TAG_SEPARATOR = '\\';

        public string OperatorTag
        {
            get { return "set"; }
        }

        public IActionItem<EditStudyContext> Compile(XmlElement xmlNode)
        {
            // Make sure the xml is valid
            if (xmlNode.Attributes["tag"] == null || String.IsNullOrEmpty(xmlNode.Attributes["tag"].Value))
                throw new XmlActionCompilerException("Unexpected missing tag attribute or its value for settag action");

            if (xmlNode.Attributes["value"] == null)
                throw new XmlActionCompilerException("Unexpected missing value attribute or its value for settag action");

            string[] tagValues = xmlNode.Attributes["tag"].Value.Split(TAG_SEPARATOR);
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
            catch(Exception)
            {
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                xmlNode.WriteTo(xw);
                
                Platform.Log(LogLevel.Error, "Error occured when compiling the action specification {0}", sw.ToString());
                throw;
            }
        }


        #region IXmlActionCompilerOperator<EditStudyContext> Members


        public XmlSchemaElement GetSchema()
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
