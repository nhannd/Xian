
using System.Web.UI;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    /// <summary>
    /// View model for <see cref="DeletedStudyDetailsDialog"/>
    /// </summary>
    internal class DeletedStudyDetailsDialogViewModel
    {
        private DeletedStudyInfo _deletedStudyRecord;
        
        public DeletedStudyInfo DeletedStudyRecord
        {
            get { return _deletedStudyRecord;  }
            set { _deletedStudyRecord = value; }
        }
    }

    public partial class DeletedStudyDetailsDialog : System.Web.UI.UserControl
    {
        #region Private Fields
        private DeletedStudyDetailsDialogViewModel viewModel;
        #endregion

        #region Internal Properties
        internal DeletedStudyDetailsDialogViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }
        #endregion

        #region Public Methods
        public void Show()
        {
            DialogContent.ViewModel = this.ViewModel;
            DataBind();
            ModalDialog.Show();
        }
        #endregion

        #region Protected Methods
        protected void CloseClicked(object sender, ImageClickEventArgs e)
        {
            ModalDialog.Hide();
        }

        #endregion
    }
}