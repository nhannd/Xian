#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using AjaxControlToolkit;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.WebViewer
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.WebViewer.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.TheGrid.ClientID; }
        }
       
        [ExtenderControlProperty]
        [ClientPropertyName("CanViewImages")]
        public bool CanViewImages
        {
            get { return Thread.CurrentPrincipal.IsInRole(ImageServerConstants.WebViewerAuthorityToken); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewImageButtonClientID")]
        public string ViewImageButtonClientID
        {
            get { return ViewImageButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewImagePageUrl")]
        public string ViewImagePageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.WebViewerDefaultPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("Username")]
        public string Username
        {
            get; set;
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SessionId")]
        public string SessionId
        {
            get; set;
        }

        public WebViewerInitParams InitParams
        { get; set; }
        
        #endregion Public Properties  

        #region Private Methods

        private void SetupChildControls()
        {
           
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerStudySingleItem, App_GlobalResources.SR.GridPagerStudyMultipleItems, StudyListGridView.TheGrid, delegate { return StudyListGridView.Studies.Count; }, ImageServerConstants.GridViewPagerPosition.Top);
            StudyListGridView.Pager = GridPagerTop;
        }
        
        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        private static void SetStringCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("*") || val.Contains("?"))
            {
                String value = val.Replace("%", "[%]").Replace("_", "[_]");
                value = value.Replace('*', '%');
                value = value.Replace('?', '_');
                cond.Like(value);
            }
            else
                cond.EqualTo(val);
        }

        private static IList<StudySummary> LoadStudies(WebViewerInitParams initParams)
        {
            var controller = new StudyController();
            var partitionAdapter = new ServerPartitionDataAdapter();
            StudySelectCriteria studyCriteria;
            var partitionCriteria = new ServerPartitionSelectCriteria();
            ServerPartition partition = null;
            IList<Study> studies;
            List<StudySummary> totalStudies = new List<StudySummary>();

            if (!string.IsNullOrEmpty(initParams.AeTitle))
            {
                partitionCriteria.AeTitle.EqualTo(initParams.AeTitle);
                IList<ServerPartition> partitions = partitionAdapter.GetServerPartitions(partitionCriteria);
                if (partitions.Count == 1)
                {
                    partition = partitions[0];
                }
            }

            foreach (string patientId in initParams.PatientIds)
            {
                studyCriteria = new StudySelectCriteria();                
                if (partition != null) studyCriteria.ServerPartitionKey.EqualTo(partition.Key);
                SetStringCondition(studyCriteria.PatientId, patientId);
                studyCriteria.StudyDate.SortDesc(0);
                studies = controller.GetStudies(studyCriteria);

                foreach (Study study in studies)
                {
                    totalStudies.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));
                }
            }

            foreach (string accession in initParams.AccessionNumbers)
            {
                studyCriteria = new StudySelectCriteria();
                if (partition != null) studyCriteria.ServerPartitionKey.EqualTo(partition.Key);
                SetStringCondition(studyCriteria.AccessionNumber, accession);
                studyCriteria.StudyDate.SortDesc(0);
                studies = controller.GetStudies(studyCriteria);

                foreach (Study study in studies)
                {
                    totalStudies.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));
                }
            }

            if (initParams.StudyInstanceUids.Count > 0)
            {
                studyCriteria = new StudySelectCriteria();
                if (partition != null) studyCriteria.ServerPartitionKey.EqualTo(partition.Key);
                studyCriteria.StudyInstanceUid.In(initParams.StudyInstanceUids);
                studyCriteria.StudyDate.SortDesc(0);
                studies = controller.GetStudies(studyCriteria);

                foreach (Study study in studies)
                {
                    totalStudies.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));
                }
            }

            totalStudies.Sort((a, b) => a.StudyDate.CompareTo(b.StudyDate) * -1);

            return totalStudies;
        }

        #endregion Private Methods

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupChildControls();
            StudyListGridView.Studies = LoadStudies(InitParams);
            StudyListGridView.Refresh();
        }

        #endregion Protected Methods
    }
}