using System;
using System.IO;
using System.Collections.Generic;
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

			var ftp = new FtpFileAccessProvider(
				AttachedDocumentFtpSettings.Default.FtpUserId,
				AttachedDocumentFtpSettings.Default.FtpPassword,
				AttachedDocumentFtpSettings.Default.FtpBaseUrl,
				AttachedDocumentFtpSettings.Default.FtpPassiveMode);

			var remoteFileUrl = new Uri(ftp.BaseUri, documentSummary.DataRelativeUrl);

			var fileName = Path.GetFileName(remoteFileUrl.LocalPath);
			var localFilePath = Path.Combine(Path.GetTempPath(), fileName);
			var requests = new List<FileTransferRequest>
				{
					new FileTransferRequest(remoteFileUrl, localFilePath)
				};

			ftp.Download(requests);

			return localFilePath;
		}
	}
}
