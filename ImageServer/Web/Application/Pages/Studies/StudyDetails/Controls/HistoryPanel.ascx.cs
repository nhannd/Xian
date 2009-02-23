using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;

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
            criteria.StudyStorageKey.EqualTo(TheStudySummary.TheStudyStorage.GetKey());
            _historyList = CollectionUtils.Select<StudyHistory>(adaptor.Get(criteria),
                        delegate(StudyHistory history)
                            {
                                // only include reconciliation records that result in updating the current study
                                if (history.StudyHistoryTypeEnum==StudyHistoryTypeEnum.StudyReconciled)
                                {
                                    ReconcileHistoryRecord desc = StudyHistoryRecordDecoder.ReadReconcileRecord(history);
                                    return desc.UpdateDescription.ReconcileAction == ReconcileAction.Merge;
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