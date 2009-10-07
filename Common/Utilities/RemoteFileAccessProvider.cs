using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Defines a request to transfer a file between local and remote file systems.
	/// </summary>
	public class FileTransferRequest
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public FileTransferRequest(Uri remoteFile, string localFile)
		{
			RemoteFile = remoteFile;
			LocalFile = localFile;
		}

		#region Public Properties

		/// <summary>
		/// The url of the remote file.
		/// </summary>
		public Uri RemoteFile { get; private set; }

		/// <summary>
		/// The complete path of the local file.
		/// </summary>
		public string LocalFile { get; private set; }

		#endregion
	}

	/// <summary>
	/// Defines a base class for accessing remote files.
	/// </summary>
	public abstract class RemoteFileAccessProvider
	{
		/// <summary>
		/// Upload files from local to remote.
		/// </summary>
		/// <param name="requests"></param>
		public void Upload(List<FileTransferRequest> requests)
		{
			FileTransferRequest requestBeingProcessed = null;

			try
			{
				foreach (var request in requests)
				{
					requestBeingProcessed = request;
					Upload(request);
				}
			}
			catch (Exception e)
			{
				var message = requestBeingProcessed == null
					? SR.ExceptionFailedToInitializeFileTransfer
					: string.Format(SR.ExceptionFailedToTransferFile, requestBeingProcessed.LocalFile, requestBeingProcessed.RemoteFile);
				throw new Exception(message, e);
			}
		}

		/// <summary>
		/// Download files from remote to local.
		/// </summary>
		/// <param name="requests"></param>
		public void Download(List<FileTransferRequest> requests)
		{
			FileTransferRequest requestBeingProcessed = null;

			try
			{
				foreach (var request in requests)
				{
					requestBeingProcessed = request;
					Download(request);
				}
			}
			catch (Exception e)
			{
				var message = requestBeingProcessed == null
					? SR.ExceptionFailedToInitializeFileTransfer
					: string.Format(SR.ExceptionFailedToTransferFile, requestBeingProcessed.RemoteFile, requestBeingProcessed.LocalFile);
				throw new Exception(message, e);
			}
		}

		/// <summary>
		/// Upload one file from local to remote.
		/// </summary>
		/// <param name="request"></param>
		protected abstract void Upload(FileTransferRequest request);

		/// <summary>
		/// Download one file from remote to local
		/// </summary>
		/// <param name="request"></param>
		protected abstract void Download(FileTransferRequest request);
	}
}
