using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchAccordian : System.Web.UI.UserControl
    {
        private IList<Study> _studies = new List<Study>();
        private Study _selectedStudy;
        private int _pageCount;
        private int _pageIndex = 0;
        private int _pageSize = 15;

        public int PageSize
        {
            get { return _pageSize; }
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
                PageCount = _studies.Count/PageSize + 1;
            }
        }

        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; }
        }

        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                this.ViewState.Add("PageIndex", value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                if (ViewState["PageIndex"] != null)
                {
                    PageIndex = (int) ViewState["PageIndex"];
                    if (PageIndex > PageCount)
                        PageIndex = PageCount;
                }
            }
            UpdatePager();
        }

        public override void DataBind()
        {
            base.DataBind();

            this.MainAccordian.Panes.Clear();
            this.MainAccordian.Visible = true;
            this.MainAccordian.SuppressHeaderPostbacks = true;

            for (int i = PageIndex*PageSize; (i < (PageIndex + 1)*PageSize) && i < Studies.Count; i++)
            {
                AccordionPane pane = new AccordionPane();

                StudySummary studySummary = LoadControl("StudySummary.ascx") as StudySummary;
                StudyDetails studyDetails = LoadControl("StudyDetails.ascx") as StudyDetails;

                studySummary.Study = Studies[i];
                studyDetails.Study = Studies[i];

                pane.ContentContainer.Controls.Add(studyDetails);
                pane.HeaderContainer.Controls.Add(studySummary);

                pane.HeaderCssClass = "accordionHeader";

                pane.ContentCssClass = "accordionContent";
                pane.BorderWidth = new Unit(0d);
                pane.Visible = true;

                this.MainAccordian.Panes.Add(pane);
            }

            this.MainAccordian.RequireOpenedPane = false;
            this.MainAccordian.SelectedIndex = -1;
        }

        protected void PageButtonClick(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString().ToLower())
            {
                case "first":
                    PageIndex = 0;
                    break;
                case "prev":
                    if (PageIndex > 0)
                        PageIndex = PageIndex - 1;
                    break;
                case "next":
                    if (PageIndex < PageCount)
                        PageIndex = PageIndex + 1;
                    break;
                case "last":
                    PageIndex = PageCount;
                    break;
            }

            UpdatePager();
            DataBind();
        }

        /// <summary>
        /// Updates the grid pager based on the current list.
        /// </summary>
        protected void UpdatePager()
        {
            #region update pager of the gridview if it is used

            // Show Number of studies in the list
            this.PagerStudyCountLabel.Text = string.Format("{0} studies", this.Studies.Count);

            // Show current page and the number of pages for the list
            this.PagerPagingLabel.Text = string.Format("Page {0} of {1}", PageCount == 0 ? 0 : PageIndex + 1, PageCount);

            // Enable/Disable the "Prev" page button
            ImageButton btn = this.PrevPageButton;
            if (btn != null)
            {
                if (this.Studies.Count == 0 || PageIndex == 0)
                {
                    btn.ImageUrl = "~/images/icons/BackDisabled.png";
                    btn.Enabled = false;
                }
                else
                {
                    btn.ImageUrl = "~/images/icons/BackEnabled.png";
                    btn.Enabled = true;
                }

                btn.Style.Add("cursor", "hand");
            }

            // Enable/Disable the "Next" page button
            btn = this.NextPageButton;
            if (btn != null)
            {
                if (this.Studies.Count == 0 || PageIndex == PageCount - 1)
                {
                    btn.ImageUrl = "~/images/icons/ForwardDisabled.png";
                    btn.Enabled = false;
                }
                else
                {
                    btn.ImageUrl = "~/images/icons/ForwardEnabled.png";
                    btn.Enabled = true;
                }

                btn.Style.Add("cursor", "hand");
            }

            #endregion
        }
    }
}