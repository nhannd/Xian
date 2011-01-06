#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

		public void Compile(XmlNode ruleNode, ServerRuleTypeEnum ruleType, XmlSpecificationCompiler specCompiler,
                            XmlActionCompiler<ServerActionContext,ServerRuleTypeEnum> actionCompiler)
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
				_actions = actionCompiler.Compile(actionNode as XmlElement, ruleType, true);
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
        /// <param name="type">The type of rule to validate</param>
        /// <param name="rule">The rule to validate</param>
        /// <param name="errorDescription">A failure description on error.</param>
        /// <returns>true on successful validation, otherwise false.</returns>
        public static bool ValidateRule(ServerRuleTypeEnum type, XmlDocument rule, out string errorDescription)
        {
            XmlSpecificationCompiler specCompiler = new XmlSpecificationCompiler("dicom");
            XmlActionCompiler<ServerActionContext,ServerRuleTypeEnum> actionCompiler = new XmlActionCompiler<ServerActionContext,ServerRuleTypeEnum>();

            ServerRule theServerRule = new ServerRule();
            theServerRule.RuleXml = rule;
        	theServerRule.ServerRuleTypeEnum = type;

            Rule theRule = new Rule();
            theRule.Name = theServerRule.RuleName;

            XmlNode ruleNode =
                CollectionUtils.SelectFirst<XmlNode>(theServerRule.RuleXml.ChildNodes,
                                                     delegate(XmlNode child) { return child.Name.Equals("rule"); });


            try
            {
                theRule.Compile(ruleNode, type, specCompiler, actionCompiler);
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