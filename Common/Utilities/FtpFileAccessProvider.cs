using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// This file access remote files using the FTP protocol.
	/// </summary>
	public class FtpFileAccessProvider : RemoteFileAccessProvider
	{
		private readonly bool _usePassive;

		/// <summary>
		/// Constructor
		/// </summary>
		public FtpFileAccessProvider(string userId, string password, string remoteUrlBase, bool usePassive)
			: base(userId, password, remoteUrlBase)
		{
			_usePassive = usePassive;
		}

		#region Overrides

		public override void CreateRemoteDirectory(string remotePath)
		{
			try
			{
				var ftpRequest = (FtpWebRequest)WebRequest.Create(remotePath);
				ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
				ftpRequest.UseBinary = true;
				ftpRequest.UsePassive = _usePassive;
				ftpRequest.Credentials = new NetworkCredential(this.UserId, this.Password);

				var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
				ftpResponse.Close();
			}
			catch (Exception)
			{
				// Purposely swallowing exception here because the remote folders may have existed already because of race condition.
				// For example, two users creating the folders at a similar time.
				// But this is okay because if there is a real problem creating folders, 
				// another exception will be thrown when uploading files
			}
		}

		public override List<string> ListRemoteFiles(string remotePath)
		{
			var remoteFiles = new List<string>();

			var ftpRequest = (FtpWebRequest) WebRequest.Create(remotePath);
			ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
			ftpRequest.UseBinary = true;
			ftpRequest.UsePassive = _usePassive;
			ftpRequest.Credentials = new NetworkCredential(this.UserId, this.Password);

			var ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();
			var ftpResponseReader = new StreamReader(ftpResponse.GetResponseStream());

			var remoteFileName = ftpResponseReader.ReadLine();
			while (!string.IsNullOrEmpty(remoteFileName))
			{
				remoteFiles.Add(remoteFileName);
				remoteFileName = ftpResponseReader.ReadLine();
			}

			ftpResponseReader.Close();
			ftpResponse.Close();

			return remoteFiles;
		}

		public override void TransferFiles(List<FileTransferRequest> requests)
		{
			FileTransferRequest requestBeingProcessed = null;

			try
			{
				// create all the directories at the destination before processing the actual file transfer
				CreateRemoteDirectoryFromRequest(requests);

				foreach (var request in requests)
				{
					requestBeingProcessed = request;
					if (request.Mode == FileTransferRequest.TransferMode.Download)
					{
						var path = GetParentPath(request.LocalFile, LocalPathSeparator);
						CreateLocalDirectory(path);

						Download(request);
					}
					else
					{
						Upload(request);
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

		private void Upload(FileTransferRequest request)
		{
			var localFileInf = new FileInfo(request.LocalFile);
			var ftpRequest = (FtpWebRequest)WebRequest.Create(request.RemoteFile);
			ftpRequest.Credentials = new NetworkCredential(this.UserId, this.Password);
			ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
			ftpRequest.UseBinary = true;
			ftpRequest.UsePassive = _usePassive;
			ftpRequest.ContentLength = localFileInf.Length;

			const int bufferLength = 2048;
			var buffer = new byte[bufferLength];

			FileStream localFileStream = null;
			Stream ftpRequestStream = null;

			try
			{
				localFileStream = localFileInf.OpenRead();
				ftpRequestStream = ftpRequest.GetRequestStream();

				// Read from the file stream a packet at a time till Stream content ends
				var localFileContentLength = localFileStream.Read(buffer, 0, bufferLength);
				while (localFileContentLength != 0)
				{
					// Write Content from the file stream to the FTP Upload Stream
					ftpRequestStream.Write(buffer, 0, localFileContentLength);
					localFileContentLength = localFileStream.Read(buffer, 0, bufferLength);
				}
			}
			finally
			{
				if (ftpRequestStream != null)
					ftpRequestStream.Close();

				if (localFileStream != null)
					localFileStream.Close();
			}
		}

		private void Download(FileTransferRequest request)
		{
			var ftpRequest = (FtpWebRequest)WebRequest.Create(request.RemoteFile);
			ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
			ftpRequest.UseBinary = true;
			ftpRequest.UsePassive = _usePassive;
			ftpRequest.Credentials = new NetworkCredential(this.UserId, this.Password);

			const int bufferSize = 2048;
			var buffer = new byte[bufferSize];

			FtpWebResponse ftpResponse = null;
			Stream ftpResponseStream = null;
			FileStream localFileStream = null;

			try
			{
				ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
				ftpResponseStream = ftpResponse.GetResponseStream();
				localFileStream = new FileStream(request.LocalFile, FileMode.Create);

				var readCount = ftpResponseStream.Read(buffer, 0, bufferSize);
				while (readCount > 0)
				{
					localFileStream.Write(buffer, 0, readCount);
					readCount = ftpResponseStream.Read(buffer, 0, bufferSize);
				}
			}
			finally
			{
				if (ftpResponseStream != null)
					ftpResponseStream.Close();

				if (localFileStream != null)
					localFileStream.Close();
				
				if (ftpResponse != null)
					ftpResponse.Close();
			}
		}
	}
}