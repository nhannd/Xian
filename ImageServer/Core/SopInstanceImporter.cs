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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Encapsulates the context of the application when <see cref="SopInstanceImporter"/> is called.
    /// </summary>
    public class SopInstanceImporterContext
    {
        #region Private Members
        private readonly String _contextID;
        private readonly string _sourceAE;
        private readonly ServerPartition _partition;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopInstanceImporterContext"/> to be used
        /// by <see cref="SopInstanceImporter"/> 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sourceAE">Source AE title where the image(s) are imported from</param>
        /// <param name="serverAE">Target AE title where the image(s) will be imported to</param>
        public SopInstanceImporterContext(string id, string sourceAE, string serverAE)
            :
            this(id, sourceAE, ServerPartitionMonitor.Instance.GetPartition(serverAE))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SopInstanceImporterContext"/> to be used
        /// by <see cref="SopInstanceImporter"/> 
        /// </summary>
        /// <param name="contextID">The ID assigned to the context. This will be used as the name of storage folder in case of duplicate.</param>
        /// <param name="sourceAE">Source AE title of the image(s) to be imported</param>
        /// <param name="partition">The <see cref="ServerPartition"/> which the image(s) will be imported to</param>
        public SopInstanceImporterContext(string contextID, string sourceAE, ServerPartition partition)
        {
            Platform.CheckForEmptyString(contextID, "contextID");
            Platform.CheckForNullReference(partition, "partition");
            _contextID = contextID;
            _sourceAE = sourceAE;
            _partition = partition;
        }
        
        #endregion

        /// <summary>
        /// Gets the ID of this context
        /// </summary>
        public string ContextID
        {
            get { return _contextID; }
        }

        /// <summary>
        /// Gets the source AE title where the image(s) are imported from
        /// </summary>
        public string SourceAE
        {
            get { return _sourceAE; }
        }

        /// <summary>
        /// Gets <see cref="ServerPartition"/> where the image(s) will be imported to
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
        }
    }

    /// <summary>
    /// Helper class to import a DICOM image into the system.
    /// </summary>
    /// <remarks>
    /// <see cref="SopInstanceImporter"/> provides a consistent mean of
    /// getting DICOM instances into Image Server. Imported DICOM instances
    /// will be inserted into the <see cref="WorkQueue"/> for processing. Proper checks will be
    /// done if the duplicate object policy is set for the partition. Duplicates will be 
    /// ignored, rejected or inserted into the <see cref="StudyIntegrityQueue"/> for manual intervention.
    /// 
    /// </remarks>
	public class SopInstanceImporter
	{
        #region Private Members
        private readonly SopInstanceImporterContext _context; 
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopInstanceImporter"/> to import DICOM object(s)
        /// into the system.
        /// </summary>
        /// <param name="context">The context of the operation.</param>
        public SopInstanceImporter(SopInstanceImporterContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Imports the specified <see cref="DicomMessageBase"/> object into the system.
        /// The object will be inserted into the <see cref="WorkQueue"/> for processing and
        /// if it's a duplicate, proper checks will be done and depending on the policy, it will be 
        /// ignored, rejected or inserted into the <see cref="StudyIntegrityQueue"/> for manual intervention.
        /// </summary>
        /// <param name="message">The DICOM object to be imported.</param>
        /// <returns>An instance of <see cref="DicomProcessingResult"/> that describes the result of the processing.</returns>
        /// <exception cref="DicomDataException">Thrown when the DICOM object contains invalid data</exception>
        public DicomProcessingResult Import(DicomMessageBase message)
        {
            Platform.CheckForNullReference(message, "message");
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
            String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
            String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
            String accessionNumber = message.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);
        	DicomFile file = null;

			DicomProcessingResult result = new DicomProcessingResult
			                               	{
			                               		Successful = true,
			                               		StudyInstanceUid = studyInstanceUid,
			                               		SeriesInstanceUid = seriesInstanceUid,
			                               		SopInstanceUid = sopInstanceUid,
			                               		AccessionNumber = accessionNumber
			                               	};

        	try
			{
				Validate(message);
			}
			catch (DicomDataException e)
			{
				result.SetError(DicomStatuses.ProcessingFailure, e.Message);
				return result;
			}

            // Use the command processor for rollback capabilities.
            using (ServerCommandProcessor commandProcessor =
                new ServerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
            {
                try
                {
                    string failureMessage;
                    StudyStorageLocation studyLocation = GetWritableOnlineStorage(message);

                    // GetWritableOnlineStorage should throw an exception if the study location cannot be found.
                    Platform.CheckForNullReference(studyLocation, "studyLocation");

                    if (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle)
                        && (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled)))
                    {
                        failureMessage = String.Format("Study {0} on partition {1} is being processed: {2}, can't accept new images.",
                                                       studyLocation.StudyInstanceUid, _context.Partition.Description, studyLocation.QueueStudyStateEnum.Description);
                        result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
                        return result;
                    }
                	if (studyLocation.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
                	{
                		if (studyLocation.IsLatestArchiveLossless)
                		{
                			result.DicomStatus = DicomStatuses.StorageStorageOutOfResources;
                			failureMessage = String.Format("Study {0} on partition {1} can't accept new images due to lossy compression of the study.  Restoring study.",
                			                               studyLocation.StudyInstanceUid, _context.Partition.Description);
                			Platform.Log(LogLevel.Error, failureMessage);
                			if (ServerHelper.InsertRestoreRequest(studyLocation) == null)
                			{
                				Platform.Log(LogLevel.Warn, "Unable to insert Restore Request for Study");
                			}

                			result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
                			return result;
                		}
                	}

                	String path = studyLocation.FilesystemPath;
                	const string extension = null;
                    String finalDest = studyLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);
                    file = ConvertToDicomFile(message, finalDest, _context.SourceAE);

                    if (HasUnprocessedCopy(studyLocation.Key, seriesInstanceUid, sopInstanceUid))
                    {
                        failureMessage = string.Format("Another copy of the SOP Instance was received but has not been processed: {0}", sopInstanceUid);
                        result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
                        return result;
                    }
                	if (File.Exists(finalDest))
                	{
                		result = HandleDuplicate(sopInstanceUid, studyLocation, commandProcessor, file);
                		if (!result.Successful)
                			return result;
                	}
                	else
                	{
                		HandleNonDuplicate(seriesInstanceUid, sopInstanceUid, studyLocation, commandProcessor, file, path, false, extension);
                	}

                	if (commandProcessor.Execute())
                	{
                		result.DicomStatus = DicomStatuses.Success;
                	}
                	else
                	{
                		failureMessage =
                			String.Format("Failure processing message: {0}. Sending failure status.",
                			              commandProcessor.FailureReason);
                		result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
                		// processor already rolled back
                		return result;
                	}
                }
                catch(NoWritableFilesystemException)
                {
                    String failureMessage = String.Format("Unable to process image, no writable filesystem found for Study UID {0}.", sopInstanceUid);
                    commandProcessor.Rollback();
                    result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
                }
                catch(StudyIsNearlineException e)
                {
                    String failureMessage = e.RestoreRequested
                                                ? String.Format("{0}. Restore has been requested.", e.Message)
                                                : e.Message;

                    Platform.Log(LogLevel.Error, failureMessage);
                    commandProcessor.Rollback();
                    result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
                }
                catch (FilesystemNotWritableException)
                {
					commandProcessor.Rollback();

                	string folder;
					if (!FilesystemMonitor.Instance.GetWriteableIncomingFolder(_context.Partition, out folder))
					{
						String failureMessage =
							String.Format("Unable to process image, study storage location is missing or not writeable: {0}.", sopInstanceUid);
						result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
						return result;
					}

                	if (file == null)
                		file = ConvertToDicomFile(message, string.Empty, _context.SourceAE);

                	if (!SaveToFolder(folder, sopInstanceUid, studyInstanceUid, file))
                	{
						String failureMessage =
							String.Format("Study storage location not writeable and no writeable incoming folder: {0}.", sopInstanceUid);
						result.SetError(DicomStatuses.StorageStorageOutOfResources, failureMessage);
						return result;
                	}

                	Platform.Log(LogLevel.Info, "Saved existing SOP without writeable storage location to {0} folder: {1}",
                	             FilesystemMonitor.ImportDirectorySuffix, sopInstanceUid);
					result.DicomStatus = DicomStatuses.Success;
                	return result;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", commandProcessor.Description);
                    commandProcessor.Rollback();
                    result.SetError(result.DicomStatus ?? DicomStatuses.ProcessingFailure, e.Message);
                }
            }

            return result;
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Gets the online storage location for a <see cref="DicomMessageBase"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="StudyIsNearlineException">Thrown if the study is nearline.</exception>
        /// <exception cref="FilesystemNotWritableException">Thrown if the study is online but the filesystem is not writable.</exception>
        /// <exception cref="SopInstanceProcessingException">Thrown ifor other exceptions.</exception>
        private StudyStorageLocation GetWritableOnlineStorage(DicomMessageBase message)
        {
            string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string studyDate = message.DataSet[DicomTags.StudyDate].GetString(0, String.Empty);
            Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

        	StudyStorageLocation studyLocation;

			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				bool created;
				studyLocation = FilesystemMonitor.Instance.GetOrCreateWritableStudyStorageLocation(studyInstanceUid,
				                                                                                   studyDate,
				                                                                                   message.TransferSyntax,
				                                                                                   updateContext,
				                                                                                   _context.Partition,
				                                                                                   out created);
				if (created)
					updateContext.Commit();
			}

        	Platform.CheckForNullReference(studyLocation, "studyLocation");
            return studyLocation;
        }

        private static void HandleNonDuplicate(string seriesInstanceUid, string sopInstanceUid, StudyStorageLocation studyLocation, ServerCommandProcessor commandProcessor, DicomFile file, string path, bool dupImage, string extension)
        {
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, studyLocation.PartitionFolder);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, studyLocation.StudyFolder);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, studyLocation.StudyInstanceUid);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, seriesInstanceUid);
            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, sopInstanceUid);
            path += ServerPlatform.DicomFileExtension;

            commandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

            commandProcessor.AddCommand(
                new UpdateWorkQueueCommand(file, studyLocation, dupImage, extension));

            #region SPECIAL CODE FOR TESTING
            if (Common.Diagnostics.Settings.SimulateFileCorruption)
            {
                commandProcessor.AddCommand(new CorruptDicomFileCommand(path));
            }
            #endregion
        }

		private static bool SaveToFolder(string folder, string sopInstanceUid, string studyInstanceUid, DicomFile file)
		{
			using (ServerCommandProcessor commandProcessor =
				new ServerCommandProcessor(String.Format("Saving Sop Instance to Incoming {0}", sopInstanceUid)))
			{
				string path = Path.Combine(folder, studyInstanceUid);
				commandProcessor.AddCommand(new CreateDirectoryCommand(path));

				path = Path.Combine(path, sopInstanceUid);
				path += ServerPlatform.DicomFileExtension;

				if (File.Exists(path))
					return false;

				commandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

				return commandProcessor.Execute();
			}
		}

    	private DicomProcessingResult HandleDuplicate(string sopInstanceUid, StudyStorageLocation studyLocation, ServerCommandProcessor commandProcessor, DicomFile file)
        {
        	Study study = studyLocation.Study ??
                          studyLocation.LoadStudy(ExecutionContext.Current.PersistenceContext);
            if (study != null)
                Platform.Log(LogLevel.Info, "Received duplicate SOP {0} (A#:{1} StudyUid:{2}  Patient: {3}  ID:{4})",
                             sopInstanceUid,
                             study.AccessionNumber, study.StudyInstanceUid,
                             study.PatientsName, study.PatientId);
            else
                Platform.Log(LogLevel.Info,
                             "Received duplicate SOP {0} (StudyUid:{1}). Existing files haven't been processed.",
                             sopInstanceUid, studyLocation.StudyInstanceUid);

            SopProcessingContext sopProcessingContext = new SopProcessingContext(commandProcessor, studyLocation, _context.ContextID);
            DicomProcessingResult result = DuplicateSopProcessorHelper.Process(sopProcessingContext, file);
            return result;
        }

       
        private static void Validate(DicomMessageBase message)
        {
            DicomSopInstanceValidator validator = new DicomSopInstanceValidator();
            validator.Validate(message);

        }

        static private DicomFile ConvertToDicomFile(DicomMessageBase message, string filename, string sourceAE)
        {
            // This routine sets some of the group 0x0002 elements.
            DicomFile file;
            if (message is DicomFile)
            {
                file = message as DicomFile;
            }
            else if (message is DicomMessage)
            {
                file = new DicomFile(message as DicomMessage, filename);
            }
            else
            {
                throw new NotSupportedException(String.Format("Cannot convert {0} to DicomFile", message.GetType()));
            }

            file.SourceApplicationEntityTitle = sourceAE;
            file.TransferSyntax = message.TransferSyntax.Encapsulated ? 
				message.TransferSyntax : TransferSyntax.ExplicitVrLittleEndian;

            return file;
        }

        static private bool HasUnprocessedCopy(ServerEntityKey storageLocationKey, string seriesUid, string sopUid)
        {
			if (ServerHelper.WorkQueueUidExists(storageLocationKey, seriesUid, sopUid))
				return true;

			return ServerHelper.StudyIntegrityUidExists(storageLocationKey, seriesUid, sopUid);
        }
        
        #endregion
	}


    /// <summary>
	/// Class used for testing purposes to simulate a corrupted file.
	/// </summary>
    internal class CorruptDicomFileCommand : ServerCommand
    {
        private readonly string _path;

        public CorruptDicomFileCommand(string path):base("Corrupt file", false)
        {
            _path = path;
        }

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			Random rand = new Random();

			if (Common.Diagnostics.Settings.SimulateFileCorruption)
			{
				Common.Diagnostics.RandomError.Generate(
					rand.Next()%2 == 0,
					String.Format("Corrupting the file {0}", _path),
					delegate
						{
							FileInfo f = new FileInfo(_path);
							long size = rand.Next(0, (int) f.Length/2);
							if (size <= 0)
							{
								FileStream s = FileStreamOpener.OpenForSoleUpdate(_path, FileMode.Truncate);
								s.Flush();
								s.Close();
							}
							else
							{
								FileStream s = FileStreamOpener.OpenForRead(_path, FileMode.Open);
								byte[] buffer = new byte[size];
								int bytesRead = s.Read(buffer, 0, buffer.Length);
								s.Close();

								s = FileStreamOpener.OpenForSoleUpdate(_path, FileMode.Truncate);
								s.Write(buffer, 0, bytesRead);
								s.Flush();
								s.Close();
							}
						}
					);
			}
		}

    	protected override void OnUndo()
        {
            
        }
    }
}