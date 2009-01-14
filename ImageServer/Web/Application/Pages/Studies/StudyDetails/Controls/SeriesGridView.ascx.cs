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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.SeriesGridView.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Series list panel within the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesGridView",
                           ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.SeriesGridView.js")]
    public partial class SeriesGridView : ScriptUserControl
    {
        #region Private members

        private ServerPartition _serverPartition = null;
        private StudySummary _study = null;
        private IList<Series> _series;
        #endregion Private members

        #region Public properties
        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public ServerPartition Partition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public StudySummary Study
        {
            get { return _study; }
            set { _study = value; }
        }

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        protected IList<Series> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SeriesListClientID")]
        public string SeriesListClientID
        {
            get { return GridView1.ClientID; }
        }   

        [ExtenderControlProperty]
        [ClientPropertyName("OpenSeriesPageUrl")]
        public string OpenSeriesPageUrl
        {
            get { return  Page.ResolveClientUrl(ImageServerConstants.PageURLs.SeriesDetailsPage); }
        }      

        public Web.Common.WebControls.UI.GridView SeriesListControl
        {
            get { return GridView1; }
        }

         

        #endregion Public properties

        #region Constructors

        public SeriesGridView()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

            
        #region Protected methods

        public override void DataBind()
        {
            if (Study != null && Partition != null)
            {
                SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                SeriesSelectCriteria criteria = new SeriesSelectCriteria();
                criteria.StudyKey.EqualTo(Study.TheStudy.GetKey());
                criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

                Series = seriesAdaptor.Get(criteria);

                GridView1.DataSource = Series;
            }

            base.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach(GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = GridView1.PageIndex * GridView1.PageSize + row.RowIndex;
                    Series series = _series[index];

                    row.Attributes["serverae"] = _serverPartition.AeTitle;
                    row.Attributes["studyuid"] = _study.StudyInstanceUid;
                    row.Attributes["seriesuid"] = series.SeriesInstanceUid;
                }
            }

            
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

        #endregion Protected methods

    }
}