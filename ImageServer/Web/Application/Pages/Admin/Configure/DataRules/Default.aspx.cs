#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.DataRules
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup)]
    public partial class Default : BasePage
    {
        private readonly ServerRuleController _controller = new ServerRuleController();

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
            {
                var panel =
                    LoadControl("DataRulePanel.ascx") as
                    DataRulePanel;
                if (panel != null)
                {
                    panel.ServerPartition = partition;
                    panel.ID = "DataRulePanel_" + partition.AeTitle;

                    panel.EnclosingPage = this;
                }
                return panel;
            });

            ConfirmDialog.Confirmed += delegate(object data)
            {
                // delete the device and reload the affected partition.
                var key = data as ServerEntityKey;

                ServerRule rule = ServerRule.Load(key);

                _controller.DeleteServerRule(rule);

                ServerPartitionTabs.Update(rule.ServerPartitionKey);
            };


            AddEditDataRuleControl.OKClicked += delegate(ServerRule rule)
            {
                if (AddEditDataRuleControl.Mode == AddEditDataRuleDialogMode.Edit)
                {
                    // Commit the change into database
                    if (_controller.UpdateServerRule(rule))
                    {
                    }
                    else
                    {
                        // TODO: alert user
                    }
                }
                else
                {
                    // Create new device in the database
                    ServerRule newRule = _controller.AddServerRule(rule);
                    if (newRule != null)
                    {
                    }
                    else
                    {
                        //TODO: alert user
                    }
                }

                ServerPartitionTabs.Update(rule.ServerPartitionKey);
            };


            SetPageTitle(Titles.DataRulesPageTitle);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                ServerPartitionTabs.Update(0);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Displays a popup dialog box for users to edit a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnEditRule(ServerRule rule, ServerPartition partition)
        {
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.Edit;
            AddEditDataRuleControl.ServerRule = rule;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to edit a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnCopyRule(ServerRule rule, ServerPartition partition)
        {
            ServerRule copiedRule = new ServerRule(rule.RuleName + " (Copy)",rule.ServerPartitionKey,rule.ServerRuleTypeEnum, rule.ServerRuleApplyTimeEnum, rule.Enabled, rule.DefaultRule, rule.ExemptRule, (XmlDocument)rule.RuleXml.CloneNode(true));

            // Store a dummy entity key
            copiedRule.SetKey(new ServerEntityKey("ServerRule",Guid.NewGuid()));
 
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.Copy;
            AddEditDataRuleControl.ServerRule = copiedRule;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to delete a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnDeleteRule(ServerRule rule, ServerPartition partition)
        {
            ConfirmDialog.Message = string.Format(SR.AdminServerRules_DeleteDialog_AreYouSure, rule.RuleName, partition.AeTitle);
            ConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
            ConfirmDialog.Data = rule.GetKey();
            ConfirmDialog.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to add a new rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnAddRule(ServerRule rule, ServerPartition partition)
        {
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.New;
            AddEditDataRuleControl.ServerRule = null;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        #endregion Public Methods
    }
}
