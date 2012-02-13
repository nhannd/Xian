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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class AddAuthorityGroupsDialog : UserControl
    {
        public IList<StudySummary> AuthorityGroupStudies
        {
            get
            {
                return ViewState["AuthorityGroupStudies"] as IList<StudySummary>;
            }
            set { ViewState["AuthorityGroupStudies"] = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup))
            {
                using (AuthorityManagement service = new AuthorityManagement())
                {
                    IList<AuthorityGroupSummary> tokens = service.ListDataAccessAuthorityGroups();
                    IList<ListItem> items = CollectionUtils.Map(
                        tokens,
                        delegate(AuthorityGroupSummary group)
                            {
                                ListItem item = new ListItem(group.Name, group.AuthorityGroupRef.ToString(false, false));
                                item.Attributes["title"] = group.Description;
                                item.Selected = false;
                                return item;
                            });

                    AuthorityGroupCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
                }
            }
        }

        public override void DataBind()
        {
            StudyListing.DataSource = AuthorityGroupStudies;

            base.DataBind();
        }       

        protected void AddButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (Page.IsValid)
            {
                try
                {
                    List<string> assignedGroups = new List<string>();
                    foreach (ListItem item in AuthorityGroupCheckBoxList.Items)
                    {
                        if (item.Selected)
                            assignedGroups.Add(item.Value);
                        item.Selected = false;
                    }

                    foreach (StudySummary study in AuthorityGroupStudies)
                    {
                        try
                        {
                            StudyDataAccessController controller = new StudyDataAccessController();                            
                            controller.AddStudyAuthorityGroups(study.StudyInstanceUid, study.AccessionNumber, study.TheStudyStorage.Key, assignedGroups);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex, "AddClicked failed: Unable to add authority groups to studies");
                            throw;
                        }
                    }           
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                EnsureDialogVisible();
            }
        }

        protected void CancelButton_Clicked(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        internal void EnsureDialogVisible()
        {
            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        public void Initialize(IList<StudySummary> list)
        {
            AuthorityGroupStudies = list;
        }

        internal void Show()
        {
            DataBind();
            EnsureDialogVisible();
        }
    }
}