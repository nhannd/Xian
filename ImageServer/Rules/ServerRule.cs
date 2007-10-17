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

using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Rules
{
    public class ServerRule
    {
        #region Private Members
        private readonly XmlDocument _doc;
        private ISpecification _conditions;
        private IActionSet _actions;
        #endregion

        #region Constructors
        public ServerRule(string script)
        {
            _doc = new XmlDocument();
            _doc.Load(new StringReader(script));
        }

        public ServerRule(XmlDocument doc)
        {
            _doc = doc;
        }

        #endregion

        #region Public Methods

        public void Compile(XmlSpecificationCompiler specCompiler, XmlActionCompiler actionCompiler)
        {
            XmlNode ruleNode =
        CollectionUtils.SelectFirst<XmlNode>(_doc.ChildNodes,
                                             delegate(XmlNode child)
                                             { return child.Name.Equals("rule"); });

            XmlNode conditionNode =
                CollectionUtils.SelectFirst<XmlNode>(ruleNode.ChildNodes,
                                                     delegate(XmlNode child)
                                                     { return child.Name.Equals("condition"); });

            _conditions = specCompiler.Compile(conditionNode as XmlElement);

            XmlNode actionNode =
                CollectionUtils.SelectFirst<XmlNode>(ruleNode.ChildNodes,
                                                     delegate(XmlNode child)
                                                     { return child.Name.Equals("action"); });


            _actions = actionCompiler.Compile(actionNode as XmlElement);
        }

        public void Execute(DicomAttributeCollection collection, object context)
        {
            TestResult result = _conditions.Test(collection);

            if (result.Success)
            {
                TestResult actionResult = _actions.Execute(collection, context);
                if (actionResult.Fail)
                {
                    foreach (TestResultReason reason in actionResult.Reasons)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error performing action: {0}", reason.Message);
                    }
                }
            }
        }

        #endregion
    }
}
