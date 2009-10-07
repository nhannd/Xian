using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare {


	/// <summary>
	/// AttachedDocument entity
	/// </summary>
	public partial class AttachedDocument : ClearCanvas.Enterprise.Core.Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Marks this document as having been attached.
		/// </summary>
		public virtual void Attach()
		{
			return;
		}

		/// <summary>
		/// Marks this document as being detached.
		/// </summary>
		public virtual void Detach()
		{
			
		}

		/// <summary>
		/// Summary of derived-class specific details of the attached document
		/// </summary>
		public virtual IDictionary<string, string> DocumentHeaders
		{
			get { return null; }
		}

		public virtual string DocumentTypeName
		{
			get { return "Attached Document"; }
		}

		/// <summary>
		/// Upload a document to a storage site.
		/// </summary>
		/// <remarks>The DataRelativeUrl is constructed based on Year/Month/Day/EntityRefOID.FileExtension.</remarks>
		public void UploadDocument(FtpFileTransfer ftpFileTransfer, string localFilePath)
		{
			this.DataRelativeUrl = BuildAttachedDocumentRelativeUri();
			var remoteFileUrl = new Uri(ftpFileTransfer.BaseUri, this.DataRelativeUrl);
			ftpFileTransfer.Upload(new FileTransferRequest(remoteFileUrl, localFilePath));
		}

		/// <summary>
		/// Download a document to a temp file.
		/// </summary>
		/// <returns>The path of the downloaded file.</returns>
		public string DownloadDocument(FtpFileTransfer ftpFileTransfer)
		{
			var remoteFileUrl = new Uri(ftpFileTransfer.BaseUri, this.DataRelativeUrl);
			var localFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(remoteFileUrl.LocalPath));
			ftpFileTransfer.Download(new FileTransferRequest(remoteFileUrl, localFilePath));

			return localFilePath;
		}

		private string BuildAttachedDocumentRelativeUri()
		{
			var now = Platform.Time;

			var builder = new StringBuilder();
			builder.Append(now.Year.ToString());
			builder.Append("/");
			builder.Append(now.Month.ToString());
			builder.Append("/");
			builder.Append(now.Day.ToString());
			builder.Append("/");
			builder.AppendFormat("{0}.{1}", this.GetRef().ToString(false, false), this.FileExtension);
			return builder.ToString();
		}
	}
}