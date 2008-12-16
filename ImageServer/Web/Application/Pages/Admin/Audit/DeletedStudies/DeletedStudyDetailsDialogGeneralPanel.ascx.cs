
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class DeletedStudyDetailsDialogGeneralPanel : System.Web.UI.UserControl
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

        public override void DataBind()
        {
            IList<DeletedStudyInfo> dataSource = new List<DeletedStudyInfo>();
            if (viewModel!=null)
                dataSource.Add(viewModel.DeletedStudyRecord);
            StudyDetailView.DataSource = dataSource;
            base.DataBind();
        }

        #endregion
    }
}