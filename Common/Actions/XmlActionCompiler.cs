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

using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
    /// Defines an extension point for types of actions that can be parsed by the 
    /// <see cref="XmlActionCompiler{T}"/>.
    /// </summary>
    /// <seealso cref="IXmlActionCompilerOperator{T}"/>
    [ExtensionPoint]
    public class XmlActionCompilerOperatorExtensionPoint<T> : ExtensionPoint<IXmlActionCompilerOperator<T>>
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
    /// for the action are are interpreted by the operation defined for the action type.
    /// </para>
    /// </remarks>
    public class XmlActionCompiler<T>
    {
        private readonly Dictionary<string, IXmlActionCompilerOperator<T>> _operatorMap = new Dictionary<string, IXmlActionCompilerOperator<T>>();

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
        /// <param name="containingNode">The input XML to perform</param>
        /// <returns>A class instance that implements the <see cref="IActionSet{T}"/> interface.</returns>
        public IActionSet<T> Compile(XmlElement containingNode)
        {
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
                    throw new XmlActionCompilerException(string.Format("Unable to find matching action for {0} node in script.  Unable to perform action.", node.Name));
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