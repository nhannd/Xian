#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using System.Runtime.InteropServices;
using ClearCanvas.Ris.Application.Common.Jsml;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="PriorReportComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PriorReportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PriorReportComponent class
    /// </summary>
    [AssociateView(typeof(PriorReportComponentViewExtensionPoint))]
    public class PriorReportComponent : ApplicationComponent
    {
        /// <summary>
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.  It must be COM-visible.
        /// </summary>
        [ComVisible(true)]
        public class ScriptCallback
        {
            private PriorReportComponent _component;

            public ScriptCallback(PriorReportComponent component)
            {
                _component = component;
            }

            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string GetData(string tag)
            {
                string temp = _component.GetData(tag);
                return temp;
            }
        }

        private ScriptCallback _scriptCallback;

        private EntityRef _reportingStepRef;

        private ReportSummaryTable _reportList;
        private ReportSummary _selectedReport;

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorReportComponent(EntityRef reportingStepRef)
        {
            _scriptCallback = new ScriptCallback(this);

            _reportingStepRef = reportingStepRef;
            _reportList = new ReportSummaryTable();
        }

        public override void Start()
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetPriorReportResponse response = service.GetPriorReport(new GetPriorReportRequest(_reportingStepRef));
                    _reportList.Items.AddRange(response.Reports);
                });

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public ITable Reports
        {
            get { return _reportList; }
        }

        public ISelection SelectedReport
        {
            get { return _selectedReport == null ? Selection.Empty : new Selection(_selectedReport); }
            set
            {
                ReportSummary newSelection = (ReportSummary)value.Item;
                if (_selectedReport != newSelection)
                {
                    _selectedReport = newSelection;
                    NotifyAllPropertiesChanged();
                }
            }
        }

        public string PreviewUrl
        {
            get { return ReportEditorComponentSettings.Default.ReportPreviewPageUrl; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public string GetData(string tag)
        {
            return JsmlSerializer.Serialize(_selectedReport, "report");
        }

    }
}
