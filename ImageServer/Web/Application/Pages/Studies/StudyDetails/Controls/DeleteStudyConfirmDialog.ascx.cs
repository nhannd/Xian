using System;
using System.Collections.Generic;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [Serializable]
    public class DeleteStudyInfo
    {
        private ServerEntityKey _studyKey;
        private string _studyInstanceUid;
        private string _accessionNumber;
        private string _patientId;
        private string _patientsName;
        private string _studyDescription;
        private string _modalities;
        private string _studyDate;

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public string Modalities
        {
            get { return _modalities; }
            set { _modalities = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }
    }

    public class DeleteStudyConfirmDialogStudyDeletingEventArgs : EventArgs
    {
        private IList<DeleteStudyInfo> _deletingStudies;
        private string _reasonForDeletion;

        public IList<DeleteStudyInfo> DeletingStudies
        {
            get { return _deletingStudies; }
            set { _deletingStudies = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public class DeleteStudyConfirmDialogStudyDeletedEventArgs:EventArgs
    {
        private IList<DeleteStudyInfo> _deletedStudies;
        private string _reasonForDeletion;

        public IList<DeleteStudyInfo> DeletedStudies
        {
            get { return _deletedStudies; }
            set { _deletedStudies = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public partial class DeleteStudyConfirmDialog : System.Web.UI.UserControl
    {
        private EventHandler<DeleteStudyConfirmDialogStudyDeletingEventArgs> _studyDeletingHandler;
        private EventHandler<DeleteStudyConfirmDialogStudyDeletedEventArgs> _studyDeletedHandler;

        public event EventHandler<DeleteStudyConfirmDialogStudyDeletingEventArgs> StudyDeleting
        {
            add { _studyDeletingHandler += value; }
            remove { _studyDeletingHandler -= value; }
        }
        
        public event EventHandler<DeleteStudyConfirmDialogStudyDeletedEventArgs> StudyDeleted
        {
            add { _studyDeletedHandler += value; }
            remove { _studyDeletedHandler -= value; }
        }
        
        public IList<DeleteStudyInfo> DeletingStudies
        {
            get
            {
                return ViewState["DeletedStudies"] as IList<DeleteStudyInfo>;
            }
            set { ViewState["DeletedStudies"] = value; }
        }

        public override void DataBind()
        {
            StudyListing.DataSource = DeletingStudies;
            base.DataBind();
        }

        protected void DeleteButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (Page.IsValid)
            {
                try
                {
                    OnDeletingStudies();
                    StudyController controller = new StudyController();
                    foreach (DeleteStudyInfo study in DeletingStudies)
                    {
                        try
                        {
                            controller.DeleteStudy(study.StudyKey, Reason.Text);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex, "DeleteClicked failed: Unable to delete studies");
                            throw;
                        }
                    }

                    OnStudiesDeleted();
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                Show();
            }
        }

        protected void CancelButton_Clicked(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        private void OnStudiesDeleted()
        {
            DeleteStudyConfirmDialogStudyDeletedEventArgs args = new DeleteStudyConfirmDialogStudyDeletedEventArgs();
            args.DeletedStudies = DeletingStudies;
            args.ReasonForDeletion = Reason.Text;
            EventsHelper.Fire(_studyDeletedHandler, this, args);
        }

        private void OnDeletingStudies()
        {
            DeleteStudyConfirmDialogStudyDeletingEventArgs args = new DeleteStudyConfirmDialogStudyDeletingEventArgs();
            args.DeletingStudies = DeletingStudies;
            args.ReasonForDeletion = Reason.Text;
            EventsHelper.Fire(_studyDeletingHandler, this, args);
        }

        public void Show()
        {
            DataBind();
            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        
    }
}