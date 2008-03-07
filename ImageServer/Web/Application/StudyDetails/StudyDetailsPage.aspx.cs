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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    /// <summary>
    /// Study details page.
    /// </summary>
    public partial class StudyDetailsPage : BasePage
    {
        #region constants
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "siuid";
        private const string QUERY_KEY_SERVER_AE = "serverae";
        #endregion constants

        #region Private members
        private string _studyInstanceUid = null;
        private string _serverae = null;
        private Model.ServerPartition _partition = null;
        private Model.Study _study = null;
        
        #endregion Private members

        #region Public properties

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        #endregion Public properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            _studyInstanceUid = Request.QueryString[QUERY_KEY_STUDY_INSTANCE_UID];
            _serverae = Request.QueryString[QUERY_KEY_SERVER_AE];

            if (!String.IsNullOrEmpty(_studyInstanceUid) && !String.IsNullOrEmpty(_serverae)) 
            {
                
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                criteria.AeTitle.EqualTo(Request.QueryString[QUERY_KEY_SERVER_AE]);
                IList<Model.ServerPartition> partitions = adaptor.Get(criteria);
                if (partitions != null && partitions.Count>0)
                {
                    Platform.CheckArgumentRange(partitions.Count, 1, 1, "Number of partitions with matching AE Title");
                    
                    Partition = partitions[0];
                    LoadStudy();
                }
            }
        }

        protected void LoadStudy()
        {
            if (String.IsNullOrEmpty(_studyInstanceUid))
                return;

            if (_partition == null)
                return;

            StudyAdaptor studyAdaptor = new StudyAdaptor();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.StudyInstanceUid.EqualTo(_studyInstanceUid);
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());
            IList<Model.Study> studies =  studyAdaptor.Get(criteria);

            if (studies!=null && studies.Count>0)
            {
                Platform.CheckArgumentRange(studies.Count, 1, 1, "Number of study with matching study instance uid on the server");
                
                // there should be only one study
                _study  = studies[0];
            }


            StudyDetailsPanel.Study = _study;
            StudyDetailsPanel.DataBind();
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e); 
            
            if (_study == null)
            {
                Response.Write("<Br>NO SUCH STUDY FOUND<Br>");
                StudyDetailsPanel.Visible = false;
            } 
        }

        #endregion Protected Methods


    }
}
