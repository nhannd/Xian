using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.AttachedDocumentService;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides methods for working with attached documents stored on the RIS server.
    /// </summary>
    public static class AttachedDocument
    {
        /// <summary>
        /// Downloads the specified binary data document to a temporary file with the
        /// specified extension.
        /// </summary>
        /// <param name="documentRef"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static string DownloadToTempFile(EntityRef documentRef, string fileExtension)
        {
            Platform.CheckForNullReference(documentRef, "dataRef");

            // if already cached locally, return local file name
            string tempFile = TempFileManager.Instance.GetFile(documentRef);
            if (!string.IsNullOrEmpty(tempFile))
                return tempFile;

            // retrieve data and create new local file
            Byte[] data = RetrieveData(documentRef);
			return TempFileManager.Instance.CreateFile(documentRef, fileExtension, data,
				TimeSpan.FromSeconds(AttachedDocumentSettings.Default.DownloadCacheTimeToLive));
        }

        private static byte[] RetrieveData(EntityRef dataRef)
        {
            byte[] data = null;

            Platform.GetService<IAttachedDocumentService>(
                delegate(IAttachedDocumentService service)
                {
                    GetDocumentDataResponse response = service.GetDocumentData(new GetDocumentDataRequest(dataRef));
                    data = response.BinaryData;
                });

            return data;
        }
    }
}
