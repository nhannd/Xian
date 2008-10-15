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
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Rules engine for applying rules against DICOM files and performing actions.
    /// </summary>
    /// <remarks>
    /// The ServerRulesEngine encapsulates code to apply rules against DICOM file 
    /// objects.  It will load the rules from the persistent store, maintain them by type,
    /// and then can apply them against specific files.
    /// </remarks>
    /// <seealso cref="ServerActionContext"/>
    /// <example>
    /// Here is an example rule for routing all images with Modality set to CT to an AE
    /// Title CLEARCANVAS.
    /// <code>
    /// <rule id="CT Rule">
    ///   <condition expressionLanguage="dicom">
    ///     <equal test="$Modality" refValue="CT"/>
    ///   </condition>
    ///   <action>
    ///     <auto-route device="CLEARCANVAS"/>
    ///   </action>
    /// </rule
    /// </code>
    /// </example>
    public class ServerRulesEngine
    {
        private readonly ServerRuleApplyTimeEnum _applyTime;
        private readonly ServerRuleTypeEnum _ruleType = null;
        private readonly ServerEntityKey _serverPartitionKey;
        private readonly RuleEngineStatistics _stats;

        private readonly Dictionary<ServerRuleTypeEnum, RuleTypeCollection> _typeList =
            new Dictionary<ServerRuleTypeEnum, RuleTypeCollection>();

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// A rules engine will only load rules that apply at a specific time.  The
        /// apply time is specified by the <paramref name="applyTime"/> parameter.
        /// </remarks>
        /// <param name="applyTime">An enumerated value as to when the rules shall apply.</param>
        /// <param name="serverPartitionKey">The Server Partition the rules engine applies to.</param>
        public ServerRulesEngine(ServerRuleApplyTimeEnum applyTime, ServerEntityKey serverPartitionKey)
        {
            _applyTime = applyTime;
            _serverPartitionKey = serverPartitionKey;

            _stats = new RuleEngineStatistics(applyTime.Lookup, applyTime.LongDescription);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// A rules engine typically will only load rules that apply at a specific time.  The
        /// apply time is specified by the <paramref name="applyTime"/> parameter.  This constructor
        /// also allows for the loading of only certain types of rules through the <paramref name="type"/>
        /// parameter.
        /// </remarks>
        /// <param name="applyTime">An enumerated value as to when the rules shall apply.</param>
        /// <param name="type">A enumerated value as to the type of rule to load.</param>
        /// <param name="serverPartitionKey">The Server Partition the rules engine applies to.</param>
        public ServerRulesEngine(ServerRuleApplyTimeEnum applyTime, ServerRuleTypeEnum type,
                                 ServerEntityKey serverPartitionKey)
        {
            _applyTime = applyTime;
            _serverPartitionKey = serverPartitionKey;
            _ruleType = type;

            _stats = new RuleEngineStatistics(applyTime.Lookup, applyTime.LongDescription);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ServerRuleApplyTimeEnum"/> for the rules engine.
        /// </summary>
        public ServerRuleApplyTimeEnum RuleApplyTime
        {
            get { return _applyTime; }
        }

        /// <summary>
        /// Gets the <see cref="RuleEngineStatistics"/> for the rules engine.
        /// </summary>
        public RuleEngineStatistics Statistics
        {
            get { return _stats; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load the rules engine from the Persistent Store and compile the conditions and actions.
        /// </summary>
        public void Load()
        {
            _stats.LoadTime.Start();

            // Clearout the current type list.
            _typeList.Clear();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IServerRuleEntityBroker broker = read.GetBroker<IServerRuleEntityBroker>();

                ServerRuleSelectCriteria criteria = new ServerRuleSelectCriteria();
                criteria.Enabled.EqualTo(true);
                criteria.ServerRuleApplyTimeEnum.EqualTo(_applyTime);
                criteria.ServerPartitionKey.EqualTo(_serverPartitionKey);
                if (_ruleType != null)
                    criteria.ServerRuleTypeEnum.EqualTo(_ruleType);

                IList<ServerRule> list = broker.Find(criteria);

                // Create the specification and action compilers
                // We'll compile the rules right away
                XmlSpecificationCompiler specCompiler = new XmlSpecificationCompiler("dicom");
                XmlActionCompiler<ServerActionContext, ServerRuleTypeEnum> actionCompiler = new XmlActionCompiler<ServerActionContext, ServerRuleTypeEnum>();

                foreach (ServerRule serverRule in list)
                {
                    try
                    {
                        Rule theRule = new Rule();
                        theRule.Name = serverRule.RuleName;
                    	theRule.IsDefault = serverRule.DefaultRule;
                    	theRule.IsExempt = serverRule.ExemptRule;
                        theRule.Description = serverRule.ServerRuleApplyTimeEnum.Description;

                        XmlNode ruleNode =
                            CollectionUtils.SelectFirst<XmlNode>(serverRule.RuleXml.ChildNodes,
                                                                 delegate(XmlNode child) { return child.Name.Equals("rule"); });


						theRule.Compile(ruleNode, serverRule.ServerRuleTypeEnum, specCompiler, actionCompiler);

                        RuleTypeCollection typeCollection;

                        if (!_typeList.ContainsKey(serverRule.ServerRuleTypeEnum))
                        {
                            typeCollection = new RuleTypeCollection(serverRule.ServerRuleTypeEnum);
                            _typeList.Add(serverRule.ServerRuleTypeEnum, typeCollection);
                        }
                        else
                        {
                            typeCollection = _typeList[serverRule.ServerRuleTypeEnum];
                        }

                        typeCollection.AddRule(theRule);
                    }
                    catch (Exception e)
                    {
                        // something wrong with the rule...
                        Platform.Log(LogLevel.Warn, e, "Unable to add rule {0} to the engine. It will be skipped",
                                     serverRule.RuleName);
                    }
                }
            }

            _stats.LoadTime.End();
        }

        /// <summary>
        /// Execute the rules against the context for the rules.
        /// </summary>
        /// <param name="context">A class containing the context for applying the rules.</param>
        public void Execute(ServerActionContext context)
        {
            _stats.ExecutionTime.Start();

            foreach (RuleTypeCollection typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context, false);
            }

            _stats.ExecutionTime.End();
        }

		/// <summary>
		/// Execute the rules against the context for the rules.
		/// </summary>
		/// <param name="context">A class containing the context for applying the rules.</param>
		/// <param name="stopOnFirst">Stop on first valid rule of type.</param>
		public void Execute(ServerActionContext context, bool stopOnFirst)
		{
			_stats.ExecutionTime.Start();

			foreach (RuleTypeCollection typeCollection in _typeList.Values)
			{
				typeCollection.Execute(context, stopOnFirst);
			}

			_stats.ExecutionTime.End();
		}
        #endregion
    }
}