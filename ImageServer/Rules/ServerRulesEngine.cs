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
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

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
        #region Private Members
        private readonly ServerRuleApplyTimeEnum _applyTime;
        private readonly ServerEntityKey _serverPartitionKey;
        private readonly Dictionary<ServerRuleTypeEnum, RuleTypeCollection> _typeList = new Dictionary<ServerRuleTypeEnum, RuleTypeCollection>();
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// A rules engine will only load rules that apply at a specific time.  The
        /// apply time is specified by the <paramref name="applyTime"/> parameter.
        /// </remarks>
        /// <param name="applyTime">An enumerater value as to when the rules shall apply.</param>
        /// <param name="serverPartitionKey">The Server Partition the rules engine applies to.</param>
        public ServerRulesEngine(ServerRuleApplyTimeEnum applyTime, ServerEntityKey serverPartitionKey)
        {
            _applyTime = applyTime;
            _serverPartitionKey = serverPartitionKey;
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
        #endregion

        #region Events
        /// <summary>
        /// Defines the event handler for <see cref="ExecuteBeginEventHandler"/>.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="context"></param>
        public delegate void ExecuteBeginEventHandler(ServerRulesEngine engine, ServerActionContext context);
        /// <summary>
        /// Defines the event handler for <see cref="ExecuteCompletedEventHandler"/>.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="context"></param>
        public delegate void ExecuteCompletedEventHandler(ServerRulesEngine engine, ServerActionContext context);
        /// <summary>
        /// Defines the event handler for <see cref="LoadBeginEventHandler"/>.
        /// </summary>
        /// <param name="engines"></param>
        public delegate void LoadBeginEventHandler(ServerRulesEngine engines);
        /// <summary>
        /// Defines the event handler for <see cref="LoadCompletedEventHandler"/>.
        /// </summary>
        /// <param name="engines"></param>
        public delegate void LoadCompletedEventHandler(ServerRulesEngine engines);

        /// <summary>
        /// Occurs when <see cref="Execute"/> is called.
        /// </summary>
        public event ExecuteBeginEventHandler ExecuteBegin;
        /// <summary>
        /// Occurs before <see cref="Execute"/> returns.
        /// </summary>
        public event ExecuteCompletedEventHandler ExecuteCompleted;
        /// <summary>
        /// Occurs when <see cref="Load"/> is called.
        /// </summary>
        public event LoadBeginEventHandler LoadBegin;
        /// <summary>
        /// Occurs before <see cref="Load"/> returns.
        /// </summary>
        public event LoadCompletedEventHandler LoadCompleted;

        #endregion Events

        #region Public Methods
        /// <summary>
        /// Load the rules engine from the Persistent Store and compile the conditions and actions.
        /// </summary>
        public void Load()
        {
            if (LoadBegin != null)
                LoadBegin(this);

            // Clearout the current type list.
            _typeList.Clear();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                ISelectServerRule broker = read.GetBroker<ISelectServerRule>();

                ServerRuleSelectCriteria criteria = new ServerRuleSelectCriteria();
                criteria.Active.EqualTo(true);
                criteria.ServerRuleApplyTimeEnum.EqualTo(_applyTime);
                criteria.ServerPartitionKey.EqualTo(_serverPartitionKey);

                IList<ServerRule> list = broker.Find(criteria);

                // Create the specification and action compilers
                // We'll compile the rules right away
                XmlSpecificationCompiler specCompiler = new XmlSpecificationCompiler("dicom");
                XmlActionCompiler<ServerActionContext> actionCompiler = new XmlActionCompiler<ServerActionContext>();

                foreach (ServerRule serverRule in list)
                {
                    Rule theRule = new Rule(serverRule);
                    theRule.Compile(specCompiler, actionCompiler);

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
            }

            if (LoadCompleted != null)
                LoadCompleted(this);

        }

        /// <summary>
        /// Execute the rules against the context for the rules.
        /// </summary>
        /// <param name="context">A class containing the context for applying the rules.</param>
        public void Execute(ServerActionContext context)
        {
            if (ExecuteBegin != null)
                ExecuteBegin(this, context);

            foreach (RuleTypeCollection typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context);
            }

            if (ExecuteCompleted != null)
                ExecuteCompleted(this, context);
        }

        #endregion
    }
}
