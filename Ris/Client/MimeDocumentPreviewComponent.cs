using System;
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
        private readonly bool _hideSummary;
        private Mode _mode;
        private ITable _attachmentTable;
        private ISelection _selection;

        // Preview members
        private string _tempFileName;
        private event EventHandler _dataChanged;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MimeDocumentPreviewComponent()
        {
            _hideSummary = false;
        }

        /// <summary>
        /// Constructor to show/hide the summary section
        /// </summary>
        /// <param name="hideSummary">True to hide the summary section</param>
        public MimeDocumentPreviewComponent(bool hideSummary)
        {
            _hideSummary = hideSummary;
        }

        #region Summary Methods

        public bool HideSummary
        {
            get { return _hideSummary; }
        }

        public IList<PatientAttachmentSummary> PatientAttachments
        {
            get { return _mode != Mode.PatientAttachment ? null : (IList<PatientAttachmentSummary>) _attachmentTable.Items; }
            set
            {
                _mode = Mode.PatientAttachment;
                _attachmentTable = new PatientAttachmentTable();
                _attachmentTable.Items.AddRange(value);
            }
        }
            
        public IList<OrderAttachmentSummary> OrderAttachments
        {
            get { return _mode != Mode.OrderAttachment ? null : (IList<OrderAttachmentSummary>)_attachmentTable.Items; }
            set
            {
                _mode = Mode.OrderAttachment;
                _attachmentTable = new OrderAttachmentTable();
                _attachmentTable.Items.AddRange(value);
            }
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
