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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SeriesDetails : System.Web.UI.UserControl
    {
        private Series _series;

        public Series Series
        {
            get { return _series; }
            set { _series = value; }
        }
 
        protected void AddRow(string name, string val)
        {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Text = name;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = val;
            row.Cells.Add(cell);
            this.DetailsTable.Rows.Add(row);           
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            this.SeriesLabel.Text = String.Format("Series: {0}", _series.SeriesDescription);
            AddRow("Modality", _series.Modality);
            AddRow("Series Description", _series.SeriesDescription);
            AddRow("Series Instance Uid", _series.SeriesInstanceUid);

            //AddRow("Series Instance Uid", "2222222222222222222222222222222222222222222222");
            //AddRow("Series Instance Uid", "123456789012345678901234567890123456789012345");

            AddRow("Series Number", _series.SeriesNumber);
            AddRow("Performed Procedure Step Start Date", _series.PerformedProcedureStepStartDate);
            AddRow("Performed Procedure Step Start Time", _series.PerformedProcedureStepStartTime);
            AddRow("Number of Series Related Instances", _series.NumberOfSeriesRelatedInstances.ToString());
            AddRow("Source AE Title", _series.SourceApplicationEntityTitle);
        }    
    }
}