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
using System.Web.UI.WebControls;
using System.Collections.Generic;

using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Search
{
    //
    //  Used to display the list of studies.
    //
    public partial class StudyListGridView : System.Web.UI.UserControl
    {
        #region private members
        // server partitions lookup table based on server key
        private Dictionary<string, ServerPartition> _DictionaryPartitions = new Dictionary<string, ServerPartition>();
        // list of devices to display
        private IList<Model.Study> _studies;
        private ServerPartition _partition;
        private StudyController _controller = new StudyController();

        private Unit _height;
        #endregion Private members

        #region protected properties

        protected Dictionary<string, ServerPartition> DictionaryPartitions
        {
            get { return _DictionaryPartitions; }
            set { _DictionaryPartitions = value; }
        }

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView StudyListGrid
        {
            get { return this.StudyListControl; }
        }


        public ServerPartition Partition
        {
            set { _partition = value; }
            get { return _partition; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<Study> SelectedStudies
        {
            get
            {
                if (Studies==null || Studies.Count == 0)
                    return null;

                int[] rows = StudyListControl.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<Study> studies = new List<Study>();
                for(int i=0; i<rows.Length; i++)
                {
                    // SelectedIndex is for the current page. Must convert to the index of the entire list
                    int index = StudyListControl.PageIndex * StudyListControl.PageSize + rows[i];
                    if (index >=0 && index<Studies.Count)
                        studies.Add(Studies[index]);
                }

                return studies;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<Study> Studies
        {
            get
            {
                return _studies;
            }
            set
            {
                _studies = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                else
                    return _height; 
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Defines the handler for <seealso cref="OnStudySelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedStudies"></param>
        public delegate void StudySelectedEventHandler(object sender, IList<Model.Study> selectedStudies);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event StudySelectedEventHandler OnStudySelectionChanged;

        #endregion // Events

        
        #region protected methods


        protected void Page_Load(object sender, EventArgs e)
        {

            StudyListControl.DataBind();
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            StudyListControl.SelectedIndexChanged += new EventHandler(StudyListControl_SelectedIndexChanged);

        }

         /// <summary>
        /// Updates the grid pager based on the current list.
        /// </summary>
        protected void UpdatePager()
        {
            #region update pager of the gridview if it is used
            if (StudyListControl.BottomPagerRow != null) 
            {
                // Show Number of devices in the list
                Label lbl = StudyListControl.BottomPagerRow.Cells[0].FindControl("PagerDeviceCountLabel") as Label;
                if (lbl != null)
                {
                    if (Studies.Count > 1)
                        lbl.Text = string.Format("{0} studies", this.Studies.Count);
                    else
                        lbl.Text = string.Format("{0} study", this.Studies.Count);
                }

                // Show current page and the number of pages for the list
                lbl = StudyListControl.BottomPagerRow.Cells[0].FindControl("PagerPagingLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("Page {0} of {1}", StudyListControl.PageIndex + 1, StudyListControl.PageCount);

                // Enable/Disable the "Prev" page button
                ImageButton btn = StudyListControl.BottomPagerRow.Cells[0].FindControl("PagerPrevImageButton") as ImageButton;
                if (btn != null)
                {
                    if (this.Studies.Count == 0 || StudyListControl.PageIndex == 0)
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
                btn = StudyListControl.BottomPagerRow.Cells[0].FindControl("PagerNextImageButton") as ImageButton;
                if (btn != null)
                {
                    if (this.Studies.Count == 0 || StudyListControl.PageIndex == StudyListControl.PageCount - 1)
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


        protected void StudyListControl_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            StudySummary studySummary = e.Row.DataItem as StudySummary;
            if (studySummary!=null)
            {
                Label statusLabel = e.Row.FindControl("Status") as Label;
                if (statusLabel!=null)
                {
                    statusLabel.Text = studySummary.StudyStatusEnum.Description;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType==DataControlRowType.DataRow)
                {
                    int index = StudyListControl.PageIndex * StudyListControl.PageSize + row.RowIndex;
                    Study study = Studies[index];
                    
                    if (study!=null)
                    {

                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        bool deleted = controller.IsScheduledForDelete(study);
                        if (deleted)
                            row.Attributes.Add("deleted", "true");
                   
                    }
 
                }
                    
            }
        }


        protected void StudyListControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<Study> studies = SelectedStudies;
            if (studies != null)
                if (OnStudySelectionChanged != null)
                    OnStudySelectionChanged(this, studies);
            
        }

        protected void StudyListControl_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            
        }

        protected void StudyListControl_DataBound(object sender, EventArgs e)
        {
            UpdatePager();          
        }

        protected void StudyListControl_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void StudyListControl_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyListControl.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void ImageButton_Command(object sender, CommandEventArgs e)
        {

            // get the current page selected
            int intCurIndex = StudyListControl.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "first":
                    StudyListControl.PageIndex = 0;
                    break;
                case "prev":
                    StudyListControl.PageIndex = intCurIndex - 1;
                    break;
                case "next":
                    StudyListControl.PageIndex = intCurIndex + 1;
                    break;
                case "last":
                    StudyListControl.PageIndex = StudyListControl.PageCount;
                    break;
            }

            DataBind();
        }
        
        #endregion

        
        #region public methods
        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="Study"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            IList<StudySummary> studySummaries = new List<StudySummary>();
            foreach(Study study in Studies)
            {
                studySummaries.Add(StudySummaryAssembler.CreateStudySummary(study));
            }

            StudyListControl.DataSource = studySummaries;
            StudyListControl.DataBind();
            StudyListControl.PagerSettings.Visible = false;

        }

        #endregion // public methods

    }

}
