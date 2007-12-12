using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class StudyDetails : System.Web.UI.UserControl
    {
        private Study _study;
        private IList<Series> _seriesList;
        private readonly SearchController _searchController = new SearchController();

        public Study Study
        {
            get { return _study; }
            set
            {
                _study = value;
                SeriesList = _searchController.GetSeries(value);
            }
        }

        public IList<Series> SeriesList
        {
            get { return _seriesList; }
            set { _seriesList = value; }
        }

        protected void AddRow(string name, string val)
        {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Text = name;
            row.Cells.Add(cell);
            cell = new TableCell();
            if (val == null)
                cell.Text = " ";
            else
                cell.Text = val;
            row.Cells.Add(cell);
            this.DetailsTable.Rows.Add(row);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddRow("Patient Name", _study.PatientName);
            AddRow("Patient ID", _study.PatientId);
            AddRow("Patient's Birth Date", _study.PatientsBirthDate);
            AddRow("Patient's Sex", _study.PatientsSex);
            AddRow("Referring Physician's Name", _study.ReferringPhysiciansName);
            AddRow("Study Date", _study.StudyDate);
            AddRow("Study Time", _study.StudyTime);
            AddRow("Study ID", _study.StudyId);
            AddRow("Study Description", _study.StudyDescription);
            AddRow("Study Instance UID", _study.StudyInstanceUid);
            AddRow("Accession Number", _study.AccessionNumber);
            AddRow("Number of Study Related Series", _study.NumberOfStudyRelatedSeries.ToString());
            AddRow("Number of Study Related Instances", _study.NumberOfStudyRelatedInstances.ToString());

            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.ColumnSpan = 2;


            foreach (Series series in SeriesList)
            {
                SeriesDetails seriesDetails = LoadControl("SeriesDetails.ascx") as SeriesDetails;
                seriesDetails.Series = series;
                cell.Controls.Add(seriesDetails);
            }

            row.Cells.Add(cell);
            this.DetailsTable.Rows.Add(row);
        }
    }
}