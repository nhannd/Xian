#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class AddAuthorityGroupDialog : UserControl
    {
        private EventHandler<EventArgs> _authorityGroupEditedHandler;

        public event EventHandler<EventArgs> AuthorityGroupsEdited
        {
            add { _authorityGroupEditedHandler += value; }
            remove { _authorityGroupEditedHandler -= value; }
        }

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public StudySummary Study
        {
            get; set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack || Study == null) return;
            if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup))
            {

                StudyDataAccessController controller = new StudyDataAccessController();
                IList<StudyDataAccessSummary> list = controller.LoadStudyDataAccess(Study.TheStudyStorage.Key);

                using (AuthorityManagement service = new AuthorityManagement())
                {
                    IList<AuthorityGroupSummary> tokens = service.ListDataAccessAuthorityGroups();
                    IList<ListItem> items = CollectionUtils.Map(
                        tokens,
                        delegate(AuthorityGroupSummary group)
                            {
                                ListItem item = new ListItem(group.Name, group.AuthorityGroupRef.ToString(false, false));
                                item.Attributes["title"] = group.Description;

                                foreach (StudyDataAccessSummary s in list)
                                {
                                    if (s.AuthorityGroupOID.Equals(group.AuthorityGroupRef.ToString(false, false)))
                                        item.Selected = true;
                                }
                                return item;
                            });

                    AuthorityGroupCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
                }
            }
        }

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            //If the validation failed, keep everything as is, and 
            //make sure the dialog stays visible.
            if (!Page.IsValid)
            {
                ModalDialog.Show();
                return;
            }

            CancelButton.Visible = true;
            UpdateButton.Visible = true;

            ModalDialog.Show();
            return;
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        protected void UpdateButton_Click(object sender, ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                List<string> assignedGroups = new List<string>();
                foreach (ListItem item in AuthorityGroupCheckBoxList.Items)
                {
                    if (item.Selected)
                        assignedGroups.Add(item.Value);
                }

                StudyDataAccessController controller = new StudyDataAccessController();
                controller.UpdateStudyAuthorityGroups(Study.StudyInstanceUid, Study.AccessionNumber, Study.TheStudyStorage.Key, assignedGroups);
                
                OnAuthorityGroupsUpdated();
                
                Close();
            }
            else
            {
                Show();
            }
        }

        protected void CancelButton_Click(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        private void OnAuthorityGroupsUpdated()
        {
            EventArgs args = new EventArgs();
            EventsHelper.Fire(_authorityGroupEditedHandler, this, args);
        }
    }
}