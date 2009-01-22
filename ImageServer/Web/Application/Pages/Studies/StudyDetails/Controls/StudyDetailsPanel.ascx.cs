#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public class StudyDetailsPanelDeleteStudyClickEventArgs : EventArgs
    {}

    public class StudyDetailsPanelEditStudyClickEventArgs : EventArgs
    { }

    /// <summary>
    /// Main panel within the <see cref="Default"/>
    /// </summary>
    public partial class StudyDetailsPanel : UserControl
    {
        #region Private Members
        private StudySummary _study;
        private EventHandler<StudyDetailsPanelDeleteStudyClickEventArgs> _deleteStudyClickedHandler;
        private EventHandler<StudyDetailsPanelEditStudyClickEventArgs> _editStudyClickedHandler;
        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public StudySummary Study
        {
            get { return _study; }
            set { _study = value; }
        }

        #endregion Public Properties

        #region Events
        public event EventHandler<StudyDetailsPanelDeleteStudyClickEventArgs> DeleteStudyClicked
        {
            add { _deleteStudyClickedHandler += value; }
            remove { _deleteStudyClickedHandler -= value; }
        }
        public event EventHandler<StudyDetailsPanelEditStudyClickEventArgs> EditStudyClicked
        {
            add { _editStudyClickedHandler += value; }
            remove { _editStudyClickedHandler -= value; }
        }
        #endregion

        #region Protected Methods

        public override void DataBind()
        {
            // setup the data for the child controls
            if (Study != null)
            {
                PatientSummaryPanel.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(Study.TheStudy);

                StudyDetailsTabs.Partition = Study.ThePartition;
                StudyDetailsTabs.Study = Study;
                StudyStateAlertPanel.Study = Study;
            } 
            
            base.DataBind();

        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI(); 
            
            base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            if (Study!=null)
            {
                string reason;
                DeleteStudyButton.Enabled = Study.CanScheduleDelete(out reason);
                if (!DeleteStudyButton.Enabled)
                    DeleteStudyButton.ToolTip = reason;

                EditStudyButton.Enabled = Study.CanScheduleEdit(out reason);
                if (!EditStudyButton.Enabled)
                    EditStudyButton.ToolTip = reason;

            }

            SearchUpdatePanel.Update();// force update
        }

        protected void DeleteStudyButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_deleteStudyClickedHandler, this, new StudyDetailsPanelDeleteStudyClickEventArgs());
        }

        protected void EditStudyButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_editStudyClickedHandler, this, new StudyDetailsPanelEditStudyClickEventArgs());
        }

        #endregion Protected Methods

    }
}