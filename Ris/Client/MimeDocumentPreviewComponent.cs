using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.MimeDocumentService;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="MimeDocumentPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class MimeDocumentPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// MimeDocumentPreviewComponent class
    /// </summary>
    [AssociateView(typeof(MimeDocumentPreviewComponentViewExtensionPoint))]
    public class MimeDocumentPreviewComponent : ApplicationComponent
    {
        public enum Mode
        {
            PatientAttachment,
            OrderAttachment
        }

        // Summary component members
        private readonly bool _showSummary;
        private readonly EntityRef _entityRef;
        private readonly Mode _mode;
        private readonly ITable _attachmentTable;
        private ISelection _selection;

        // Preview members
        private string _tempFileName;
        private event EventHandler _dataChanged;

        /// <summary>
        /// Constructor for showing only the preview section
        /// </summary>
        public MimeDocumentPreviewComponent()
        {
            _showSummary = false;
        }

        /// <summary>
        /// Constructors for showing the patient attachments in the summary and preview sections
        /// </summary>
        /// <param name="attachments">A list of patient attachments</param>
        public MimeDocumentPreviewComponent(IList<PatientAttachmentSummary> attachments)
        {
            _showSummary = true;
            _mode = Mode.PatientAttachment;

            _attachmentTable = new PatientAttachmentTable();
            _attachmentTable.Items.AddRange(attachments);
        }

        /// <summary>
        /// Constructors for showing the order attachments in the summary and preview sections
        /// </summary>
        /// <param name="attachments">A list of order attachments</param>
        public MimeDocumentPreviewComponent(IList<OrderAttachmentSummary> attachments)
        {
            _showSummary = true;
            _mode = Mode.OrderAttachment;

            _attachmentTable = new OrderAttachmentTable();
            _attachmentTable.Items.AddRange(attachments);
        }

        /// <summary>
        /// Constructor for showing both the summary and preview sections
        /// The component will retrieve a list of MimeDocuments
        /// </summary>
        /// <param name="entityRef">Patient or Order entity ref</param>
        /// <param name="mode"></param>
        public MimeDocumentPreviewComponent(EntityRef entityRef, Mode mode)
        {
            _showSummary = true;

            _entityRef = entityRef;
            _mode = mode;

            if (mode == Mode.PatientAttachment)
                _attachmentTable = new PatientAttachmentTable();
            else
                _attachmentTable = new OrderAttachmentTable();
        }

        public override void Start()
        {
            if (_showSummary && _entityRef != null)
            {
                if (_mode == Mode.PatientAttachment)
                {
                    Platform.GetService<IMimeDocumentService>(
                        delegate(IMimeDocumentService service)
                            {
                                GetAttachmentsForPatientResponse response =
                                    service.GetAttachmentsForPatient(new GetAttachmentsForPatientRequest(_entityRef));
                                _attachmentTable.Items.AddRange(response.Attachments);
                            });
                }
                else
                {
                    Platform.GetService<IMimeDocumentService>(
                        delegate(IMimeDocumentService service)
                            {
                                GetAttachmentsForOrderResponse response =
                                    service.GetAttachmentsForOrder(new GetAttachmentsForOrderRequest(_entityRef));
                                _attachmentTable.Items.AddRange(response.Attachments);
                            });
                }
            }
            base.Start();
        }

        #region Summary Methods

        public bool ShowSummary
        {
            get { return _showSummary; }
        }

        public ITable Attachments
        {
            get { return _attachmentTable; }
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
                        if (_mode == Mode.PatientAttachment)
                        {
                            PatientAttachmentSummary item = _selection.Item as PatientAttachmentSummary;
                            this.SetPreviewData(item.Document.MimeType, item.Document.FileExtension, item.Document.BinaryDataRef);
                        }
                        else
                        {
                            OrderAttachmentSummary item = _selection.Item as OrderAttachmentSummary;
                            this.SetPreviewData(item.Document.MimeType, item.Document.FileExtension, item.Document.BinaryDataRef);
                        }
                    }
                }
            }
        }

        #endregion

        #region Preview Methods

        public void ClearPreviewData()
        {
            _tempFileName = null;
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
