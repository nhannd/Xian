using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;
using ClearCanvas.ImageServer.Model.UpdateBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    internal class ServerRuleAdaptor : BaseAdaptor<ServerRule,ServerRuleSelectCriteria,ISelectServerRule,IUpdateServerRuleBroker,UpdateServerRuleParameters>
    {
        
    }

    public class ServerRuleController : BaseController
    {
        #region Private Members
        private readonly ServerRuleAdaptor _adaptor = new ServerRuleAdaptor();
        #endregion

        #region Public Methods
        public IList<ServerRule> GetServerRules(ServerRuleSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public bool DeleteServerRule(ServerRule rule)
        {
            return _adaptor.Delete(rule.GetKey());
        }

        public bool AddServerRule(ServerRule rule)
        {
                UpdateServerRuleParameters parms = new UpdateServerRuleParameters();

                parms.Default = rule.DefaultRule;
                parms.Enabled = rule.Enabled;
                parms.RuleName = rule.RuleName;
                parms.RuleXml = rule.RuleXml;
                parms.ServerPartitionKey = rule.ServerPartitionKey;
                parms.ServerRuleApplyTime = rule.ServerRuleApplyTimeEnum;
                parms.ServerRuleType = rule.ServerRuleTypeEnum;

             return   _adaptor.Add(parms);
        
        }
        public bool UpdateServerRule(ServerRule rule)
        {
            UpdateServerRuleParameters parms = new UpdateServerRuleParameters();

            parms.Default = rule.DefaultRule;
            parms.Enabled = rule.Enabled;
            parms.RuleName = rule.RuleName;
            parms.RuleXml = rule.RuleXml;
            parms.ServerPartitionKey = rule.ServerPartitionKey;
            parms.ServerRuleApplyTime = rule.ServerRuleApplyTimeEnum;
            parms.ServerRuleType = rule.ServerRuleTypeEnum;

            return _adaptor.Update(rule.GetKey(), parms);
        }
        #endregion
    }
}
