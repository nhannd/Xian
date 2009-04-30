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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    
    public partial class HistoryPanel : System.Web.UI.UserControl
    {
        private IList<StudyHistory> _historyList; 
        public StudySummary TheStudySummary;
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void DataBind()
        {
            // load history
            LoadHistory();

            StudyHistoryGridView.DataSource = _historyList;
            base.DataBind();
        }

        private void LoadHistory()
        {
            StudyHistoryeAdaptor adaptor = new StudyHistoryeAdaptor();
            StudyHistorySelectCriteria criteria = new StudyHistorySelectCriteria();
            criteria.DestStudyStorageKey.EqualTo(TheStudySummary.TheStudyStorage.GetKey());
            criteria.InsertTime.SortDesc(0);
            _historyList = CollectionUtils.Select(adaptor.Get(criteria),
                        delegate(StudyHistory history)
                            {
                                // only include reconciliation records that result in updating the current study
                                if (history.StudyHistoryTypeEnum==StudyHistoryTypeEnum.StudyReconciled)
                                {
                                    ReconcileHistoryRecord desc = StudyHistoryRecordDecoder.ReadReconcileRecord(history);
                                    switch(desc.UpdateDescription.Action)
                                    {
                                        case StudyReconcileAction.CreateNewStudy:
                                        case StudyReconcileAction.Merge:
                                        case StudyReconcileAction.ProcessAsIs:
                                            return true;
                                    }
                                    return false;
                                }
                                return true;
                            });

        }

        protected void StudyHistoryGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            
        }

        protected void StudyHistoryGridView_PageIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        protected void StudyHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                StudyHistory item = e.Row.DataItem as StudyHistory;
                
                StudyHistoryChangeDescPanel panel = e.Row.FindControl("StudyHistoryChangeDescPanel") as StudyHistoryChangeDescPanel;
                panel.HistoryRecord = item;
                panel.DataBind();
            }
        }
    }
}