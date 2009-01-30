using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services
{
	public static class DicomFilePublisher
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
					FileInfo[] files = subDirectory.GetFiles();
					if (files == null || files.Length == 0)
						subDirectory.Delete();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Info, e, "Failed to delete old temp directory ({0})", subDirectory);
				}
			}
		}

		private static string FixTempDirectoryPrefix(string prefix)
		{
			//replace directory separators with spaces.
			prefix = prefix.Replace('\\', ' ');
			prefix = prefix.Replace('/', ' ');

			//Replace the wildcard characters as well.
			prefix = prefix.Replace('*', ' ');
			prefix = prefix.Replace('?', ' ');

			char[] invalidChars = Path.GetInvalidPathChars();
			foreach (char invalidChar in invalidChars)
				prefix = prefix.Replace(invalidChar, ' '); //replace invalid characters with spaces.

			return prefix;
		}

		private static string GetTempDirectory(string path, string prefix)
		{
			prefix = FixTempDirectoryPrefix(prefix);
			DateTime dateTime = Platform.Time;

			int number = 0;
			string tempDirectory;
			do
			{
				tempDirectory = String.Format("{0}.{1}.{2}", prefix, dateTime.Date.ToString("yyyyMMdd"), ++number);
				tempDirectory = Path.Combine(path, tempDirectory);

			} while (Directory.Exists(tempDirectory));

			return tempDirectory;
		}

		private static void SaveFiles(IEnumerable<DicomFile> files, string tempDirectoryPrefix, out string tempDirectory)
		{
			tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ClearCanvas");
			tempDirectory = System.IO.Path.Combine(tempDirectory, "Publishing");

			DeleteEmptyFolders(tempDirectory);
			tempDirectory = GetTempDirectory(tempDirectory, tempDirectoryPrefix);
			Directory.CreateDirectory(tempDirectory);

			foreach (DicomFile file in files)
			{
				string savePath = System.IO.Path.Combine(tempDirectory, file.DataSet[DicomTags.SopInstanceUid]);
				savePath = System.IO.Path.ChangeExtension(savePath, ".dcm");
				file.Save(savePath);
			}
		}

		public static void PublishLocal(IEnumerable<DicomFile> files)
		{
			//TODO: clean up files/directory when there's no connection?
			string tempFileDirectory;
			SaveFiles(files, "Local", out tempFileDirectory);

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

		public static void PublishRemote(IEnumerable<DicomFile> files, AEInformation destinationServer)
		{
			//TODO: clean up files/directory when there's no connection?
			string tempFileDirectory;
			SaveFiles(files, destinationServer.AETitle , out tempFileDirectory);

			DicomSendServiceClient client = new DicomSendServiceClient();
			try
			{
				client.Open();
				SendFilesRequest request = new SendFilesRequest();
				request.FilePaths = new string[] { tempFileDirectory };
				request.FileExtensions = new string[0];
				request.Recursive = true;
				request.DeletionBehaviour = DeletionBehaviour.DeleteOnSuccess;
				request.DestinationAEInformation = destinationServer;
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
