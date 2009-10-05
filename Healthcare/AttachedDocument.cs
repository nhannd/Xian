using System.Collections.Generic;
using System.IO;
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
		public void UploadDocument(RemoteFileAccessProvider fileAccessProivder, string localFilePath)
		{
			var now = Platform.Time;
			this.DataRelativeUrl = RemoteFileAccessProvider.BuildRelativeUrl(
				now.Year.ToString(),
				now.Month.ToString(),
				now.Day.ToString(),
				string.Format("{0}.{1}", this.GetRef().ToString(false, false), this.FileExtension));

			var remoteFileUrl = fileAccessProivder.GetFullUrl(this.DataRelativeUrl);

			var requests = new List<FileTransferRequest>
				{
					new FileTransferRequest(remoteFileUrl, localFilePath, FileTransferRequest.TransferMode.Upload)
				};

			fileAccessProivder.TransferFiles(requests);
		}

		/// <summary>
		/// Download a document to a temp file.
		/// </summary>
		/// <returns>The path of the downloaded file.</returns>
		public string DownloadDocument(RemoteFileAccessProvider fileAccessProivder)
		{
			var remoteFileUrl = fileAccessProivder.GetFullUrl(this.DataRelativeUrl);
			var localFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(remoteFileUrl));

			var requests = new List<FileTransferRequest>
				{
					new FileTransferRequest(remoteFileUrl, localFilePath, FileTransferRequest.TransferMode.Download)
				};

			fileAccessProivder.TransferFiles(requests);

			return localFilePath;
		}
	}
}