#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Common
{
	public class DicomFilePublishingException : Exception
	{
		internal DicomFilePublishingException(string message, Exception innerException)
			: base(message, innerException) {}
	}

	//TODO: Instead of static helper, could be part of 2 separate bridges (IDicomSendBridge and ILocalDataStorebridge).
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
					Platform.Log(LogLevel.Info, e, "Failed to delete old temp directory ({0}).", subDirectory);
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
			DateTime dateTime = DateTime.Now;

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
			tempDirectory = Path.Combine(Path.GetTempPath(), "ClearCanvas");
			tempDirectory = Path.Combine(tempDirectory, "Publishing");

			DeleteEmptyFolders(tempDirectory);
			tempDirectory = GetTempDirectory(tempDirectory, tempDirectoryPrefix);
			Directory.CreateDirectory(tempDirectory);

			foreach (DicomFile file in files)
			{
				string savePath = Path.Combine(tempDirectory, file.DataSet[DicomTags.SopInstanceUid] + ".dcm");
				file.Save(savePath);
			}
		}

		private static AuditedInstances GetAuditedInstances(IEnumerable<DicomFile> files, bool includeFilename)
		{
			var instances = new AuditedInstances();
			foreach (var file in files)
			{
				var patientId = file.DataSet[DicomTags.PatientId].ToString();
				var patientsName = file.DataSet[DicomTags.PatientsName].ToString();
				var studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
				instances.AddInstance(patientId, patientsName, studyInstanceUid, includeFilename ? file.Filename : null);
			}
			return instances;
		}

		public static void PublishLocal(ICollection<DicomFile> files, bool isBackground)
		{
			if (files == null || files.Count == 0)
				return;

			// cache files to temporary storage
			string tempFileDirectory;
			SaveFiles(files, @"Local", out tempFileDirectory);

			// setup auditing information
			var result = EventResult.Success;
			var auditedInstances = GetAuditedInstances(files, true);

			var client = new LocalDataStoreServiceClient();
			try
			{
				var request = new FileImportRequest();
				request.FilePaths = new[] {tempFileDirectory};
				request.Recursive = true;
				request.FileImportBehaviour = FileImportBehaviour.Move;
				request.IsBackground = isBackground;

				client.Open();
				client.Import(request);
				client.Close();
			}
			catch (Exception ex)
			{
				client.Abort();
				result = EventResult.MajorFailure;

				var message = string.Format("Failed to connect to the local data store service to import files.  The files must be imported manually (location: {0})", tempFileDirectory);
				throw new DicomFilePublishingException(message, ex);
			}
			finally
			{
				// audit attempt to import these instances to local store
				AuditHelper.LogImportStudies(auditedInstances, EventSource.CurrentUser, result);
			}
		}

		public static void PublishRemote(ICollection<DicomFile> files, AEInformation destinationServer, bool isBackground)
		{
			if (files == null || files.Count == 0)
				return;

			// cache files to temporary storage
			string tempFileDirectory;
			SaveFiles(files, destinationServer.AETitle, out tempFileDirectory);

			// setup auditing information
			var result = EventResult.Success;
			var auditedInstances = GetAuditedInstances(files, false);

			var client = new DicomSendServiceClient();
			try
			{
				var request = new SendFilesRequest();
				request.FilePaths = new[] {tempFileDirectory};
				request.FileExtensions = new string[0];
				request.Recursive = true;
				request.DeletionBehaviour = DeletionBehaviour.DeleteOnSuccess;
				request.DestinationAEInformation = destinationServer;
				request.IsBackground = isBackground;

				client.Open();
				client.SendFiles(request);
				client.Close();
			}
			catch (Exception ex)
			{
				client.Abort();
				result = EventResult.MajorFailure;

				var message = string.Format("Failed to connect to the dicom send service to send files.  The files must be published manually (location: {0})", tempFileDirectory);
				throw new DicomFilePublishingException(message, ex);
			}
			finally
			{
				// audit attempt to update instances on remote server
				AuditHelper.LogUpdateInstances(new[] {destinationServer.AETitle}, auditedInstances, EventSource.CurrentUser, result);
			}
		}
	}
}