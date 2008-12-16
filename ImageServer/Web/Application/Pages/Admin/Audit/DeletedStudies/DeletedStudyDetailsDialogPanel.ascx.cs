using ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class DeletedStudyDetailsDialogPanel : System.Web.UI.UserControl
    {
        private DeletedStudyDetailsDialogViewModel viewModel;

        internal DeletedStudyDetailsDialogViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }

        public override void DataBind()
        {
            GeneralInfoPanel.ViewModel = this.ViewModel;
            ArchiveInfoPanel.ViewModel = this.ViewModel;
            
            base.DataBind();
        }
    }
}