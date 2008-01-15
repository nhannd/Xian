using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    /// <summary>
    /// WorkQueue Details Panel 
    /// </summary>
    public partial class WorkQueueDetailsPanel : UserControl
    {
        #region Private members

        private WorkQueueDetails _workQueueDetails;

        #endregion Private members

        #region public properties

        public WorkQueueDetails WorkQueueDetails
        {
            get { return _workQueueDetails; }
            set { _workQueueDetails = value; }
        }

        #endregion public properties

        #region Protected Methods

        protected void AddRow(string name, string val)
        {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Text = name;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Style.Add(HtmlTextWriterStyle.PaddingLeft, "5px");
            if (val == null)
                cell.Text = " ";
            else
                cell.Text = val;
            row.Cells.Add(cell);
            DetailsTable.Rows.Add(row);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddRow("Patient ID", _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.PatientID);
            AddRow("Patient Name", _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.PatientName);
            AddRow("Accession Number", _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.AccessionNumber);
            AddRow("Study Description",
                   _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.StudyDescription);
            AddRow("Study Date", _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.AccessionNumber);
            AddRow("Study Instance UID",
                   _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.StudyInstanceUID);

            String status = _workQueueDetails.Study==null? "N/A":_workQueueDetails.Study.Status;
                
            if (_workQueueDetails.Study!=null && _workQueueDetails.Study.Lock != null)
            {
                if (_workQueueDetails.Study.Lock.Value == true)
                    status = status + " / Locked";
            }

            AddRow("Study Status ", status);
            

            AddRow("Series Pending", _workQueueDetails.NumSeriesPending.ToString());
            AddRow("Instances Pending", _workQueueDetails.NumInstancesPending.ToString());
            AddRow("Expiration Time", _workQueueDetails.ExpirationTime.ToString());
            AddRow("Retry Count", _workQueueDetails.FailureCount.ToString());
            AddRow("Processor ID", _workQueueDetails.ProcessorID);
        }

        #endregion Protected Methods
    }
}