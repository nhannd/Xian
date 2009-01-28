using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Services
{
	public static class DicomPublishingHelper
	{
		private static void DeleteEmptyFolders(string directory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(directory);
			if (!directoryInfo.Exists)
				return;

			foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
			{
				try
				{
					DateTime now = Platform.Time.ToUniversalTime();
					if (now.Subtract(subDirectory.CreationTimeUtc) < TimeSpan.FromHours(12))
					{
						FileInfo[] files = subDirectory.GetFiles();
						if (files == null || files.Length == 0)
							subDirectory.Delete();
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Info, e, "Failed to delete old temp directory ({0})", subDirectory);
				}
			}
		}

		private static void SaveFiles(IEnumerable<DicomFile> files, out string tempFileDirectory)
		{
			tempFileDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ClearCanvas");
			DeleteEmptyFolders(tempFileDirectory);

			tempFileDirectory = System.IO.Path.Combine(tempFileDirectory, System.IO.Path.GetRandomFileName());
			Directory.CreateDirectory(tempFileDirectory);

			foreach (DicomFile file in files)
			{
				string savePath = System.IO.Path.Combine(tempFileDirectory, file.DataSet[DicomTags.SopInstanceUid]);
				savePath = System.IO.Path.ChangeExtension(savePath, ".dcm");
				file.Save(savePath);
			}
		}

		public static void PublishLocal(IEnumerable<DicomFile> files)
		{
			string tempFileDirectory;
			SaveFiles(files, out tempFileDirectory);

			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();
				FileImportRequest request = new FileImportRequest();
				request.FilePaths = new string[] { tempFileDirectory };
				request.Recursive = true;
				request.FileImportBehaviour = FileImportBehaviour.Move;
				client.Import(request);
				client.Close();
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

		public static void PublishRemote(IEnumerable<DicomFile> files, IEnumerable<AEInformation> destinationServers)
		{
			string tempFileDirectory;
			SaveFiles(files, out tempFileDirectory);

			DicomServerServiceClient client = new DicomServerServiceClient();
			try
			{
				client.Open();
				SendFilesRequest request = new SendFilesRequest();
				request.Behaviour = SendFileBehaviour.DeleteOnSuccess;
				request.FilePaths = new string[] { tempFileDirectory };
				request.FileExtensions = new string[0];
				request.Recursive = true;

				foreach (AEInformation destinationServer in destinationServers)
				{
					request.DestinationAEInformation = destinationServer;
					client.SendFiles(request);
				}

				client.SendFiles(request);
				client.Close();
			}
			catch
			{
				client.Abort();
				throw;
			}
		}
	}
}
