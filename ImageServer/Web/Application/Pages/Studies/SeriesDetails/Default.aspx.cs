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
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    public partial class Default : BasePage
    {
        #region Constants

        private const string QUERY_KEY_SERVER_AE = "serverae";
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERIES_INSTANCE_UID = "seriesuid";

        #endregion Constants

        #region Private mmembers

        private string _serverae = null;
        private string _studyInstanceUid = null;
        private string _seriesInstanceUid = null;

        private ServerPartition _partition;
        private Study _study;
        private Series _series;

        #endregion Private mmembers

        #region Public Properties

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            _studyInstanceUid = Request.QueryString[QUERY_KEY_STUDY_INSTANCE_UID];
            _serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            _seriesInstanceUid = Request.QueryString[QUERY_KEY_SERIES_INSTANCE_UID];

            LoadSeriesDetails();
        }

        #endregion Protected Methods

        #region Public Methods

        protected override void  OnPreRender(EventArgs e)
        {
            if (_series == null)
            {
                Response.Write("<Br>NO  SUCH SERIES FOUND<Br>");

                SeriesDetailsPanel1.Visible = false;
            }
            else
            {
                Page.Title = String.Format("{0}:{1} (Series: {2})", NameFormatter.Format(_study.PatientsName) , _study.PatientId, _series.SeriesNumber);
            }
 	        
            base.OnPreRender(e);
        }

        #endregion Public Methods


        #region Private Methods

        private void LoadSeriesDetails()
        {

            if (!String.IsNullOrEmpty(_serverae) && !String.IsNullOrEmpty(_studyInstanceUid) && !String.IsNullOrEmpty(_seriesInstanceUid))
            {
                StudyAdaptor studyAdaptor = new StudyAdaptor();
                SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                        
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                criteria.AeTitle.EqualTo(_serverae);

                Model.ServerPartition partition = adaptor.GetFirst(criteria);
                if (partition != null)
                {
                    Partition = partition;

                    StudySelectCriteria studyCriteria = new StudySelectCriteria();
                    studyCriteria.StudyInstanceUid.EqualTo(_studyInstanceUid);
                    studyCriteria.ServerPartitionKey.EqualTo(Partition.GetKey());
                    Model.Study study = studyAdaptor.GetFirst(studyCriteria);

                    if (study != null)
                    {
                        // there should be only one study
                        _study = study;

                        SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
                        seriesCriteria.SeriesInstanceUid.EqualTo(_seriesInstanceUid);
                        Series series = seriesAdaptor.GetFirst(seriesCriteria);

                        if (series != null)
                        {
                            _series = series;
                        }
                    }

                }
            }

            if (_study!=null && _series != null)
            {
                SeriesDetailsPanel1.Study = _study;
                SeriesDetailsPanel1.Series = _series;
            }

            DataBind();
        }

        #endregion Private Methods

    }
}
