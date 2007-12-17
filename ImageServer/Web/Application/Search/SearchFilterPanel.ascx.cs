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