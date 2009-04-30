#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services
{
	public class DicomFilePublishingException : Exception
	{
		internal DicomFilePublishingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
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
				string savePath = System.IO.Path.Combine(tempDirectory, file.DataSet[DicomTags.SopInstanceUid] + ".dcm");
				file.Save(savePath);
			}
		}

		public static void PublishLocal(ICollection<DicomFile> files, bool isBackground)
		{
			if (files.Count == 0)
				return;

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
				request.IsBackground = isBackground;
				client.Import(request);
				client.Close();

				AuditHelper.LogImportStudies("Publish Instances", new AuditedInstances(true, tempFileDirectory), EventSource.CurrentProcess, EventResult.Success);
			}
			catch (Exception e)
			{
				client.Abort();
				string message =
					String.Format("Failed to connect to the dicom send service to send files.  The files must be published manually (location: {0})",
						tempFileDirectory);
				throw new DicomFilePublishingException(message, e);
			}
		}

		public static void PublishRemote(ICollection<DicomFile> files, AEInformation destinationServer, bool isBackground)
		{
			if (files.Count == 0)
				return;

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
				request.IsBackground = isBackground;
				client.SendFiles(request);
				client.Close();
			}
			catch (Exception e)
			{
				client.Abort();
				string message =
					String.Format("Failed to connect to the local data store service to import files.  The files must be imported manually (location: {0})",
						tempFileDirectory);
				throw new DicomFilePublishingException(message, e);
			}
		}
	}
}
