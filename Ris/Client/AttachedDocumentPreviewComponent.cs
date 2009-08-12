#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="AttachedDocumentPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AttachedDocumentPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class AttachedDocumentToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IAttachedDocumentToolContext : IToolContext
    {
        event EventHandler SelectedDocumentChanged;
        EntityRef SelectedDocumentRef { get; }

        void RemoveSelectedDocument();
        event EventHandler ChangeCommitted;

        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// AttachedDocumentPreviewComponent class
    /// </summary>
    [AssociateView(typeof(AttachedDocumentPreviewComponentViewExtensionPoint))]
    public class AttachedDocumentPreviewComponent : ApplicationComponent
    {
        public enum AttachmentMode
        {
            Patient,
            Order
        }

        class AttachedDocumentToolContext : ToolContext, IAttachedDocumentToolContext
        {
            private readonly AttachedDocumentPreviewComponent _component;

            internal AttachedDocumentToolContext(AttachedDocumentPreviewComponent component)
            {
                _component = component;
            }

            #region IAttachedDocumentToolContext Members

            public event EventHandler SelectedDocumentChanged
            {
                add { _component.SelectedDocumentChanged += value; }
                remove { _component.SelectedDocumentChanged -= value; }
            }

            public EntityRef SelectedDocumentRef
            {
                get { return _component.SelectedDocument == null ? null : _component.SelectedDocument.DocumentRef; }
            }

            public void RemoveSelectedDocument()
            {
                _component.RemoveSelectedDocument();
            }

            public event EventHandler ChangeCommitted
            {
                add { _component.ChangeCommited += value; }
                remove { _component.ChangeCommited -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            #endregion
        }

        class AttachedDocumentDHtmlPreviewComponent : DHtmlComponent
        {
            private readonly AttachedDocumentPreviewComponent _component;

            public AttachedDocumentDHtmlPreviewComponent(AttachedDocumentPreviewComponent component)
            {
                _component = component;
                this.SetUrl(this.PreviewUrl);
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _component.SelectedDocument;
            }

            public string PreviewUrl
            {
                get { return WebResourcesSettings.Default.AttachedDocumentPreviewUrl; }
            }

            public void Refresh()
            {
                this.SetUrl(this.PreviewUrl);
            }
        }

        // Summary component members
        private readonly bool _showSummary;
        private readonly bool _showToolbar;
        private AttachmentMode _mode;
        private ITable _attachmentTable;
        private ISelection _selection;
        private event EventHandler _changeCommitted;
        private event EventHandler _selectedDocumentChanged;

        private List<PatientAttachmentSummary> _patientAttachments;
        private List<OrderAttachmentSummary> _orderAttachments;

        private AttachedDocumentDHtmlPreviewComponent _previewComponent;
        private ChildComponentHost _previewComponentHost;

        private ToolSet _toolSet;

        /// <summary>
        /// Default Constructor to show summary but hide all tools
        /// </summary>
        public AttachedDocumentPreviewComponent()
            : this(true, false, AttachmentMode.Patient)
        {
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        public AttachedDocumentPreviewComponent(bool showSummary)
            : this(showSummary, false, AttachmentMode.Patient)
        {
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        /// <param name="showToolbar">True to show the summary toolbar, false to hide it</param>
        public AttachedDocumentPreviewComponent(bool showSummary, bool showToolbar)
            : this(showSummary, showToolbar, AttachmentMode.Patient)
        {
            _showSummary = showSummary;
            _showToolbar = showToolbar;

            _patientAttachments = new List<PatientAttachmentSummary>();
            _orderAttachments = new List<OrderAttachmentSummary>();
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        /// <param name="showToolbar">True to show the summary toolbar, false to hide it</param>
        /// <param name="mode">Set the component attachment mode</param>
        public AttachedDocumentPreviewComponent(bool showSummary, bool showToolbar, AttachmentMode mode)
        {
            _showSummary = showSummary;
            _showToolbar = showToolbar;
            _mode = mode;

            _patientAttachments = new List<PatientAttachmentSummary>();
            _orderAttachments = new List<OrderAttachmentSummary>();
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new AttachedDocumentToolExtensionPoint(), new AttachedDocumentToolContext(this));

            _previewComponent = new AttachedDocumentDHtmlPreviewComponent(this);
            _previewComponentHost = new ChildComponentHost(this.Host, _previewComponent);
            _previewComponentHost.StartComponent();

            base.Start();
        }

        public override void Stop()
        {
            if (_previewComponentHost != null)
            {
                _previewComponentHost.StopComponent();
                _previewComponentHost = null;
            }

            _toolSet.Dispose(); 
            base.Stop();
        }

        #region Events

        public event EventHandler ChangeCommited
        {
            add { _changeCommitted += value; }
            remove { _changeCommitted -= value; }
        }

        public event EventHandler SelectedDocumentChanged
        {
            add { _selectedDocumentChanged += value; }
            remove { _selectedDocumentChanged -= value; }
        }

        #endregion

        #region Presentation Models

        public ApplicationComponentHost PreviewHost
        {
            get { return _previewComponentHost; }
        }

        public bool ShowSummary
        {
            get { return _showSummary; }
        }

        public bool ShowToolbar
        {
            get { return _showToolbar; }
        }

        public List<PatientAttachmentSummary> PatientAttachments
        {
            get { return _mode != AttachmentMode.Patient ? null : _patientAttachments; }
            set
            {
                _patientAttachments = value;
                _mode = AttachmentMode.Patient;
                PatientAttachmentTable table = new PatientAttachmentTable();
                table.Items.AddRange(_patientAttachments);
                _attachmentTable = table;
            }
        }
            
        public List<OrderAttachmentSummary> OrderAttachments
        {
            get { return _mode != AttachmentMode.Order ? null : _orderAttachments; }
            set
            {
                _orderAttachments = value;
                _mode = AttachmentMode.Order;
                OrderAttachmentTable table = new OrderAttachmentTable();
                table.Items.AddRange(_orderAttachments);
                _attachmentTable = table;
            }
        }

        public ITable Attachments
        {
            get { return _attachmentTable; }
        }

        public ActionModelRoot AttachmentActionModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "attached-document-items", _toolSet.Actions); }
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
        }

        public ISelection Selection
        {
            get { return _selection; }
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    _previewComponent.Refresh();

                    EventsHelper.Fire(_selectedDocumentChanged, this, EventArgs.Empty);
                }
            }
        }

        #endregion

        public AttachedDocumentSummary SelectedDocument
        {
            get
            {
                if (_selection == null)
                    return null;

                if (_mode == AttachmentMode.Patient)
                {
                    PatientAttachmentSummary item = _selection.Item as PatientAttachmentSummary;
                    return item == null ? null : item.Document;
                }
                else
                {
                    OrderAttachmentSummary item = _selection.Item as OrderAttachmentSummary;
                    return item == null ? null : item.Document;
                }
            }
        }

        public void RemoveSelectedDocument()
        {
            if (_selection == null)
                return;

            _attachmentTable.Items.Remove(_selection.Item);

            if (_mode == AttachmentMode.Patient)
                _patientAttachments.Remove((PatientAttachmentSummary)_selection.Item);
            else
                _orderAttachments.Remove((OrderAttachmentSummary)_selection.Item);

            this.Modified = true;
        }

        public void SaveChanges()
        {
            EventsHelper.Fire(_changeCommitted, this, EventArgs.Empty);
        }

	}
}
