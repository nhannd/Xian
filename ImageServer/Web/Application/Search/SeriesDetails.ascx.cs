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
            AddRow("Modality", _series.Modality);
            AddRow("Series Description", _series.SeriesDescription);
            AddRow("Series Instance Uid", _series.SeriesInstanceUid);
            AddRow("Series Number", _series.SeriesNumber);
            AddRow("Performed Procedure Step Start Date", _series.PerformedProcedureStepStartDate);
            AddRow("Performed Procedure Step Start Time", _series.PerformedProcedureStepStartTime);
            AddRow("Number of Series Related Instances", _series.NumberOfSeriesRelatedInstances.ToString());
            AddRow("Source AE Title", _series.SourceAeTitle);
        }    
    }
}