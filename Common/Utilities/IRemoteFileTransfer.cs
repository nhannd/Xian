using System;

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
	/// Defines an interface for accessing remote files.
	/// </summary>
	public interface IRemoteFileTransfer
	{
		/// <summary>
		/// Upload one file from local to remote.
		/// </summary>
		/// <param name="request"></param>
		void Upload(FileTransferRequest request);

		/// <summary>
		/// Download one file from remote to local
		/// </summary>
		/// <param name="request"></param>
		void Download(FileTransferRequest request);
	}
}
