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
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Rules
{
    public class ServerRulesEngine
    {
        #region Private Members
        private readonly ServerRuleApplyTimeEnum _applyTime;
        private readonly Dictionary<ServerRuleTypeEnum, RuleTypeCollection> _typeList = new Dictionary<ServerRuleTypeEnum, RuleTypeCollection>();
        #endregion

        #region Constructors
        public ServerRulesEngine(ServerRuleApplyTimeEnum applyTime)
        {
            _applyTime = applyTime;
        }
        #endregion

        #region Properties
        public ServerRuleApplyTimeEnum RuleApplyTime
        {
            get { return _applyTime; }
        }
        #endregion

        #region Public Methods
        public void Load()
        {
            // Clearout the current type list.
            _typeList.Clear();

            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            ISelectServerRule broker = read.GetBroker<ISelectServerRule>();
 
            ServerRuleSelectCriteria critera = new ServerRuleSelectCriteria();
            critera.Active.EqualTo(true);
            critera.ServerRuleApplyTimeEnum.EqualTo(_applyTime);

            IList<ServerRule> list = broker.Find(critera);

            read.Dispose();

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

        public void Execute(ServerActionContext context)
        {
            foreach (RuleTypeCollection typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context);
            }
        }

        #endregion
    }
}
