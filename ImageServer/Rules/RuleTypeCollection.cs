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