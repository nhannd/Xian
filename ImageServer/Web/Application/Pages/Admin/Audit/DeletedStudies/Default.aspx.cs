using System;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class Default : BaseAdminPage
    {
        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            SearchPanel.ViewDetailsClicked += new EventHandler<DeletedStudyViewDetailsClickedEventArgs>(SearchPanel_ViewDetailsClicked);
            SearchPanel.DeleteClicked += new EventHandler<DeletedStudyDeleteClickedEventArgs>(SearchPanel_DeleteClicked);
            DeleteConfirmMessageBox.Confirmed += new ClearCanvas.ImageServer.Web.Application.Controls.MessageBox.ConfirmedEventHandler(DeleteConfirmMessageBox_Confirmed);
            base.OnInit(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.Title = App_GlobalResources.Titles.DeletedStudiesPageTitle;

            DataBind();
        }

        protected void Refresh()
        {
            DataBind();
            UpdatePanel1.Update();
        }
        #endregion

        #region Private Methods
        void DeleteConfirmMessageBox_Confirmed(object data)
        {
            try
            {
                ServerEntityKey record = data as ServerEntityKey;
                DeletedStudyController controller = new DeletedStudyController();
                controller.Delete(record);
            }
            finally
            {
                SearchPanel.Refresh();
            }
        }

        void SearchPanel_DeleteClicked(object sender, DeletedStudyDeleteClickedEventArgs e)
        {
            DeleteConfirmMessageBox.Data = e.SelectedItem.DeleteStudyRecord;
            DeleteConfirmMessageBox.Show();
        }

        void SearchPanel_ViewDetailsClicked(object sender, DeletedStudyViewDetailsClickedEventArgs e)
        {
            DeletedStudyDetailsDialogViewModel dialogViewModel = new DeletedStudyDetailsDialogViewModel();
            dialogViewModel.DeletedStudyRecord = e.DeletedStudyInfo;
            DetailsDialog.ViewModel = dialogViewModel;
            DetailsDialog.Show();
        }

        #endregion

    }
}