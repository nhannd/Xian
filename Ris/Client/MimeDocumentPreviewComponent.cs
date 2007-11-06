using System;
using System.IO;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.MimeDocumentService;

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
        private string _tempFileName;
        private event EventHandler _dataChanged;

        private static readonly Dictionary<EntityRef, string> _tempFileDictionary = new Dictionary<EntityRef, string>();

        ~MimeDocumentPreviewComponent()
        {
            try
            {
                // Clean up all temporary files
                foreach (EntityRef key in _tempFileDictionary.Keys)
                {
                    if (File.Exists(_tempFileDictionary[key]))
                        File.Delete(_tempFileDictionary[key]);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, SR.ExceptioinFailedToDeleteTemporaryFiles);
            }
        }

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
                if (_tempFileDictionary.ContainsKey(dataRef) && File.Exists(_tempFileDictionary[dataRef]))
                {
                    if (Equals(_tempFileName, _tempFileDictionary[dataRef]))
                        return;  // nothing has changed

                    _tempFileName = _tempFileDictionary[dataRef];
                }
                else
                {
                    try
                    {
                        Byte[] data = RetrieveData(dataRef);
                        _tempFileName = CreateTemporaryFile(fileExtension, data);

                        // Remember the temp file
                        _tempFileDictionary[dataRef] = _tempFileName;
                    }
                    catch (Exception e)
                    {
                        _tempFileName = null;
                        ExceptionHandler.Report(e, SR.ExceptionFailedToDisplayDocument, this.Host.DesktopWindow);
                    }
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

        private static string CreateTemporaryFile(string fileExtension, byte[] data)
        {
            string tempFileName = String.Format("{0}.{1}", System.IO.Path.GetTempFileName(), fileExtension);
            FileStream fs = new FileStream(tempFileName, FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();

            return tempFileName;
        }

    }
}
