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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move
{
    public partial class StudyGridView : System.Web.UI.UserControl
    {
        private IList<Study> _studyList = new List<Study>();
        private ServerPartition _partition;

        public IList<Study> StudyList
        {
            get { return _studyList; }
            set { _studyList = value;
                  StudyListControl.DataSource = _studyList;
            }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            StudyListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = StudyListControl.PageIndex * StudyListControl.PageSize + row.RowIndex;
                    Study study = StudyList[index];

                    if (study != null)
                    {

                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        bool deleted = controller.IsScheduledForDelete(study);
                        if (deleted)
                            row.Attributes.Add("deleted", "true");
                    }
                }
            }
        }
    }
}