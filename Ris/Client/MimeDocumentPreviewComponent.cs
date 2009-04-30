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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.MimeDocumentService;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="MimeDocumentPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class MimeDocumentPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class MimeDocumentToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IMimeDocumentToolContext : IToolContext
    {
        event EventHandler SelectedDocumentChanged;
        EntityRef SelectedDocumentRef { get; }

        void RemoveSelectedDocument();
        event EventHandler ChangeCommitted;

        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// MimeDocumentPreviewComponent class
    /// </summary>
    [AssociateView(typeof(MimeDocumentPreviewComponentViewExtensionPoint))]
    public class MimeDocumentPreviewComponent : ApplicationComponent
    {
        public enum AttachmentMode
        {
            Patient,
            Order
        }

        class MimeDocumentToolContext : ToolContext, IMimeDocumentToolContext
        {
            private readonly MimeDocumentPreviewComponent _component;

            internal MimeDocumentToolContext(MimeDocumentPreviewComponent component)
            {
                _component = component;
            }

            #region IMimeDocumentToolContext Members

            public event EventHandler SelectedDocumentChanged
            {
                add { _component.DataChanged += value; }
                remove { _component.DataChanged -= value; }
            }

            public EntityRef SelectedDocumentRef
            {
                get { return _component.SelectedDocumentRef; }
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

        // Summary component members
        private readonly bool _showSummary;
        private readonly bool _showToolbar;
        private AttachmentMode _mode;
        private ITable _attachmentTable;
        private ISelection _selection;
        private event EventHandler _changeCommitted;

        private List<PatientAttachmentSummary> _patientAttachments;
        private List<OrderAttachmentSummary> _orderAttachments;

        // Preview members
        private string _tempFileName;
        private event EventHandler _dataChanged;

        private ToolSet _toolSet;

        /// <summary>
        /// Default Constructor to show summary but hide all tools
        /// </summary>
        public MimeDocumentPreviewComponent()
            : this(true, false, AttachmentMode.Patient)
        {
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        public MimeDocumentPreviewComponent(bool showSummary)
            : this(showSummary, false, AttachmentMode.Patient)
        {
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        /// <param name="showToolbar">True to show the summary toolbar, false to hide it</param>
        public MimeDocumentPreviewComponent(bool showSummary, bool showToolbar)
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
        public MimeDocumentPreviewComponent(bool showSummary, bool showToolbar, AttachmentMode mode)
        {
            _showSummary = showSummary;
            _showToolbar = showToolbar;
            _mode = mode;

            _patientAttachments = new List<PatientAttachmentSummary>();
            _orderAttachments = new List<OrderAttachmentSummary>();
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new MimeDocumentToolExtensionPoint(), new MimeDocumentToolContext(this));
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose(); 
            base.Stop();
        }

        #region Summary Methods

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
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "mimedocument-items", _toolSet.Actions); }
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

                    if (_selection == null)
                        this.ClearPreviewData();
                    else
                    {
                        if (_mode == AttachmentMode.Patient)
                        {
                            PatientAttachmentSummary item = _selection.Item as PatientAttachmentSummary;
                            if (item == null)
                                this.ClearPreviewData();
                            else
                                this.SetPreviewData(item.Document.MimeType, item.Document.FileExtension, item.Document.BinaryDataRef);
                        }
                        else
                        {
                            OrderAttachmentSummary item = _selection.Item as OrderAttachmentSummary;
                            if (item == null)
                                this.ClearPreviewData();
                            else
                                this.SetPreviewData(item.Document.MimeType, item.Document.FileExtension, item.Document.BinaryDataRef);
                        }
                    }
                }
            }
        }

        public EntityRef SelectedDocumentRef
        {
            get
            {
                if (_selection == null)
                    return null;

                if (_mode == AttachmentMode.Patient)
                {
                    PatientAttachmentSummary item = _selection.Item as PatientAttachmentSummary;
                    return item == null ? null : item.Document.DocumentRef;
                }
                else
                {
                    OrderAttachmentSummary item = _selection.Item as OrderAttachmentSummary;
                    return item == null ? null : item.Document.DocumentRef;
                }
            }
        }

        public event EventHandler ChangeCommited
        {
            add { _changeCommitted += value; }
            remove { _changeCommitted -= value; }
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

        #endregion

        #region Preview Methods

        protected void ClearPreviewData()
        {
            _tempFileName = null;
            EventsHelper.Fire(_dataChanged, this, EventArgs.Empty);
        }

		protected void SetPreviewData(string mimeType, string fileExtension, EntityRef dataRef)
        {
            if (dataRef == null)
            {
                _tempFileName = null;                    
            }
            else
            {
                string tempFile = TempFileManager.Instance.GetTempFile(dataRef);
                if (String.IsNullOrEmpty(tempFile))
                {
                    try
                    {
                        Byte[] data = RetrieveData(dataRef);
                        _tempFileName = TempFileManager.Instance.CreateTemporaryFile(dataRef, fileExtension, data);
                    }
                    catch (Exception e)
                    {
                        _tempFileName = null;
                        ExceptionHandler.Report(e, SR.ExceptionFailedToDisplayDocument, this.Host.DesktopWindow);
                    }
                }
                else
                {
                    if (Equals(_tempFileName, tempFile))
                        return;  // nothing has changed

                    _tempFileName = tempFile;
                }
            }

            EventsHelper.Fire(_dataChanged, this, EventArgs.Empty);
        }

        public string TempFileName
        {
            get { return _tempFileName; }
        }

        public event EventHandler DataChanged
        {
            add { _dataChanged += value; }
            remove { _dataChanged -= value; }
        }

        public void SaveChanges()
        {
            EventsHelper.Fire(_changeCommitted, this, EventArgs.Empty);
        }

        private static byte[] RetrieveData(EntityRef dataRef)
        {
            byte[] data = null;

            Platform.GetService<IMimeDocumentService>(
                delegate(IMimeDocumentService service)
                    {
                        GetDocumentDataResponse response = service.GetDocumentData(new GetDocumentDataRequest(dataRef));
                        data = response.BinaryData;
                    });

            return data;
        }

        #endregion

	}
}
