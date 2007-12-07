using System;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchFilterPanel : UserControl
    {
        /// <summary>
        /// Used to store the device filter settings.
        /// </summary>
        public class SearchFilterSettings
        {
            #region private members

            private string _patientName;
            private string _patientId;
            private string _accessionNumber;
            private string _studyDescription;

            #endregion

            #region public properties

            /// <summary>
            /// The Patient Name filter
            /// </summary>
            public string PatientName
            {
                get { return _patientName; }
                set { _patientName = value; }
            }

            /// <summary>
            ///  The Patient Id filter
            /// </summary>
            public string PatientId
            {
                get { return _patientId; }
                set { _patientId = value; }
            }

            /// <summary>
            ///  The Patient Id filter
            /// </summary>
            public string AccessionNumber
            {
                get { return _accessionNumber; }
                set { _accessionNumber = value; }
            }

            public string StudyDescription
            {
                get { return _studyDescription; }
                set { _studyDescription = value; }
            }

            #endregion
        }

        #region public properties

        /// <summary>
        /// Retrieves the current filter settings.
        /// </summary>
        public SearchFilterSettings Filters
        {
            get
            {
                SearchFilterSettings settings = new SearchFilterSettings();
                settings.PatientId = PatientId.Text;
                settings.PatientName = PatientName.Text;
                settings.AccessionNumber = AccessionNumber.Text;
                settings.StudyDescription = StudyDescription.Text;
                return settings;
            }
        }

        #endregion // public properties

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region public members

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = "";
            PatientName.Text = "";
            AccessionNumber.Text = "";
        }

        #endregion

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ApplyFiltersClicked != null)
                ApplyFiltersClicked(Filters);
        }

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="ApplyFiltersClicked"/>
        /// </summary>
        /// <param name="filters">The current settings.</param>
        /// <remarks>
        /// </remarks>
        public delegate void OnApplyFilterSettingsClickedEventHandler(SearchFilterSettings filters);

        /// <summary>
        /// Occurs when the filter settings users click on "Apply" on the filter panel.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event OnApplyFilterSettingsClickedEventHandler ApplyFiltersClicked;

        #endregion // Events
    }
}