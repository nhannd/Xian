using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// This class implements the file transfer using <see cref="System.Net.WebClient"/>.
	/// </summary>
	public class RemoteFileAccessProvider : IRemoteFileAccessProvider
	{
		protected const char RemotePathSeparator = '/';
		protected const char LocalPathSeparator = '\\';

		/// <summary>
		/// Constructor
		/// </summary>
		public RemoteFileAccessProvider(string userId, string password, string remoteUrlBase)
		{
			UserId = userId;
			Password = password;
			RemoteUrlBase = remoteUrlBase == null ? null : remoteUrlBase.TrimEnd(RemotePathSeparator);
		}

		#region IRemoteFileAccess Members

		public string UserId { get; set; }

		public string Password { get; set; }

		public string RemoteUrlBase { get; set; }

		public void CreateLocalDirectory(string localPath)
		{
			if (!Directory.Exists(localPath))
				Directory.CreateDirectory(localPath);
		}

		public virtual void CreateRemoteDirectory(string remotePath)
		{
			throw new NotImplementedException("CreateRemoteDirectory is not implemented");
		}

		public virtual List<string> ListRemoteFiles(string remotePath)
		{
			throw new NotImplementedException("ListRemoteFiles is not implemented");
		}

		public virtual void TransferFiles(List<FileTransferRequest> requests)
		{
			FileTransferRequest requestBeingProcessed = null;

			try
			{
				var client = new WebClient
					{
						Credentials = new NetworkCredential(this.UserId, this.Password)
					};

				CreateRemoteDirectoryFromRequest(requests);
				
				foreach (var request in requests)
				{
					requestBeingProcessed = request;
					if (request.Mode == FileTransferRequest.TransferMode.Download)
					{
						var localFilePath = GetParentPath(request.LocalFile, LocalPathSeparator);
						CreateLocalDirectory(localFilePath);
						client.DownloadFile(request.RemoteFile, request.LocalFile);
					}
					else
					{
						client.UploadFile(request.RemoteFile, request.LocalFile);
					}
				}
			}
			catch (Exception e)
			{
				string message;
				if (requestBeingProcessed == null)
				{
					message = SR.ExceptionFailedToInitializeFileTransfer;
				}
				else
				{
					message = requestBeingProcessed.Mode == FileTransferRequest.TransferMode.Download
						 ? string.Format(SR.ExceptionFailedToTransferFile, requestBeingProcessed.RemoteFile, requestBeingProcessed.LocalFile)
						 : string.Format(SR.ExceptionFailedToTransferFile, requestBeingProcessed.LocalFile, requestBeingProcessed.RemoteFile);
				}

				throw new Exception(message, e);
			}
		}

		#endregion

		#region Helper function

		/// <summary>
		/// Build the relative Url based on an array of strings.
		/// </summary>
		/// <param name="pathSegments"></param>
		/// <returns></returns>
		public static string BuildRelativeUrl(params string[] pathSegments)
		{
			var builder = new StringBuilder();

			foreach (var path in pathSegments)
			{
				builder.Append(path);
				builder.Append(RemotePathSeparator);
			}

			return builder.ToString().TrimEnd(RemotePathSeparator);
		}

		/// <summary>
		/// Find the parent path of a file or path
		/// </summary>
		/// <param name="url"></param>
		public static string GetParentUrl(string url)
		{
			return GetParentPath(url, RemotePathSeparator);
		}

		/// <summary>
		/// Get the full Url of a file by combining the base Url and the specified relative Url.
		/// </summary>
		/// <param name="relativeUrl"></param>
		/// <returns></returns>
		public string GetFullUrl(string relativeUrl)
		{
			return string.Concat(
				this.RemoteUrlBase,
				RemotePathSeparator,
				relativeUrl);
		}

		/// <summary>
		/// Scan through the <see cref="requests"/> and create all the remote directory needed for the upload request
		/// </summary>
		/// <param name="requests">A list of <see cref="FileTransferRequest"/></param>
		protected virtual void CreateRemoteDirectoryFromRequest(IEnumerable<FileTransferRequest> requests)
		{
			// Go through all the remote path in the upload requests, and find all the path that need to be created
			// For example, ftp://localhost/1 and ftp://localhost/1/2 need to be created if the remote file is stored
			// at ftp://localhost/1/2/file.txt
			var pathToBeCreated = new List<string>();
			foreach (var request in requests)
			{
				if (request.Mode == FileTransferRequest.TransferMode.Download)
					continue;

				var path = GetParentUrl(request.RemoteFile);

				// It is assume that the base Url is used
				if (!path.Contains(this.RemoteUrlBase))
					continue;

				if (pathToBeCreated.Contains(path))
					continue;

				// Start from _remoteBaseUrl, find all the parent paths because they will have to be created first
				var partialPath = this.RemoteUrlBase;
				var pathDiff = path.Substring(partialPath.Length);
				var pathTokens = pathDiff.Split(RemotePathSeparator);
				foreach (var token in pathTokens)
				{
					if (string.IsNullOrEmpty(token))
						continue;

					partialPath = string.Concat(partialPath, RemotePathSeparator, token);
					if (pathToBeCreated.Contains(partialPath) == false)
						pathToBeCreated.Add(partialPath);
				}
			}

			// Check each pathToBeCreated to see if they exist remotely
			var pathsNotExist = new List<string>();
			foreach (var path in pathToBeCreated)
			{
				var parentPath = GetParentUrl(path);

				// if parent path doesn't exist, this path can't exist either
				if (pathsNotExist.Contains(parentPath))
				{
					pathsNotExist.Add(path);
					continue;
				}

				// List all subdirectory of the parent path, and check if the current one already exist
				var directories = ListRemoteFiles(parentPath);
				if (!directories.Contains(path))
					pathsNotExist.Add(path);
			}

			// Now we are ready to create the remote paths
			foreach (var path in pathsNotExist)
				CreateRemoteDirectory(path);
		}

		/// <summary>
		/// Find the parent path of a file or path
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pathSeparator"></param>
		protected static string GetParentPath(string path, char pathSeparator)
		{
			path = path.TrimEnd(pathSeparator);
			int index = path.LastIndexOf(pathSeparator);
			return path.Substring(0, index);
		}
	
		#endregion

	}
}
