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
using System.Security.Permissions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.Search)]
    public partial class Default : BasePage
    {
        private readonly Dictionary<string, SearchPanel> _partitionPanelMap = new Dictionary<string,SearchPanel>();
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServerPartitionTabs.SetupLoadPartitionTabs(CreatePartitionTab);
            DeleteStudyConfirmDialog.StudyDeleted += DeleteStudyConfirmDialog_StudyDeleted;
            
            SetPageTitle(Titles.StudiesPageTitle);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !Page.IsAsync)
            {
                string aeTitle = Request["AETitle"];

                if (aeTitle != null)
                {
                    RefreshPartitionTab(aeTitle);
                    ServerPartitionTabs.SetActivePartition(aeTitle);
                }
            }
        }

        private SearchPanel CreatePartitionTab(ServerPartition partition)
        {
            SearchPanel panel = LoadControl("SearchPanel.ascx") as SearchPanel;
            if (panel != null)
            {
                panel.ServerPartition = partition;
                panel.ID = "SearchPanel_" + partition.AeTitle;
                panel.DeleteButtonClicked += SearchPanel_DeleteButtonClicked;
                panel.AssignAuthorityGroupsButtonClicked += SearchPanel_AssignAuthorityGroupsButtonClicked;
                _partitionPanelMap.Add(partition.AeTitle, panel);
            }
            return panel;
        }

        private void DeleteStudyConfirmDialog_StudyDeleted(object sender, DeleteStudyConfirmDialogStudyDeletedEventArgs e)
        {
            IList<string> changedPartitions = new List<string>();

            foreach (DeleteStudyInfo study in e.DeletedStudies)
            {
                if(!changedPartitions.Contains(study.ServerPartitionAE))
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

        private void SearchPanel_DeleteButtonClicked(object sender, SearchPanelButtonClickedEventArgs e)
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
                    var info = new DeleteStudyInfo
                                   {
                                       StudyKey = study.Key,
                                       ServerPartitionAE = study.ThePartition.AeTitle,
                                       AccessionNumber = study.AccessionNumber,
                                       Modalities = study.ModalitiesInStudy,
                                       PatientId = study.PatientId,
                                       PatientsName = study.PatientsName,
                                       StudyDate = study.StudyDate,
                                       StudyDescription = study.StudyDescription,
                                       StudyInstanceUid = study.StudyInstanceUid
                                   };
                    return info;
                }
                ));
            DeleteStudyConfirmDialog.Show();
        }

        private void SearchPanel_AssignAuthorityGroupsButtonClicked(object sender, SearchPanelButtonClickedEventArgs e)
        {
            List<StudySummary> list = new List<StudySummary>();
            list.AddRange(e.SelectedStudies);
            ShowAddAuthorityGroupDialog(list);
        }

        protected void ShowAddAuthorityGroupDialog(IList<StudySummary> studyList)
        {
            AddAuthorityGroupsDialog.Initialize(studyList);
            AddAuthorityGroupsDialog.Show();
        }
    }
}
