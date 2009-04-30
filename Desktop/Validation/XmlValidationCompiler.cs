#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Compiles validation rules that are encoded as XML specifications.
    /// </summary>
    public class XmlValidationCompiler
    {
        private readonly XmlSpecificationCompiler _specCompiler;

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlValidationCompiler()
            :this("jscript")
        {
        }

        /// <summary>
        /// Constructor that allows specifying the expression language to use when evaluating the XML specification.
        /// </summary>
        /// <param name="defaultExpressionLanguage"></param>
        public XmlValidationCompiler(string defaultExpressionLanguage)
        {
            _specCompiler = new XmlSpecificationCompiler(defaultExpressionLanguage);
        }

        /// <summary>
        /// Compiles all rules contained in the specified XML rules document.
        /// </summary>
        /// <param name="rulesDocument"></param>
        /// <returns></returns>
        public List<IValidationRule> CompileRules(TextReader rulesDocument)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(rulesDocument);

            return CompileRules(xmlDocument);
        }

        /// <summary>
        /// Compiles all rules contained in the specified XML rules document.
        /// </summary>
        /// <param name="rulesDocument"></param>
        /// <returns></returns>
        public List<IValidationRule> CompileRules(XmlDocument rulesDocument)
        {
            XmlNodeList ruleNodes = rulesDocument.SelectNodes("validation-rules/validation-rule");
            return CollectionUtils.Map<XmlNode, IValidationRule>(ruleNodes,
                delegate(XmlNode ruleNode) { return CompileRule((XmlElement)ruleNode); });
        }

        /// <summary>
        /// Compiles the rule described by the specified XML element, which must be a "validation-rule" element.
        /// </summary>
        /// <param name="ruleNode"></param>
        /// <returns></returns>
        public IValidationRule CompileRule(XmlElement ruleNode)
        {
            string property = ruleNode.GetAttribute("boundProperty");

            ISpecification spec = _specCompiler.Compile(ruleNode);
            return new ValidationRule(property, spec);
        }
    }
}
