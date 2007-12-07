using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchGridView : System.Web.UI.UserControl
    {
        private IList<Study> _studies = new List<Study>();
        private Study _selectedStudy;

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView TheGrid
        {
            get { return GridView1; }
        }

        public Study SelectedStudy
        {
            get { return _selectedStudy; }
            set { _selectedStudy = value; }
        }

        public IList<Study> Studies
        {
            get { return _studies; }
            set
            {
                _studies = value;
                GridView1.DataSource = _studies; // must manually call DataBind() later
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            TheGrid.PagerSettings.Visible = false;
            TheGrid.SelectedIndexChanged += GridView1_SelectedIndexChanged;
        }

        protected void ImageButton_Command(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = TheGrid.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "first":
                    TheGrid.PageIndex = 0;
                    break;
                case "prev":
                    TheGrid.PageIndex = intCurIndex - 1;
                    break;
                case "next":
                    TheGrid.PageIndex = intCurIndex + 1;
                    break;
                case "last":
                    TheGrid.PageIndex = GridView1.PageCount;
                    break;
            }
            //DataBind();
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Study study = SelectedStudy;
            if (study != null)
                if (OnStudySelectionChanged != null)
                    OnStudySelectionChanged(this, study);
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            UpdatePager();
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            //DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //TheGrid.PageIndex = e.NewPageIndex;
            //DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    e.Row.Attributes["OnClick"] =
                        Page.ClientScript.GetPostBackEventReference(GridView1, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    // For some reason, double-click won't work if single-click is used
                    // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                    Study study = e.Row.DataItem as Study;
                    if (study != null)
                        e.Row.ToolTip =
                            String.Format(
                                "Patient Name: {0}\nPatient ID: {1}\nPatients Birth Date: {2}\nPatients Sex: {3}\nAccession Number: {4}\nReferring Physician's Name: {5}\nStudy ID: {6}\nStudy Description: {7}\nStudy Date: {8}\nStudy Time: {9}\nStudy Instance UID: {10}\nNumber of Study Related Series: {11}\nNumber of Study Related Instances: {12}",
                                study.PatientName, study.PatientId, study.PatientsBirthDate,
                                study.PatientsSex, study.AccessionNumber, study.ReferringPhysiciansName,
                                study.StudyId, study.StudyDescription, study.StudyDate, study.StudyTime,
                                study.StudyInstanceUid, study.NumberOfStudyRelatedSeries,
                                study.NumberOfStudyRelatedInstances);
                }
            }
        }

        /// <summary>
        /// Updates the grid pager based on the current list.
        /// </summary>
        protected void UpdatePager()
        {
            #region update pager of the gridview if it is used

            if (GridView1.BottomPagerRow != null)
            {
                // Show Number of studies in the list
                Label lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerStudyCountLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("{0} studies", this.Studies.Count);

                // Show current page and the number of pages for the list
                lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPagingLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("Page {0} of {1}", GridView1.PageIndex + 1, GridView1.PageCount);

                // Enable/Disable the "Prev" page button
                ImageButton btn =
                    GridView1.BottomPagerRow.Cells[0].FindControl("PagerPrevImageButton") as ImageButton;
                if (btn != null)
                {
                    if (this.Studies.Count == 0 || GridView1.PageIndex == 0)
                    {
                        btn.ImageUrl = "~/images/prev_disabled.gif";
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.ImageUrl = "~/images/prev.gif";
                        btn.Enabled = true;
                    }

                    btn.Style.Add("cursor", "hand");
                }

                // Enable/Disable the "Next" page button
                btn = GridView1.BottomPagerRow.Cells[0].FindControl("PagerNextImageButton") as ImageButton;
                if (btn != null)
                {
                    if (this.Studies.Count == 0 || GridView1.PageIndex == GridView1.PageCount - 1)
                    {
                        btn.ImageUrl = "~/images/next_disabled.gif";
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.ImageUrl = "~/images/next.gif";
                        btn.Enabled = true;
                    }

                    btn.Style.Add("cursor", "hand");
                }
            }

            #endregion
        }

        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="Studies"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            TheGrid.DataBind();

            TheGrid.PagerSettings.Visible = false;
        }

        #region Events

        /// <summary>
        /// Defines the handler for <seealso cref="OnStudySelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedStudy"></param>
        public delegate void StudySelectedEventHandler(object sender, Study selectedStudy);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event StudySelectedEventHandler OnStudySelectionChanged;

        #endregion // Events
    }
}