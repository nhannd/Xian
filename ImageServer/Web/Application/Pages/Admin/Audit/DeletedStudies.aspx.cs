using System;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit
{
    public partial class DeletedStudies : BaseAdminPage
    {

        protected override void OnInit(EventArgs e)
        {
            SearchPanel.ViewDetailsClicked += new EventHandler<DeletedStudyViewDetailsClickedEventArgs>(SearchPanel_ViewDetailsClicked);
            base.OnInit(e);
        }

        void SearchPanel_ViewDetailsClicked(object sender, DeletedStudyViewDetailsClickedEventArgs e)
        {
            DetailsDialogd.StudyInfo = e.DeletedStudyInfo;
            DetailsDialogd.Show();
            UpdatePanel1.Update();
        }
        
        protected override void OnLoad(EventArgs e)
        {
           
            base.OnLoad(e);
            DataBind();
        }

    }
}
