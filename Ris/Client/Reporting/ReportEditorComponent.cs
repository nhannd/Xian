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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ReportEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportEditorComponent class
    /// </summary>
    [AssociateView(typeof(ReportEditorComponentViewExtensionPoint))]
    public class ReportEditorComponent : ApplicationComponent, IReportEditor
    {
        public class DHtmlReportEditorComponent : DHtmlComponent
        {
            private readonly ReportEditorComponent _owner;

            public DHtmlReportEditorComponent(ReportEditorComponent owner)
            {
                _owner = owner;
            }

            public void Refresh()
            {
                NotifyAllPropertiesChanged();
            }

            protected override string GetTag(string tag)
            {
                switch (tag)
                {
                    case "Report":
                        ReportPartDetail reportPart = _owner._report.GetPart(0);
                        return reportPart == null ? "" : reportPart.Content;
                    case "Addendum":
                        ReportPartDetail addendumPart = _owner._reportPart.Index > 0 ? _owner._reportPart : null;
                        return addendumPart == null ? "" : addendumPart.Content;
                    case "Preview":
                        return JsmlSerializer.Serialize(_owner._report, "report");
                    default:
                        string value;
                        _owner._extendedProperties.TryGetValue(tag, out value);
                        if(string.IsNullOrEmpty(value))
                        {
                            value = JsmlSerializer.Serialize(_owner._report, "report");
                        }
                        return value;
                }
            }

            protected override void SetTag(string tag, string data)
            {
                switch (tag)
                {
                    case "Report":
                    case "Addendum":
                        _owner.ReportContent = data;
                        break;
                    default:
                        break;
                }
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _owner._worklistItem;
            }
        }

        private readonly DHtmlReportEditorComponent _reportEditorComponent;
        private ChildComponentHost _reportEditorHost;

        private readonly DHtmlReportEditorComponent _reportPreviewComponent;
        private ChildComponentHost _reportPreviewHost;

        private ReportingWorklistItem _worklistItem;
        private ReportDetail _report;
        private ReportPartDetail _reportPart;

        private bool _isEditingAddendum;
        private bool _verifyEnabled;
        private bool _sendToVerifyEnabled;
        private bool _sendToTranscriptionEnabled;

        private event EventHandler _verifyRequested;
        private event EventHandler _sendToVerifyRequested;
        private event EventHandler _sendToTranscriptionRequested;
        private event EventHandler _saveRequested;
        private event EventHandler _cancelRequested;

        private Dictionary<string, string> _extendedProperties;
        private ILookupHandler _supervisorLookupHandler;

        public ReportEditorComponent()
        {
            _reportEditorComponent = new DHtmlReportEditorComponent(this);
            _reportPreviewComponent = new DHtmlReportEditorComponent(this);
        }

        public override void Start()
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    ListProcedureExtendedPropertiesResponse extendedPropertiesResponse = service.ListProcedureExtendedProperties(new ListProcedureExtendedPropertiesRequest(_worklistItem.ProcedureRef));
                    _extendedProperties = CollectionUtils.FirstElement(extendedPropertiesResponse.ProcedureExtendedProperties);
                });
            _supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, new string[] { "PRAD" });

            _reportEditorComponent.SetUrl(this.EditorUrl);
            _reportEditorHost = new ChildComponentHost(this.Host, _reportEditorComponent);
            _reportEditorHost.StartComponent();

            _reportPreviewComponent.SetUrl(this.PreviewUrl);
            _reportPreviewHost = new ChildComponentHost(this.Host, _reportPreviewComponent);
            _reportPreviewHost.StartComponent();

            base.Start();
        }

        private string EditorUrl
        {
            get { return this.IsEditingAddendum ? ReportEditorComponentSettings.Default.AddendumEditorPageUrl : ReportEditorComponentSettings.Default.ReportEditorPageUrl; }
        }

        private string PreviewUrl
        {
            get { return this.IsEditingAddendum ? ReportEditorComponentSettings.Default.ReportPreviewPageUrl : "about:blank"; }
        }

        #region IReportEditor Members

        public string ReportContent
        {
            get { return _reportPart.Content; }
            set { _reportPart.Content = value; }
        }

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set { _worklistItem = value; }
        }

        public ReportDetail Report
        {
            get { return _report; }
            set { _report = value; }
        }

        public ReportPartDetail ReportPart
        {
            get { return _reportPart; }
            set { _reportPart = value; }
        }

        public event EventHandler VerifyRequested
        {
            add { _verifyRequested += value; }
            remove { _verifyRequested -= value; }
        }

        public event EventHandler SendToVerifyRequested
        {
            add { _sendToVerifyRequested += value; }
            remove { _sendToVerifyRequested -= value; }
        }

        public event EventHandler SendToTranscriptionRequested
        {
            add { _sendToTranscriptionRequested += value; }
            remove { _sendToTranscriptionRequested -= value; }
        }

        public event EventHandler SaveRequested
        {
            add { _saveRequested += value; }
            remove { _saveRequested -= value; }
        }

        public event EventHandler CancelRequested
        {
            add { _cancelRequested += value; }
            remove { _cancelRequested -= value; }
        }

        public bool IsEditingAddendum
        {
            get { return _isEditingAddendum; }
            set { _isEditingAddendum = value; }
        }

        public bool VerifyEnabled
        {
            get { return _verifyEnabled; }
            set { _verifyEnabled = value; }
        }

        public bool SendToVerifyEnabled
        {
            get { return _sendToVerifyEnabled; }
            set { _sendToVerifyEnabled = value; }
        }

        public bool SendToTranscriptionEnabled
        {
            get { return _sendToTranscriptionEnabled; }
            set { _sendToTranscriptionEnabled = value; }
        }

        public StaffSummary Supervisor
        {
            get { return _reportPart.Supervisor; }
            set { _reportPart.Supervisor = value; }
        }

        #endregion

        #region Presentation Model

        public ApplicationComponentHost ReportEditorHost
        {
            get { return _reportEditorHost; }
        }

        public ApplicationComponentHost ReportPreviewHost
        {
            get { return _reportPreviewHost; }
                }

        public bool CanSendToTranscription
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
        }

        public bool CanVerifyReport
                {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
                }

        public void Verify()
            {
            _reportEditorComponent.SaveData();
            EventsHelper.Fire(_verifyRequested, this, EventArgs.Empty);
            }

        public void SendToVerify()
        {
                _reportEditorComponent.SaveData();
            EventsHelper.Fire(_sendToVerifyRequested, this, EventArgs.Empty);
                }

        public void SendToTranscription()
        {
                _reportEditorComponent.SaveData();
            EventsHelper.Fire(_sendToTranscriptionRequested, this, EventArgs.Empty);
            }

        public void Save()
        {
                _reportEditorComponent.SaveData();
            EventsHelper.Fire(_saveRequested, this, EventArgs.Empty);
            }

        public void Cancel()
        {
            EventsHelper.Fire(_cancelRequested, this, EventArgs.Empty);
        }

        public ILookupHandler SupervisorLookupHandler
        {
            get { return _supervisorLookupHandler; }
        }

        public IList GetRadiologistSuggestion(string query)
        {
            ArrayList suggestions = new ArrayList();
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        GetRadiologistListResponse response = service.GetRadiologistList(new GetRadiologistListRequest());
                        suggestions.AddRange(response.Radiologists);
                    });
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return suggestions;
        }

        #endregion
            }
}
