using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
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
        private string _serverPartitionAE;

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

        public string ServerPartitionAE
        {
            get { return _serverPartitionAE; }
            set { _serverPartitionAE = value; }
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
        private const string REASON_CANNEDTEXT_CATEGORY = "DeleteStudyReason";
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

            if (ReasonListBox.Items.Count==0)
            {
                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                using (IReadContext context = store.OpenReadContext())
                {
                    ICannedTextEntityBroker broker = context.GetBroker<ICannedTextEntityBroker>();
                    CannedTextSelectCriteria criteria = new CannedTextSelectCriteria();
                    criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                    IList<CannedText> list = broker.Find(criteria);
                    ReasonListBox.Items.Add(new ListItem(" - Select one -", ""));
                    foreach (CannedText text in list)
                    {
                        ReasonListBox.Items.Add(new ListItem(text.Name, text.Text));
                    }
                    ReasonListBox.Items.Add(new ListItem("Other (Specify)", "Enter the reason here"));
                } 
            }
            
            base.DataBind();
        }

        protected void DeleteButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (Page.IsValid)
            {
                try
                {
                    if (!String.IsNullOrEmpty(SaveReasonAsName.Text))
                    {
                        SaveCustomReason();
                    }

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

        private void SaveCustomReason()
        {
            if (ReasonListBox.Items.FindByText(SaveReasonAsName.Text)!=null)
            {
                // update
                StudyDeleteReasonAdaptor adaptor = new StudyDeleteReasonAdaptor();
                CannedTextSelectCriteria criteria = new CannedTextSelectCriteria();
                criteria.Name.EqualTo(SaveReasonAsName.Text);
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> reasons = adaptor.Get(criteria);
                foreach(CannedText reason in reasons)
                {
                    CannedTextUpdateColumns rowColumns = new CannedTextUpdateColumns();
                    rowColumns.Text = Reason.Text;
                    adaptor.Update(reason.Key, rowColumns);
                }
                
            }
            else
            {
                // add 
                StudyDeleteReasonAdaptor adaptor = new StudyDeleteReasonAdaptor();
                CannedTextUpdateColumns rowColumns = new CannedTextUpdateColumns();
                rowColumns.Category = REASON_CANNEDTEXT_CATEGORY;
                rowColumns.Name = SaveReasonAsName.Text;
                rowColumns.Text = Reason.Text;
                adaptor.Add(rowColumns);
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