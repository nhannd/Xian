using System;
using System.Net;
using System.IO;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Provide access to remote files with Http scheme.
	/// </summary>
	/// <remarks>
	/// This provider class does not create remote directory before uploading files.
	/// </remarks>
	public class HttpFileTransfer : IRemoteFileTransfer
	{
		private readonly string _userId;
		private readonly string _password;

		/// <summary>
		/// Default constructor with no authentication.
		/// </summary>
		public HttpFileTransfer()
		{
		}

		/// <summary>
		/// Constructor with authentication provided.
		/// </summary>
		public HttpFileTransfer(string userId, string password)
		{
			_userId = userId;
			_password = password;
		}

		/// <summary>
		/// Upload one file from local to remote.
		/// </summary>
		/// <param name="request"></param>
		/// <remarks>
		/// The remote directories are not created before uploading files.
		/// </remarks>
		public void Upload(FileTransferRequest request)
		{
			try
			{
				using (var webClient = new WebClient())
				{
					if (!string.IsNullOrEmpty(_userId) && !string.IsNullOrEmpty(_password))
						webClient.Credentials = new NetworkCredential(_userId, _password);

					webClient.UploadFile(request.RemoteFile, request.LocalFile);
				}
			}
			catch (Exception e)
			{
				//TODO (cr Oct 2009): we're not supposed to use SR for exception messages.
				//Throw a different type of exception and use an ExceptionHandler if it's supposed to be a user message.
				var message = string.Format(SR.ExceptionFailedToTransferFile, request.RemoteFile, request.LocalFile);
				throw new Exception(message, e);
			}
		}

		/// <summary>
		/// Download one file from remote to local
		/// </summary>
		/// <param name="request"></param>
		public void Download(FileTransferRequest request)
		{
			try
			{
				using (var webClient = new WebClient())
				{
					if (!string.IsNullOrEmpty(_userId) && !string.IsNullOrEmpty(_password))
						webClient.Credentials = new NetworkCredential(_userId, _password);

					var downloadDirectory = Path.GetDirectoryName(request.LocalFile);
					if (!Directory.Exists(downloadDirectory))
						Directory.CreateDirectory(downloadDirectory);

					webClient.DownloadFile(request.RemoteFile, request.LocalFile);
				}
			}
			catch (Exception e)
			{
				//TODO (cr Oct 2009): we're not supposed to use SR for exception messages.
				//Throw a different type of exception and use an ExceptionHandler if it's supposed to be a user message.
				var message = string.Format(SR.ExceptionFailedToTransferFile, request.RemoteFile, request.LocalFile);
				throw new Exception(message, e);
			}
		}
	}
}
