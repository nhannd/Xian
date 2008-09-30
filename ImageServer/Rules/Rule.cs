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
using System.Xml;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    public class Rule
    {
        private IActionSet<ServerActionContext> _actions;
        private ISpecification _conditions;
        private string _description;
        private bool _isDefault;
        private bool _isExempt;
        private string _name;

        #region Constructors

        #endregion

        #region Public Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public bool IsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }

        public bool IsExempt
        {
            get { return _isExempt; }
            set { _isExempt = value; }
        }

        #endregion

        #region Public Methods

        public void Compile(XmlNode ruleNode, XmlSpecificationCompiler specCompiler,
                            XmlActionCompiler<ServerActionContext> actionCompiler)
        {
            XmlNode conditionNode =
                CollectionUtils.SelectFirst<XmlNode>(ruleNode.ChildNodes,
                                                     delegate(XmlNode child) { return child.Name.Equals("condition"); });

			if (conditionNode != null)
				_conditions = specCompiler.Compile(conditionNode as XmlElement, true);
			else if (!IsDefault)
				throw new ApplicationException("No condition element defined for the rule.");
			else
				_conditions = new AndSpecification();

            XmlNode actionNode =
                CollectionUtils.SelectFirst<XmlNode>(ruleNode.ChildNodes,
                                                     delegate(XmlNode child) { return child.Name.Equals("action"); });

			if (actionNode != null)
				_actions = actionCompiler.Compile(actionNode as XmlElement, true);
			else if (!IsExempt)
				throw new ApplicationException("No action element defined for the rule.");
			else
				_actions = new ActionSet<ServerActionContext>(new List<IActionItem<ServerActionContext>>());
        }

        /// <summary>
        /// Returns true if the rule condition is passed for the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TestResult Test(ServerActionContext context)
        {
            if (IsDefault)
                return new TestResult(true);

            return _conditions.Test(context.Message);
        }


        public void Execute(ServerActionContext context, bool defaultRule, out bool ruleApplied, out bool ruleSuccess)
        {
            ruleApplied = false;
            ruleSuccess = true;

            TestResult result = Test(context);

            if (result.Success)
            {
                ruleApplied = true;
                Platform.Log(LogLevel.Debug, "Applying rule {0}", Name);
                TestResult actionResult = _actions.Execute(context);
                if (actionResult.Fail)
                {
                    foreach (TestResultReason reason in actionResult.Reasons)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error performing action {0}: {1}", Name, reason.Message);
                    }
                    ruleSuccess = false;
                }
            }
        }

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Method for validating proper format of a ServerRule.
        /// </summary>
        /// <param name="rule">The rule to validate</param>
        /// <param name="errorDescription">A failure description on error.</param>
        /// <returns>true on successful validation, otherwise false.</returns>
        public static bool ValidateRule(XmlDocument rule, out string errorDescription)
        {
            XmlSpecificationCompiler specCompiler = new XmlSpecificationCompiler("dicom");
            XmlActionCompiler<ServerActionContext> actionCompiler = new XmlActionCompiler<ServerActionContext>();

            ServerRule theServerRule = new ServerRule();
            theServerRule.RuleXml = rule;

            Rule theRule = new Rule();
            theRule.Name = theServerRule.RuleName;

            XmlNode ruleNode =
                CollectionUtils.SelectFirst<XmlNode>(theServerRule.RuleXml.ChildNodes,
                                                     delegate(XmlNode child) { return child.Name.Equals("rule"); });


            try
            {
                theRule.Compile(ruleNode, specCompiler, actionCompiler);
            }
            catch (Exception e)
            {
                errorDescription = e.Message;
                return false;
            }

            errorDescription = "Success";
            return true;
        }

        #endregion
    }
}