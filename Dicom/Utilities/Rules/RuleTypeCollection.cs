#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// A collection of rules of similar types.
    /// </summary>
    /// <typeparam name="TContext">The context for the rules.</typeparam>
    /// <typeparam name="TTypeEnum">The type of the rule.</typeparam>
    public class RuleTypeCollection<TContext, TTypeEnum>
        where TContext : ActionContext
    {
        private readonly List<Rule<TContext, TTypeEnum>> _exemptRuleList = new List<Rule<TContext, TTypeEnum>>();
        private readonly List<Rule<TContext, TTypeEnum>> _ruleList = new List<Rule<TContext, TTypeEnum>>();

        #region Constructors

        public RuleTypeCollection(TTypeEnum type)
        {
            Type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of rule for the collection.
        /// </summary>
        public TTypeEnum Type { get; private set; }

        /// <summary>
        /// The identified default rule for the collection of rules.
        /// </summary>
        public Rule<TContext, TTypeEnum> DefaultRule { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a rule to the collection.
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(Rule<TContext, TTypeEnum> rule)
        {
            if (rule.IsDefault)
            {
                if (DefaultRule != null)
                {
                    Platform.Log(LogLevel.Error, "Unexpected multiple default rules for rule {0} of type {1}",
                                 rule.Name, rule.Description);
                    Platform.Log(LogLevel.Error, "Ignoring rule {0}", rule.Name);
                }
                else
                    DefaultRule = rule;
            }
            else if (rule.IsExempt)
                _exemptRuleList.Add(rule);
            else
                _ruleList.Add(rule);
        }

        /// <summary>
        /// Execute the rules within the <see cref="RuleTypeCollection{TActionContext,TTypeEnum}"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stopOnFirst"></param>
        public void Execute(TContext context, bool stopOnFirst)
        {
            bool doDefault = true;
            try
            {
                foreach (Rule<TContext, TTypeEnum> theRule in _exemptRuleList)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

                    theRule.Execute(context, false, out ruleApplied, out ruleSuccess);

                    if (ruleApplied)
                    {
                        Platform.Log(LogLevel.Info, "Exempt rule found that applies for {0}, ignoring action.", Type.ToString());
                        return;
                    }
                }

                foreach (Rule<TContext, TTypeEnum> theRule in _ruleList)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

                    theRule.Execute(context, false, out ruleApplied, out ruleSuccess);

                    if (ruleApplied && ruleSuccess)
                    {
                        if (stopOnFirst)
                            return;

                        doDefault = false;
                    }
                }

                if (doDefault && DefaultRule != null)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

                    DefaultRule.Execute(context, true, out ruleApplied, out ruleSuccess);

                    if (!ruleSuccess)
                    {
                        Platform.Log(LogLevel.Error, "Unable to apply default rule of type {0}", Type);
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when applying rule of type: {0}", Type);
            }
        }

        #endregion
    }
}