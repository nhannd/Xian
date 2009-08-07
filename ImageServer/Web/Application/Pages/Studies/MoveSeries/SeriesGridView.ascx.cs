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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    public partial class SeriesGridView : System.Web.UI.UserControl
    {
        private IList<Series> _seriesList = new List<Series>();
        private ServerPartition _partition;
        private Study _study;

        public IList<Series> SeriesList
        {
            get { return _seriesList; }
            set { _seriesList = value;}
        }

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


        protected void Page_Load(object sender, EventArgs e)
        {
            SeriesListControl.DataSource = _seriesList;
            SeriesListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in SeriesListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = SeriesListControl.PageIndex * SeriesListControl.PageSize + row.RowIndex;
                    Series series = SeriesList[index];

                    if (series != null)
                    {

                        row.Attributes.Add("instanceuid", series.SeriesInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        row.Attributes.Add("deleted", "true");
                    }
                }
            }
        }
    }
}