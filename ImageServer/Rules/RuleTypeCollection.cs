#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    public class RuleTypeCollection
    {
        private readonly List<Rule> _exemptRuleList = new List<Rule>();
        private readonly List<Rule> _ruleList = new List<Rule>();
        private readonly ServerRuleTypeEnum _type;
        private Rule _defaultRule;

        #region Constructors

        public RuleTypeCollection(ServerRuleTypeEnum type)
        {
            _type = type;
        }

        #endregion

        #region Public Properties

        public ServerRuleTypeEnum Type
        {
            get { return _type; }
        }

        public Rule DefaultRule
        {
            get { return _defaultRule; }
        }

        #endregion

        #region Public Methods

        public void AddRule(Rule rule)
        {
            if (rule.IsDefault)
            {
                if (_defaultRule != null)
                {
                    Platform.Log(LogLevel.Error, "Unexpected multiple default rules for rule {0} of type {1}",
                                 rule.Name, rule.Description);
                    Platform.Log(LogLevel.Error, "Ignoring rule {0}", rule.Name);
                }
                else
                    _defaultRule = rule;
            }
            else if (rule.IsExempt)
                _exemptRuleList.Add(rule);
            else
                _ruleList.Add(rule);
        }

        public void Execute(ServerActionContext context, bool stopOnFirst)
        {
            bool doDefault = true;
            try
            {
                foreach (Rule theRule in _exemptRuleList)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

                    theRule.Execute(context, false, out ruleApplied, out ruleSuccess);

                    if (ruleApplied)
                    {
                        Platform.Log(LogLevel.Info, "Exempt rule found that applies for {0}, ignoring action.", Type.Description);
                        return;
                    }
                }

                foreach (Rule theRule in _ruleList)
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