using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides methods for working with attached documents stored on the RIS server.
	/// </summary>
	public static class AttachedDocument
	{
		/// <summary>
		/// Downloads a document at a specified relativeUrl to a temporary file.
		/// </summary>
		/// <param name="documentSummary"></param>
		/// <returns>The location of the downloaded file.</returns>
		public static string DownloadFile(AttachedDocumentSummary documentSummary)
		{
			Platform.CheckForNullReference(documentSummary, "documentSummary");

			// if already cached locally, return local file name
			var tempFile = TempFileManager.Instance.GetFile(documentSummary.DocumentRef);
			if (!string.IsNullOrEmpty(tempFile))
				return tempFile;

			var ftpFileTransfer = new FtpFileTransfer(
				AttachedDocumentSettings.Default.FtpUserId,
				AttachedDocumentSettings.Default.FtpPassword,
				AttachedDocumentSettings.Default.FtpBaseUrl,
				AttachedDocumentSettings.Default.FtpPassiveMode);

			var fullUrl = new Uri(ftpFileTransfer.BaseUri, documentSummary.ContentUrl);

			var fileName = Path.GetFileName(fullUrl.LocalPath);
			var localFilePath = Path.Combine(Path.GetTempPath(), fileName);

			ftpFileTransfer.Download(new FileTransferRequest(fullUrl, localFilePath));

			return localFilePath;
		}
	}
}
