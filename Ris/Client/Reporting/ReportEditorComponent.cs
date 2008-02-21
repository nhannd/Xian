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
    [AssociateView(typeof (ReportEditorComponentViewExtensionPoint))]
    public class ReportEditorComponent : ApplicationComponent, IReportEditor
    {
        #region Hackiness

        // TODO: this is a temporary hack - remove this class and use Healthcare context instead of tags
        private class TagStore : IDictionary<string, string>
        {
            private readonly ReportEditorComponent _owner;

            public TagStore(ReportEditorComponent owner)
            {
                _owner = owner;
            }

            public bool TryGetValue(string key, out string value)
            {
                switch (key)
                {
                    case "Report":
                        value = _owner._reportingContext.ReportContent;
                        return true;
                    case "Addendum":
                        value = _owner._reportingContext.ReportContent;
                        return true;
                    case "Preview":
                        value = JsmlSerializer.Serialize(_owner._reportingContext.Report, "report");
                        return true;
                    default:
                        value = null;
                        //_owner._extendedProperties.TryGetValue(key, out value);
                        //if (string.IsNullOrEmpty(value))
                        //{
                        //    value = JsmlSerializer.Serialize(_owner._reportingContext.Report, "report");
                        //}
                        return false;
                }
            }

            public string this[string key]
            {
                get
                {
                    string value;
                    TryGetValue(key, out value);
                    return value;
                }
                set
                {
                    switch (key)
                    {
                        case "Report":
                        case "Addendum":
                            _owner._reportingContext.ReportContent = value;
                            break;
                        default:
                            break;
                    }
                }
            }

            #region Unimplemented Members

            public ICollection<string> Values
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public void Add(string key, string value)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool ContainsKey(string key)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public ICollection<string> Keys
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public bool Remove(string key)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            public void Add(KeyValuePair<string, string> item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Clear()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool Contains(KeyValuePair<string, string> item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public int Count
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public bool IsReadOnly
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public bool Remove(KeyValuePair<string, string> item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        public class DHtmlReportEditorComponent : DHtmlComponent
        {
            private readonly ReportEditorComponent _owner;
            private readonly TagStore _tagStore;

            public DHtmlReportEditorComponent(ReportEditorComponent owner)
            {
                _owner = owner;
                _tagStore = new TagStore(owner);
            }

            public void Refresh()
            {
                NotifyAllPropertiesChanged();
            }

            protected override IDictionary<string, string> TagData
            {
                get
                {
                    return _tagStore;
                }
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _owner._reportingContext.WorklistItem;
            }
        }

        private readonly DHtmlReportEditorComponent _editingComponent;
        private ChildComponentHost _editingHost;

        private readonly DHtmlReportEditorComponent _previewComponent;
        private ChildComponentHost _previewHost;

        private readonly IReportingContext _reportingContext;

        private Dictionary<string, string> _extendedProperties;
        private ILookupHandler _supervisorLookupHandler;

        public ReportEditorComponent(IReportingContext context)
        {
            _reportingContext = context;
            _editingComponent = new DHtmlReportEditorComponent(this);
            _previewComponent = new DHtmlReportEditorComponent(this);
        }

        public override void Start()
        {
            _supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, new string[] {"PRAD"});

            _editingComponent.SetUrl(this.EditorUrl);
            _editingHost = new ChildComponentHost(this.Host, _editingComponent);
            _editingHost.StartComponent();

            _previewComponent.SetUrl(this.PreviewUrl);
            _previewHost = new ChildComponentHost(this.Host, _previewComponent);
            _previewHost.StartComponent();

            base.Start();
        }

        private string EditorUrl
        {
            get
            {
                return IsAddendum
                        ? ReportEditorComponentSettings.Default.AddendumEditorPageUrl
                        : ReportEditorComponentSettings.Default.ReportEditorPageUrl;
            }
        }

        private string PreviewUrl
        {
            get
            {
                return IsAddendum ? ReportEditorComponentSettings.Default.ReportPreviewPageUrl : "about:blank";
            }
        }

        #region Presentation Model

        public bool IsAddendum
        {
            get { return _reportingContext.ActiveReportPartIndex > 0; }
        }

        public ApplicationComponentHost ReportEditorHost
        {
            get { return _editingHost; }
        }

        public ApplicationComponentHost ReportPreviewHost
        {
            get { return _previewHost; }
        }

        public bool SendToTranscriptionVisible
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
        }

        public bool VerifyReportVisible
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
        }

        public bool VerifyEnabled
        {
            get { return _reportingContext.CanVerify; }
        }

        public bool SendToVerifyEnabled
        {
            get { return _reportingContext.CanSendToBeVerified; }
        }

        public bool SendToTranscriptionEnabled
        {
            get { return _reportingContext.CanSendToTranscription; }
        }
            
        public void Verify()
        {
            _editingComponent.SaveData();
            _reportingContext.VerifyReport();
        }

        public void SendToVerify()
        {
            _editingComponent.SaveData();
            _reportingContext.SendToBeVerified();
        }

        public void SendToTranscription()
        {
            _editingComponent.SaveData();
            _reportingContext.SendToTranscription();
        }

        public void Save()
        {
            _editingComponent.SaveData();
            _reportingContext.SaveReport();
        }

        public void Cancel()
        {
            _reportingContext.CancelEditing();
        }

        public StaffSummary Supervisor
        {
            get { return _reportingContext.Supervisor; }
            set
            {
                if(!Equals(value, _reportingContext.Supervisor))
                {
                    _reportingContext.Supervisor = value;
                    NotifyPropertyChanged("Supervisor");
                }
            }
        }

        public ILookupHandler SupervisorLookupHandler
        {
            get { return _supervisorLookupHandler; }
        }

        #endregion

        #region IReportEditor Members

        IApplicationComponent IReportEditor.GetComponent()
        {
            return this;
        }

        #endregion
    }
}
