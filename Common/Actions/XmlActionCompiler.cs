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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
    /// Defines an extension point for types of actions that can be parsed by the <see cref="XmlActionCompiler{T}"/>.
    /// </summary>
    /// <seealso cref="IXmlActionCompilerOperator{T}"/>
    [ExtensionPoint]
    public sealed class XmlActionCompilerOperatorExtensionPoint<T> : ExtensionPoint<IXmlActionCompilerOperator<T>>
    {
    }

    /// <summary>
    /// Compiler for compiling a set of actions to execute from an XML file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="XmlActionCompiler{T}"/> can be used to compile a set of actions to perform
    /// from XML.  The <see cref="XmlActionCompiler{T}.Compile"/> method can be called to create the
    /// set of actions to be performed.  These actions can then be executed based on input data.
    /// </para>
    /// <para>
    /// Actions are defined by the <see cref="XmlActionCompilerOperatorExtensionPoint{T}"/> extension
    /// point.  The compiler does not contain any predefined actions.  The compiler makes no assumptions
    /// about the attributes of the <see cref="XmlElement"/> for the action.  Any attributes can be defined
    /// for the action and are interpreted by the operation defined for the action type.
    /// </para>
    /// </remarks>
    public class XmlActionCompiler<T>
    {
        private readonly Dictionary<string, IXmlActionCompilerOperator<T>> _operatorMap = new Dictionary<string, IXmlActionCompilerOperator<T>>();
        private XmlSchema _schema;

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlActionCompiler()
        {
            // add extension operators
            XmlActionCompilerOperatorExtensionPoint<T> xp = new XmlActionCompilerOperatorExtensionPoint<T>();
            foreach (IXmlActionCompilerOperator<T> compilerOperator in xp.CreateExtensions())
            {
                AddOperator(compilerOperator);
            }
            _schema = CreateSchema();
        }

        private XmlSchema CreateSchema()
        {
            XmlSchema baseSchema = new XmlSchema();

            foreach (IXmlActionCompilerOperator<T> op in _operatorMap.Values)
            {
                XmlSchemaElement element = op.GetSchema();
                baseSchema.Items.Add(element);
            }

            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(baseSchema);
            set.Compile();

            XmlSchema compiledSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                compiledSchema = schema;
            }

            //StringWriter sw = new StringWriter();
            //compiledSchema.Write(sw);
            //Platform.Log(LogLevel.Info, sw);

            return compiledSchema;
        }

        /// <summary>
        /// An XML Schema representing the valid XML for the compiler.
        /// </summary>
        public XmlSchema Schema
        {
            get { return _schema; }
        }

        /// <summary>
        /// Compile a set of actions to perform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will parse the child <see cref="XmlElement"/>s of <paramref name="containingNode"/>.
        /// Based on the name of the element, the the compiler will look for an <see cref="XmlActionCompilerOperatorExtensionPoint{T}"/>
        /// extension that handles the element type.  A list is constructed of all actions to perform, and a class implementing the 
        /// <see cref="IActionSet{T}"/> interface is returned which can be called to exectute the actions based on input data.
        /// </para>
        /// </remarks>
        /// <param name="containingNode">The input XML containg actions to perform.</param>
        /// <param name="checkSchema">Check the schema when compiling.</param>
        /// <returns>A class instance that implements the <see cref="IActionSet{T}"/> interface.</returns>
        public IActionSet<T> Compile(XmlElement containingNode, bool checkSchema)
        {
            // Note, recursive calls are made to this method to compile.  The schema is not
            // checked on recursive calls, but should be checked once on an initial compile.
            if (checkSchema)
            {
                // We must parse the XML to get the schema validation to work.  So, we write
                // the xml out to a string, and read it back in with Schema Validation enabled
                StringWriter sw = new StringWriter();

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = Encoding.UTF8;
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
                xmlWriterSettings.Indent = false;
                xmlWriterSettings.NewLineOnAttributes = false;
                xmlWriterSettings.IndentChars = "";

                XmlWriter xmlWriter = XmlWriter.Create(sw, xmlWriterSettings);
                foreach (XmlNode node in containingNode.ChildNodes)
                    node.WriteTo(xmlWriter);
                xmlWriter.Close();

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.Schemas = new XmlSchemaSet();
                xmlReaderSettings.Schemas.Add(Schema);
                xmlReaderSettings.ValidationType = ValidationType.Schema;
                xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                XmlReader xmlReader = XmlTextReader.Create(new StringReader(sw.ToString()), xmlReaderSettings);
                while (xmlReader.Read()) ;
                xmlReader.Close();
            }

            List<IActionItem<T>> actions = new List<IActionItem<T>>();
            ICollection<XmlNode> nodes = GetChildElements(containingNode);
            
			foreach(XmlNode node in nodes)
            {
                if (_operatorMap.ContainsKey(node.Name))
                {
                    IXmlActionCompilerOperator<T> op = _operatorMap[node.Name];
                    actions.Add(op.Compile(node as XmlElement));
                }
                else
                {
					throw new XmlActionCompilerException(string.Format(SR.FormatUnableToFindMatchingAction, node.Name));
                }
            }

			return new ActionSet<T>(actions);
        }

        private void AddOperator(IXmlActionCompilerOperator<T> op)
        {
            _operatorMap.Add(op.OperatorTag, op);
        }

        private static ICollection<XmlNode> GetChildElements(XmlElement node)
        {
            return CollectionUtils.Select<XmlNode>(node.ChildNodes, delegate(XmlNode child) { return child is XmlElement; });
        }
    }
}