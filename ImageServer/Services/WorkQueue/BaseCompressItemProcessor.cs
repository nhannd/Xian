#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
	public abstract class BaseCompressItemProcessor : BaseItemProcessor
	{
		private ServerRulesEngine _compressionRulesEngine;

		protected ServerRulesEngine CompressionRulesEngine
		{
			get { return _compressionRulesEngine; }
			set { _compressionRulesEngine = value; }
		}

		private bool ProcessWorkQueueUid(Model.WorkQueue item, WorkQueueUid sop, StudyXml studyXml)
		{
			Platform.CheckForNullReference(item, "item");
			Platform.CheckForNullReference(sop, "sop");
			Platform.CheckForNullReference(studyXml, "studyXml");

			string basePath = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
			basePath = Path.Combine(basePath, sop.SopInstanceUid);
			string path;
			if (sop.Extension != null)
				path = basePath + "." + sop.Extension;
			else
				path = basePath + ".dcm";

			try
			{
				ProcessFile(item, sop, path, studyXml);

				// Delete it out of the queue
				DeleteWorkQueueUid(sop);
				return true;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when compressing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
				if (e.InnerException != null)
					item.FailureDescription = e.InnerException.Message;
				else
					item.FailureDescription = e.Message;

				sop.FailureCount++;

				UpdateWorkQueueUid(sop);

				return false;
			}

		}
		/// <summary>
		/// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
		/// </summary>
		/// <param name="item">The <see cref="WorkQueue"/> item.</param>
		/// <returns>A value indicating whether the uid list has been successfully processed</returns>
		protected bool ProcessUidList(Model.WorkQueue item)
		{
			StudyXml studyXml;

			studyXml = LoadStudyXml(StorageLocation);

			int successfulProcessCount = 0;

			foreach (WorkQueueUid sop in WorkQueueUidList)
			{
				if (sop.Failed)
					continue;

				if (ProcessWorkQueueUid(item, sop, studyXml))
					successfulProcessCount++;
			}

			//TODO: Should we return true only if ALL uids have been processed instead?
			return successfulProcessCount > 0;

		}

		protected void ProcessFile(Model.WorkQueue item, WorkQueueUid sop, string path, StudyXml studyXml)
		{
			DicomFile file;
		
			// Use the command processor for rollback capabilities.
			ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue Compress DICOM File");

			try
			{
				file = new DicomFile(path);
				file.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);

				// Get the Patients Name for processing purposes.
				String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");
		
				// Update the StudyStream object
				UpdateStudyXmlCommand insertStudyXmlCommand = new UpdateStudyXmlCommand(file, studyXml, StorageLocation);
				processor.AddCommand(insertStudyXmlCommand);

				// Create a context for applying actions from the rules engine
				ServerActionContext context =
					new ServerActionContext(file, StorageLocation.FilesystemKey, item.ServerPartitionKey, item.StudyStorageKey);
				context.CommandProcessor = processor;

				// Run the rules engine against the object.
				CompressionRulesEngine.Execute(context);

				if (processor.CommandCount == 1)
				{
					Platform.Log(LogLevel.Info,"No compression defined for object.");
					return;
				}

				RenameFileCommand rename = new RenameFileCommand(file.Filename, path + "_save");
				processor.AddCommand(rename);

				SaveDicomFileCommand save = new SaveDicomFileCommand(file.Filename, file);
				processor.AddCommand(save);

				FileDeleteCommand delete = new FileDeleteCommand(path + "_save", false);
				processor.AddCommand(delete);

				// Do the actual processing
				if (!processor.Execute())
				{
					Platform.Log(LogLevel.Error, "Failure compressing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
					throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid);
				}
				else
				{
					Platform.Log(LogLevel.Info, "Compress SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid,
								 patientsName);

				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
							 processor.Description);
				processor.Rollback();
				throw new ApplicationException("Unexpected exception when compressing file.", e);
			}	
		}
	}
}
