#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using System.Security.Permissions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.Study.Search)]
    public partial class Default : BasePage
    {
        private readonly Dictionary<string, SearchPanel> _partitionPanelMap = new Dictionary<string,SearchPanel>();
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServerPartitionTabs.SetupLoadPartitionTabs(CreatePartitionTab);
            DeleteStudyConfirmDialog.StudyDeleted += DeleteStudyConfirmDialog_StudyDeleted;

            Page.Title = App_GlobalResources.Titles.StudiesPageTitle;
        }

        private SearchPanel CreatePartitionTab(ServerPartition partition)
        {
            SearchPanel panel = LoadControl("SearchPanel.ascx") as SearchPanel;
            panel.ServerPartition = partition;
            panel.ID = "SearchPanel_" + partition.AeTitle;
            panel.DeleteButtonClicked += SearchPanel_DeleteButtonClicked;
            _partitionPanelMap.Add(partition.AeTitle, panel);
            return panel;
        }

        private void DeleteStudyConfirmDialog_StudyDeleted(object sender, DeleteStudyConfirmDialogStudyDeletedEventArgs e)
        {
            IList<string> changedPartitions = new List<string>();

            foreach (DeleteStudyInfo study in e.DeletedStudies)
            {
                changedPartitions.Add(study.ServerPartitionAE);
            }
            foreach (string partitionAE in changedPartitions)
            {
                RefreshPartitionTab(partitionAE);
            }
        }

        private void RefreshPartitionTab(string partitionAE)
        {
            if (_partitionPanelMap.ContainsKey(partitionAE))
                _partitionPanelMap[partitionAE].Refresh();
        }

        private void SearchPanel_DeleteButtonClicked(object sender, SearchPanelDeleteButtonClickedEventArgs e)
        {
            List<StudySummary> list = new List<StudySummary>();
            list.AddRange(e.SelectedStudies);
            ShowDeletedDialog(list);
        }

        protected void ShowDeletedDialog(IList<StudySummary> studyList)
        {
            DeleteStudyConfirmDialog.Initialize(CollectionUtils.Map<StudySummary, DeleteStudyInfo>(
                studyList,
                delegate(StudySummary study)
                {
                    DeleteStudyInfo info = new DeleteStudyInfo();
                    info.StudyKey = study.TheStudy.GetKey();
                    info.ServerPartitionAE = study.ThePartition.AeTitle;
                    info.AccessionNumber = study.AccessionNumber;
                    info.Modalities = study.ModalitiesInStudy;
                    info.PatientId = study.PatientId;
                    info.PatientsName = study.PatientsName;
                    info.StudyDate = study.StudyDate;
                    info.StudyDescription = study.StudyDescription;
                    info.StudyInstanceUid = study.StudyInstanceUid;
                    return info;
                }
                ));
            DeleteStudyConfirmDialog.Show();
        }

    }
}
