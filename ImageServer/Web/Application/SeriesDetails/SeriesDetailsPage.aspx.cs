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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.SeriesDetails
{
    public partial class SeriesDetailsPage : System.Web.UI.Page
    {

        private const string QUERY_KEY_SERVER_AE = "serverae";
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERIES_INSTANCE_UID = "seriesuid";

        private string _serverae = null;
        private string _studyInstanceUid = null;
        private string _seriesInstanceUid = null;

        private Model.ServerPartition _partition;
        private Model.Study _study;
        private Model.Series _series;

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Model.Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _studyInstanceUid = Request.QueryString[QUERY_KEY_STUDY_INSTANCE_UID];
            _serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            _seriesInstanceUid = Request.QueryString[QUERY_KEY_SERIES_INSTANCE_UID];

            LoadSeriesDetails();
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            if (_series==null)
            {
                Response.Write("<Br>NO SERIES FOUND<Br>");
            }
        }

        private void LoadSeriesDetails()
        {
            if (!String.IsNullOrEmpty(_serverae) && !String.IsNullOrEmpty(_studyInstanceUid) && !String.IsNullOrEmpty(_seriesInstanceUid))
            {
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                criteria.AeTitle.EqualTo(_serverae);

                IList<Model.ServerPartition> partitions = adaptor.Get(criteria);
                if (partitions != null && partitions.Count > 0)
                {
                    Partition = partitions[0];

                    StudyAdaptor studyAdaptor = new StudyAdaptor();
                    StudySelectCriteria studyCriteria = new StudySelectCriteria();
                    studyCriteria.StudyInstanceUid.EqualTo(_studyInstanceUid);
                    studyCriteria.ServerPartitionKey.EqualTo(Partition.GetKey());
                    IList<Model.Study> studies = studyAdaptor.Get(studyCriteria);

                    if (studies != null && studies.Count > 0)
                    {
                        // there should be only one study
                        _study = studies[0];

                        SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                        SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
                        seriesCriteria.SeriesInstanceUid.EqualTo(_seriesInstanceUid);
                        IList<Model.Series> series = seriesAdaptor.Get(seriesCriteria);

                        if (series != null && series.Count > 0)
                        {
                            _series = series[0];
                        }
                    }

                }
            }

            if (_series != null)
            {
                SeriesDetailsPanel1.Study = Study;
                SeriesDetailsPanel1.Series = Series;
            }
        }

    }
}
