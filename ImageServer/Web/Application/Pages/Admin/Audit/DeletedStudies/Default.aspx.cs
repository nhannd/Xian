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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.StudyDeleteHistory.Search)]
    public partial class Default : BaseAdminPage
    {
        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            SearchPanel.ViewDetailsClicked += SearchPanel_ViewDetailsClicked;
            SearchPanel.DeleteClicked += SearchPanel_DeleteClicked;
            DeleteConfirmMessageBox.Confirmed += DeleteConfirmMessageBox_Confirmed;
            base.OnInit(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetPageTitle(App_GlobalResources.Titles.DeletedStudiesPageTitle);

            DataBind();
        }

        protected void Refresh()
        {
            DataBind();
            UpdatePanel1.Update();
        }
        #endregion

        #region Private Methods
        void DeleteConfirmMessageBox_Confirmed(object data)
        {
            try
            {
                ServerEntityKey record = data as ServerEntityKey;
                DeletedStudyController controller = new DeletedStudyController();
                controller.Delete(record);
            }
            finally
            {
                SearchPanel.Refresh();
            }
        }

        void SearchPanel_DeleteClicked(object sender, DeletedStudyDeleteClickedEventArgs e)
        {
            DeleteConfirmMessageBox.Data = e.SelectedItem.DeleteStudyRecord;
            DeleteConfirmMessageBox.Show();
        }

        void SearchPanel_ViewDetailsClicked(object sender, DeletedStudyViewDetailsClickedEventArgs e)
        {
            DeletedStudyDetailsDialogViewModel dialogViewModel = new DeletedStudyDetailsDialogViewModel();
            dialogViewModel.DeletedStudyRecord = e.DeletedStudyInfo;
            DetailsDialog.ViewModel = dialogViewModel;
            DetailsDialog.Show();
        }

        #endregion

    }
}