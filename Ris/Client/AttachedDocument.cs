#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
			var fileExtension = Path.GetExtension(fullUrl.LocalPath).Trim('.');
			var localFilePath = TempFileManager.Instance.CreateFile(documentSummary.DocumentRef, fileExtension, 
				TimeSpan.FromSeconds(AttachedDocumentSettings.Default.DownloadCacheTimeToLive));

			ftpFileTransfer.Download(new FileTransferRequest(fullUrl, localFilePath));

			return localFilePath;
		}
	}
}
