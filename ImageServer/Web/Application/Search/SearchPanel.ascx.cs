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
using System.Web.UI;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchPanel : System.Web.UI.UserControl
    {
        private ServerPartition _serverPartition;
        private StudyController _searchController;
        private SearchPage _enclosingPage;

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        public SearchPage EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = "";
            PatientName.Text = "";
            AccessionNumber.Text = "";
            StudyDescription.Text = "";
        }

        public void LoadStudies()
        {
            StudySelectCriteria criteria = new StudySelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());

            if (!String.IsNullOrEmpty(PatientId.Text))
            {
                string key = PatientId.Text + "%";
                key = key.Replace("*", "%");
                criteria.PatientId.Like(key);
            }
            if (!String.IsNullOrEmpty(PatientName.Text))
            {
                string key = PatientName.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.PatientName.Like(key);
            }
            criteria.PatientName.SortAsc(0);

            if (!String.IsNullOrEmpty(AccessionNumber.Text))
            {
                string key = AccessionNumber.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.AccessionNumber.Like(key);
            }
            if (!String.IsNullOrEmpty(StudyDescription.Text))
            {
                string key = StudyDescription.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.StudyDescription.Like(key);
            }

            SearchAccordianControl.Studies = _searchController.GetStudies(criteria);
            SearchAccordianControl.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _searchController = new StudyController();
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SearchAccordianControl.IsPostBack)
                LoadStudies();
        }

        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            // reload the data
            LoadStudies();
            SearchAccordianControl.PageIndex = 0;
        }
    }
}