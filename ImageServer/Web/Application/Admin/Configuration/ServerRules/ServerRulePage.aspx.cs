using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules
{
    public partial class ServerRulePage : BasePage
    {
        private ServerRuleController _controller = new ServerRuleController();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {            
            ServerPartitionTabs.SetupLoadPartitionTabs( delegate(ServerPartition partition)
                                                       {
                                                           ServerRulePanel panel =
                                                               LoadControl("ServerRulePanel.ascx") as ServerRulePanel;
                                                           panel.ServerPartition = partition;
                                                           panel.ID = "ServerRulePanel_" + partition.AeTitle;

                                                           panel.EnclosingPage = this;

                                                           return panel;
                                                       });

            ConfirmDialog.Confirmed += delegate(object data)
                                        {
                                            // delete the device and reload the affected partition.
                                            ServerEntityKey key = data as ServerEntityKey;

                                            ServerRule rule = ServerRule.Load(key);

                                            UserControl control =
                                                ServerPartitionTabs.GetUserControlForPartition(rule.ServerPartitionKey);

                                            this._controller.DeleteServerRule(rule);

                                            ServerRulePanel panel = control as ServerRulePanel;

                                            panel.UpdateUI();
                                        };
            
            
            AddEditServerRuleControl.OKClicked += delegate(ServerRule rule)
                                               {
                                                   if (AddEditServerRuleControl.EditMode)
                                                   {
                                                       // Commit the change into database
                                                       if (_controller.UpdateServerRule(rule))
                                                       {
                                                           ServerRulePanel panel =
                                                               this.ServerPartitionTabs.GetUserControlForPartition(
                                                                   rule.ServerPartitionKey) as ServerRulePanel;
                                                           panel.UpdateUI();
                                                       }
                                                   }
                                                   else
                                                   {
                                                       // Create new device in the database
                                                       if (_controller.AddServerRule(rule))
                                                       {
                                                           ServerRulePanel panel = this.ServerPartitionTabs.GetUserControlForPartition(
                                                               rule.ServerPartitionKey) as ServerRulePanel;
                                                           panel.UpdateUI();
                                                       }
                                                   }


                                               };
            
            base.OnInit(e);       
        }

        public void OnEditRule(ServerRule rule, ServerPartition partition)
        {
            AddEditServerRuleControl.EditMode = true;
            AddEditServerRuleControl.ServerRule = rule;
            AddEditServerRuleControl.Partition = partition;
            AddEditServerRuleControl.Show();
        }

        public void OnDeleteRule(ServerRule rule, ServerPartition partition)
        {
            ConfirmDialog.Message = string.Format("Are you sure you want to remove rule \"{0}\" from partition {1}?", rule.RuleName, partition.AeTitle);
            ConfirmDialog.MessageType = ConfirmDialog.MessageTypeEnum.WARNING;
            ConfirmDialog.Data = rule.GetKey();
            ConfirmDialog.Show();
        }

        public void OnAddRule(ServerRule rule, ServerPartition partition)
        {
            AddEditServerRuleControl.EditMode = false;
            AddEditServerRuleControl.ServerRule = null;
            AddEditServerRuleControl.Partition = partition;
            AddEditServerRuleControl.Show();
        }
    }
}
