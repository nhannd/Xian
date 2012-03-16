#region License

// Copyright (c) 2012, ClearCanvas Inc.
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

namespace ClearCanvas.Dicom.Utilities.Rules
{
    public class Rule<TContext,TTypeEnum>
        where TContext : ActionContext
    {
        private IActionSet<TContext> _actions;
        private ISpecification _conditions;

        #region Constructors

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public bool IsExempt { get; set; }

        #endregion

        #region Public Methods

        public void Compile(XmlNode ruleNode, TTypeEnum ruleType, XmlSpecificationCompiler specCompiler,
                            XmlActionCompiler<TContext, TTypeEnum> actionCompiler)
        {
            var conditionNode =
                CollectionUtils.SelectFirst(ruleNode.ChildNodes,
                                            (XmlNode child) => child.Name.Equals("condition"));

            if (conditionNode != null)
                _conditions = specCompiler.Compile(conditionNode as XmlElement, true);
            else if (!IsDefault)
                throw new ApplicationException("No condition element defined for the rule.");
            else
                _conditions = new AndSpecification();

            var actionNode =
                CollectionUtils.SelectFirst(ruleNode.ChildNodes,
                                            (XmlNode child) => child.Name.Equals("action"));

            if (actionNode != null)
                _actions = actionCompiler.Compile(actionNode as XmlElement, ruleType, true);
            else if (!IsExempt)
                throw new ApplicationException("No action element defined for the rule.");
            else
                _actions = new ActionSet<TContext>(new List<IActionItem<TContext>>());
        }

        /// <summary>
        /// Returns true if the rule condition is passed for the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TestResult Test(TContext context)
        {
            if (IsDefault)
                return new TestResult(true);

            return _conditions.Test(context.Message);
        }


        public void Execute(TContext context, bool defaultRule, out bool ruleApplied, out bool ruleSuccess)
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
        public static bool ValidateRule(TTypeEnum type, XmlDocument rule, out string errorDescription)
        {
            var specCompiler = new XmlSpecificationCompiler("dicom");
            var actionCompiler = new XmlActionCompiler<TContext, TTypeEnum>();


            var theRule = new Rule<TContext, TTypeEnum>
                              {
                                  Name = string.Empty
                              };

            var ruleNode =
                CollectionUtils.SelectFirst(rule.ChildNodes,
                                            (XmlNode child) => child.Name.Equals("rule"));


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