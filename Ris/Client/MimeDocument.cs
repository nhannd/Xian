using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.MimeDocumentService;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides methods for working with MIME documents stored on
    /// the RIS server.
    /// </summary>
    public static class MimeDocument
    {
        /// <summary>
        /// Downloads the specified binary data document to a temporary file with the
        /// specified extension.
        /// </summary>
        /// <param name="dataRef"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static string DownloadToTempFile(EntityRef documentRef, string fileExtension)
        {
            Platform.CheckForNullReference(documentRef, "dataRef");

            // if already cached locally, return local file name
            string tempFile = TempFileManager.Instance.GetTempFile(documentRef);
            if (!string.IsNullOrEmpty(tempFile))
                return tempFile;

            // retrieve data and create new local file
            Byte[] data = RetrieveData(documentRef);
            return TempFileManager.Instance.CreateTemporaryFile(documentRef, fileExtension, data);
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
    }
}
