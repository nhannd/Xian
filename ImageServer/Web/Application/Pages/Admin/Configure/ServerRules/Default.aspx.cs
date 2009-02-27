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
using System.Security.Permissions;
using System.Web.UI;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.Admin.Configuration.ServerRules)]
    public partial class Default : BasePage
    {
        #region Private Members
        private ServerRuleController _controller = new ServerRuleController();

        #endregion Private Members

        #region Protected Methods

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

                                            _controller.DeleteServerRule(rule);

                                            ServerPartitionTabs.Update(rule.ServerPartitionKey);
                                        };
            
            
            AddEditServerRuleControl.OKClicked += delegate(ServerRule rule)
                                               {
                                                   if (AddEditServerRuleControl.EditMode)
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
                                                       if ( newRule!=null)
                                                       {
                                                       }
                                                       else
                                                       {
                                                           //TODO: alert user
                                                       }
                                                   }

                                                   ServerPartitionTabs.Update(rule.ServerPartitionKey);
                                               };
            
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
            AddEditServerRuleControl.EditMode = true;
            AddEditServerRuleControl.ServerRule = rule;
            AddEditServerRuleControl.Partition = partition;
            AddEditServerRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to delete a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnDeleteRule(ServerRule rule, ServerPartition partition)
        {
            ConfirmDialog.Message = string.Format("Are you sure you want to remove rule \"{0}\" from partition {1}?", rule.RuleName, partition.AeTitle);
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
            AddEditServerRuleControl.EditMode = false;
            AddEditServerRuleControl.ServerRule = null;
            AddEditServerRuleControl.Partition = partition;
            AddEditServerRuleControl.Show();
        }

        #endregion Public Methods
    }
}
