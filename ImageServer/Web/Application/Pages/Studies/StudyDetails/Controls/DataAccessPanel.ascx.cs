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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class DataAccessPanel : System.Web.UI.UserControl
    {
        #region Private members

        #endregion Private members

        #region Public properties

        /// <summary>
        /// The Study to get the DataAccess Groups for.
        /// </summary>
        public StudySummary Study { get; set; }

        /// <summary>
        /// Gets or sets the list of Data Access Groups to be displayed
        /// </summary>
        protected IList<StudyDataAccessSummary> DataAccessGroups { get; set; }

        public IList<StudyDataAccessSummary> SelectedItems
        {
            get
            {
                if (!AccessListControl.IsDataBound) DataBind();

                if (DataAccessGroups == null || DataAccessGroups.Count == 0)
                    return null;

                int[] rows = AccessListControl.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<StudyDataAccessSummary> dataAccessSummaries = new List<StudyDataAccessSummary>();
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] < DataAccessGroups.Count)
                    {
                        dataAccessSummaries.Add(DataAccessGroups[rows[i]]);
                    }
                }

                return dataAccessSummaries;
            }
        }     

        public Web.Common.WebControls.UI.GridView AccessListControl
        {
            get { return GridView1; }
        }

        #endregion Public properties

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void DataBind()
        {
            if (Study != null)
            {
                StudyDataAccessController controller = new StudyDataAccessController();

                DataAccessGroups = Thread.CurrentPrincipal.IsInRole(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess) 
                    ? controller.LoadStudyDataAccess(Study.TheStudyStorage.Key) 
                    : new List<StudyDataAccessSummary>();

                GridView1.DataSource = DataAccessGroups;
            }

            base.DataBind();
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            //This sets the GridView Page Size to the number of series. Needs to be done in the OnInit method,
            //since the page size needs to be set here, and the Study and Partition aren't set until the databind
            //happens in StudyDetailsTabs.
            
            var studyInstanceUID = Request.QueryString[ImageServerConstants.QueryStrings.StudyInstanceUID];
            var serverAE = Request.QueryString[ImageServerConstants.QueryStrings.ServerAE];

            if (!String.IsNullOrEmpty(studyInstanceUID) && !String.IsNullOrEmpty(serverAE)
              && Thread.CurrentPrincipal.IsInRole(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess))
            {
                var adaptor = new ServerPartitionDataAdapter();
                var partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(serverAE);
                IList<ServerPartition> partitions = adaptor.Get(partitionCriteria);
                if (partitions != null && partitions.Count > 0)
                {
                    if (partitions.Count == 1)
                    {
                        var partition = partitions[0];

                        var studyAdaptor = new StudyAdaptor();
                        var studyCriteria = new StudySelectCriteria();
                        studyCriteria.StudyInstanceUid.EqualTo(studyInstanceUID);
                        studyCriteria.ServerPartitionKey.EqualTo(partition.GetKey());
                        var study = studyAdaptor.GetFirst(studyCriteria);

                        if (study != null)
                        {
                            StudyDataAccessController controller = new StudyDataAccessController();
                            DataAccessGroups = controller.LoadStudyDataAccess(study.Key);
                            if (DataAccessGroups.Count > 0)
                                GridView1.PageSize = DataAccessGroups.Count;
                        }
                    }
                }
            }
            else
            {
                GridView1.PageSize = 150;   //Set it to a large number to ensure that all series are displayed if more than 25.
            }         
        }

        #endregion Protected methods

    }
}