using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Defines a request to transfer a file between local and remote file systems.
	/// </summary>
	public class FileTransferRequest
	{
		/// <summary>
		/// Enum defining the transfer mode of the current request.
		/// </summary>
		public enum TransferMode
		{
			/// <summary>
			/// Indicates a request to upload from local to remote.
			/// </summary>
			Upload,

			/// <summary>
			/// Indicates a request to download from remote to local.
			/// </summary>
			Download
		};

		/// <summary>
		/// Constructor.
		/// </summary>
		public FileTransferRequest(string remoteFile, string localFile, TransferMode mode)
		{
			RemoteFile = remoteFile;
			LocalFile = localFile;
			Mode = mode;
		}

		#region Public Properties

		/// <summary>
		/// The complete path of the remote file.
		/// </summary>
		public string RemoteFile { get; private set; }

		/// <summary>
		/// The complete path of the local file.
		/// </summary>
		public string LocalFile { get; private set; }

		/// <summary>
		/// The transfer mode of the current request.
		/// </summary>
		public TransferMode Mode { get; private set; }

		#endregion
	}

	/// <summary>
	/// Defines an interface for accessing remote files.
	/// </summary>
	public interface IRemoteFileAccessProvider
	{
		/// <summary>
		/// User Id used to login to remote server.
		/// </summary>
		string UserId { get; set; }

		/// <summary>
		/// Password used to login to remote server.
		/// </summary>
		string Password { get; set; }

		/// <summary>
		/// The base url of the remote server.
		/// </summary>
		string RemoteUrlBase { get; set; }

		/// <summary>
		/// Create a local directory.
		/// </summary>
		/// <param name="localPath"></param>
		void CreateLocalDirectory(string localPath);

		/// <summary>
		/// Create a directory remotely.
		/// </summary>
		/// <param name="remotePath"></param>
		void CreateRemoteDirectory(string remotePath);

		/// <summary>
		/// List the files in the remote path.
		/// </summary>
		/// <param name="remotePath"></param>
		List<string> ListRemoteFiles(string remotePath);

		/// <summary>
		/// Transfer files between local and server.
		/// </summary>
		/// <param name="requests"></param>
		void TransferFiles(List<FileTransferRequest> requests);
	}
}
