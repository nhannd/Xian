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
    public class MimeDocumentPreviewComponent : ApplicationComponent, IPreviewComponent
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
            : this(true, false)
        {
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        public MimeDocumentPreviewComponent(bool showSummary)
            : this(showSummary, false)
        {
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="showSummary">True to show the summary section, false to hide it</param>
        /// <param name="showToolbar">True to show the summary toolbar, false to hide it</param>
        public MimeDocumentPreviewComponent(bool showSummary, bool showToolbar)
        {
            _showSummary = showSummary;
            _showToolbar = showToolbar;

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

        public AttachmentMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
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
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "mimeDocument-items-tools", _toolSet.Actions); }
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

        public void ClearPreviewData()
        {
            _tempFileName = null;
            EventsHelper.Fire(_dataChanged, this, EventArgs.Empty);
        }

        public void SetPreviewData(string mimeType, string fileExtension, EntityRef dataRef)
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

        #region IPreviewComponent Members

        public void SetUrl(string url)
        {
            // this does nothing because this component does not take Url to change preview
        }

        #endregion
    }
}
