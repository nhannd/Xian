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
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Abstract class for handling Storage SCP connections.
    /// </summary>
    /// <remarks>
    /// <para>This class is an abstract base class for ImageServer plugins that process DICOM C-STORE
    /// request messages.  The class implements a number of common methods needed for SCP handlers.
    /// The class also implements the <see cref="IDicomScp{TContext}"/> interface.</para>
    /// </remarks>
    public abstract class StorageScp : BaseScp
    {
        #region Abstract Properties
        public abstract string StorageScpType { get; }
        #endregion

        #region Protected Members

        /// <summary>
        /// Converts a <see cref="DicomMessage"/> instance into a <see cref="DicomFile"/>.
        /// </summary>
        /// <remarks>This routine sets the Source AE title, </remarks>
        /// <param name="message"></param>
        /// <param name="filename"></param>
        /// <param name="assocParms"></param>
        /// <returns></returns>
        protected static DicomFile ConvertToDicomFile(DicomMessage message, string filename, AssociationParameters assocParms)
        {
            // This routine sets some of the group 0x0002 elements.
            DicomFile file = new DicomFile(message, filename);

            file.SourceApplicationEntityTitle = assocParms.CallingAE;
            if (message.TransferSyntax.Encapsulated)
                file.TransferSyntax = message.TransferSyntax;
            else
                file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            return file;
        }

        /// <summary>
        /// Load from the database the configured transfer syntaxes
        /// </summary>
        /// <param name="read">a Read context</param>
        /// <param name="partitionKey">The partition to retrieve the transfer syntaxes for</param>
        /// <param name="encapsulated">true if searching for encapsulated syntaxes only</param>
        /// <returns>The list of syntaxes</returns>
		protected static IList<PartitionTransferSyntax> LoadTransferSyntaxes(IReadContext read, ServerEntityKey partitionKey, bool encapsulated)
        {
            IList<PartitionTransferSyntax> list;

			IQueryServerPartitionTransferSyntaxes broker = read.GetBroker<IQueryServerPartitionTransferSyntaxes>();

			PartitionTransferSyntaxQueryParameters criteria = new PartitionTransferSyntaxQueryParameters();

            criteria.ServerPartitionKey = partitionKey;

            list = broker.Find(criteria);

			List<PartitionTransferSyntax> returnList = new List<PartitionTransferSyntax>();
			foreach (PartitionTransferSyntax syntax in list)
            {
				if (!syntax.Enabled) continue;

                TransferSyntax dicomSyntax = TransferSyntax.GetTransferSyntax(syntax.Uid);
                if (dicomSyntax.Encapsulated == encapsulated)
                    returnList.Add(syntax);
            }
            return returnList;
        }
        #endregion

        #region Private Methods

        private void RaiseNoStorageAlert()
        {
            ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical,
                                SR.AlertComponentDICOM, AlertTypeCodes.NoResources,
                                TimeSpan.FromMinutes(15), // don't repeat this alert again for another 15 min
                                SR.AlertNoWritableStorage,
                                Partition.AeTitle
                    );
        }
        #endregion

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {

            if (!Device.AllowStorage)
            {
                return DicomPresContextResult.RejectUser;
            }

            return DicomPresContextResult.Accept;

        }

        #endregion Overridden BaseSCP methods

        #region IDicomScp Members

        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing " + StorageScpType);
            DicomStatus returnStatus = DicomStatuses.Success;
            try
            {
                String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
                String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
                String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, "");

				if (studyInstanceUid.Length > 64 || seriesInstanceUid.Length > 64 || sopInstanceUid.Length > 64)
				{
					returnStatus = DicomStatuses.AttributeValueOutOfRange;
					string failureMessage;
					if (studyInstanceUid.Length > 64)
						failureMessage = string.Format("Study Instance UID is > 64 bytes in C-STORE Message: {0}",studyInstanceUid);
					else if (seriesInstanceUid.Length > 64)
						failureMessage = string.Format("Series Instance UID is > 64 bytes in C-STORE Message: {0}", seriesInstanceUid);
					else
						failureMessage = string.Format("SOP Instance UID is > 64 bytes in C-STORE Message: {0}", sopInstanceUid);
						
					Platform.Log(LogLevel.Error, failureMessage);
					throw new ApplicationException(failureMessage);
				}

                StudyStorageLocation studyLocation = GetStudyStorageLocation(message);
                if (studyLocation == null)
                {
					StudyStorage storage = StudyStorage.Load(Partition.Key,studyInstanceUid);
					if (storage != null)
					{
						returnStatus = DicomStatuses.ResourceLimitation;
						string failureMessage = String.Format("Study {0} on partition {1} is in a Nearline state, can't accept new images.  Inserting Restore Request for Study.", studyInstanceUid, Partition.Description);
						Platform.Log(LogLevel.Error, failureMessage);
						InsertRestore(storage.Key);
						throw new ApplicationException(failureMessage);
					}

                    returnStatus = DicomStatuses.ResourceLimitation;
                    Platform.Log(LogLevel.Error, "Unable to process image, no writeable storage location: {0}", sopInstanceUid);

                    RaiseNoStorageAlert();

                    throw new ApplicationException("No writeable storage location.");
                }


				if (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle)
					&& (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled)))
				{
					returnStatus = DicomStatuses.ResourceLimitation;
					string failureMessage =
						String.Format("Study {0} on partition {1} is being processed: {2}, can't accept new images.",
						              studyLocation.StudyInstanceUid, Partition.Description, studyLocation.QueueStudyStateEnum.Description);
					Platform.Log(LogLevel.Error, failureMessage);
					throw new ApplicationException(failureMessage);
				}
				else if (studyLocation.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
				{
					ArchiveStudyStorage archiveLocation = StudyStorageLocation.GetArchiveLocation(studyLocation.Key);
					if (archiveLocation != null && archiveLocation.ServerTransferSyntax.Lossless)
					{
						returnStatus = DicomStatuses.ResourceLimitation;
						string failureMessage =
							String.Format("Study {0} on partition {1} can't accept new images due to lossy compression of the study.  Restoring study.",
							              studyLocation.StudyInstanceUid, Partition.Description);
						Platform.Log(LogLevel.Error, failureMessage);
						InsertRestore(studyLocation.Key);
						throw new ApplicationException(failureMessage);
					}
				}

                String path = studyLocation.FilesystemPath;
                String dupPath = null;
                String basePath;
                bool dupImage = false;
                string extension = null;

                processor.AddCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, studyLocation.PartitionFolder);
                processor.AddCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, studyLocation.StudyFolder);
                processor.AddCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, studyLocation.StudyInstanceUid);
                processor.AddCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, seriesInstanceUid);
                processor.AddCommand(new CreateDirectoryCommand(path));

                basePath = path = Path.Combine(path, sopInstanceUid );
                path += ".dcm";

                if (File.Exists(path))
                {
                    if (Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
                    {
                        returnStatus = DicomStatuses.Success;

                        Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);

                        // Send the response message
                        server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, returnStatus);

                        return true;
                    }
                    else if (Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
                    {
                        returnStatus = DicomStatuses.DuplicateSOPInstance;
                        Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                        throw new ApplicationException("Duplicate SOP Instance received.");
                    }
                    else if (Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.AcceptLatest))
                    {
                    	TransferSyntax studyStorageSyntax =
                    		TransferSyntax.GetTransferSyntax(studyLocation.TransferSyntaxUid);
						if (!studyStorageSyntax.UidString.Equals(message.TransferSyntax.UidString)
							&& (message.TransferSyntax.Encapsulated || studyStorageSyntax.Encapsulated))
						{
							returnStatus = DicomStatuses.Success;
							Platform.Log(LogLevel.Error,
							             "Duplicate SOP recived, but transfer syntax has changed, db syntax: {0}, message syntax: {1}",
							             studyLocation.TransferSyntaxUid, message.TransferSyntax.Name);
							server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, returnStatus);
							return true;
						}
						else
						{
							Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, replacing previous file {0}", sopInstanceUid);
							dupPath = path + "_dup_old";
							processor.AddCommand(new RenameFileCommand(path, dupPath));
							dupImage = true;
						}
                    }
                    else if (Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
                    {
                        Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, processing {0}", sopInstanceUid);
                        for (int i = 1; i < 999; i++)
                        {
                            extension = String.Format("dup{0}.dcm", i);
                            string newPath = basePath + "." + extension;
                            if (!File.Exists(newPath))
                            {
                                dupImage = true;
                                path = newPath;
                                break;
                            }
                        }
                    }
                }

                DicomFile file = ConvertToDicomFile(message, path, association);
                processor.AddCommand(new SaveDicomFileCommand(path, file));

                processor.AddCommand(new UpdateWorkQueueCommand(file, studyLocation, dupImage, extension));

                if (processor.Execute())
                {
                    Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} on context {3}", sopInstanceUid,
                                 association.CallingAE, association.CalledAE, presentationID);
                    returnStatus = DicomStatuses.Success;
                    if (dupPath != null)
                    {
                        try
                        {
                            // Don't want to rollback here, just leave the file "as is"
                            File.Delete(dupPath);
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Warn, e, "Unexpectedly unable to remove duplicate file: {0}", dupPath);
                        }
                    }
                }
                else
                {
                    Platform.Log(LogLevel.Error, "Failure processing message, sending failure status.");

                    returnStatus = DicomStatuses.ProcessingFailure;
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", processor.Description);
                processor.Rollback();
                if (returnStatus == DicomStatuses.Success)
                    returnStatus = DicomStatuses.ProcessingFailure;
            }

            // Send the response message
            server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, returnStatus);

            return true;
        }

        #endregion
    }
}