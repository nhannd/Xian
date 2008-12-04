using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit
{
    public partial class DeletedStudyDetailsDialog : System.Web.UI.UserControl
    {
        private DeletedStudyInfo _studyInfo;

        public DeletedStudyInfo StudyInfo
        {
            get { return _studyInfo; }
            set { _studyInfo = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void StudyDetailView_DataBound(object sender, EventArgs e)
        {
            
        }

        public void Show()
        {
            List<DeletedStudyInfo> dataList = new List<DeletedStudyInfo>();
            dataList.Add(StudyInfo);
            StudyDetailView.DataSource = dataList;
            StudyDetailView.DataBind();
            ModalDialog.Show();
        }

        protected void OKClicked(object sender, ImageClickEventArgs e)
        {
            ModalDialog.Hide();
        }
    }
}