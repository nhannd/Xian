#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

            String status = _workQueueDetails.Study == null ? "N/A" : _workQueueDetails.Study.Status;

            if (_workQueueDetails.Study != null && _workQueueDetails.Study.Lock != null)
            {
                if (_workQueueDetails.Study.Lock.Value)
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