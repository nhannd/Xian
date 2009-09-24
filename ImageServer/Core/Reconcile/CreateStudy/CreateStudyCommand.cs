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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Reconcile.CreateStudy
{
	/// <summary>
	/// Command for reconciling images by creating a new study or merge into an existing study.
	/// </summary>
	class CreateStudyCommand : ReconcileCommandBase
	{
		internal class CommandParameters
		{
			public List<BaseImageLevelUpdateCommand> Commands;
		}

		private readonly CommandParameters _parameters;

	    #region Constructors

		/// <summary>
		/// Create an instance of <see cref="CreateStudyCommand"/>
		/// </summary>
		public CreateStudyCommand(ReconcileStudyProcessorContext context, CommandParameters parameters)
			: base("Create Study", true, context)
		{
			_parameters = parameters;
		}

		private CommandParameters Parameters
		{
			get { return _parameters; }
		}

		#endregion

		#region Overriden Protected Methods
		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");
			Platform.CheckForNullReference(Context.WorkQueueItem, "Context.WorkQueueItem");
			Platform.CheckForNullReference(Context.WorkQueueUidList, "Context.WorkQueueUidList");

            try
            {

                LoadUidMappings();

                PrintChangeList();
                ProcessUidList();
            }
            finally
            {
                UpdateHistory();
            }
		}

	    private void ApplySeriesStudyRules()
	    {
            // Run Study / Series Rules Engine.
            StudyRulesEngine engine = new StudyRulesEngine(Context.DestStorageLocation, Context.Partition);
            engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);
	    }


	    protected override void OnUndo()
		{

		}


		#endregion

		#region Private Methods

		private void PrintChangeList()
		{
			StringBuilder log = new StringBuilder();
			log.AppendFormat("Applying following changes to images:\n");
			foreach (BaseImageLevelUpdateCommand cmd in Parameters.Commands)
			{
				log.AppendFormat("{0}", cmd);
				log.AppendLine();
			}

			Platform.Log(LogLevel.Info, log);
		}

        //private void ProcessUidListOld()
        //{
        //    int counter = 0;
        //    Platform.Log(LogLevel.Info, "Populating images into study folder.. {0} to go", Context.WorkQueueUidList.Count);		

        //    foreach (WorkQueueUid uid in Context.WorkQueueUidList)
        //    {
        //        string imagePath = GetReconcileUidPath(uid);
        //        DicomFile file = new DicomFile(imagePath);
        //        file.Load();
        //        string oldSeriesUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
        //        try
        //        {
        //            using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
        //            {
        //                // Assign new series and instance uid
        //                string newSeriesUid;
        //                if (_uidMapper.SeriesMap.ContainsKey(oldSeriesUid))
        //                    newSeriesUid = _uidMapper.SeriesMap[oldSeriesUid].NewSeriesUid;
        //                else
        //                {
        //                    newSeriesUid = DicomUid.GenerateUid().UID;
        //                    _uidMapper.SeriesMap.Add(oldSeriesUid, new SeriesMapping(oldSeriesUid,newSeriesUid));
        //                }

        //                string newSopInstanceUid = DicomUid.GenerateUid().UID;
        //                file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesUid);
        //                file.DataSet[DicomTags.SopInstanceUid].SetStringValue(newSopInstanceUid);
        //                file.MediaStorageSopInstanceUid = newSopInstanceUid;
        //                foreach (BaseImageLevelUpdateCommand command in Parameters.Commands)
        //                {
        //                    command.File = file;
        //                    processor.AddCommand(command);
        //                }

        //                if (Context.DestStorageLocation == null)
        //                {
        //                    processor.AddCommand(new InitializeStorageCommand(Context, file));
        //                }
                        
        //                processor.AddCommand(new SaveFileCommand(Context, file));
        //                InsertWorkQueueCommand.CommandParameters parameters = new InsertWorkQueueCommand.CommandParameters();
        //                parameters.SeriesInstanceUid = newSeriesUid;
        //                parameters.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
        //                parameters.Extension = "dcm";
        //                parameters.IsDuplicate = false;
        //                processor.AddCommand(new InsertWorkQueueCommand(Context, parameters));
        //                processor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));
        //                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

        //                if (counter == 0)
        //                {
        //                    // Only update the first time through the loop
        //                    processor.AddCommand(new UpdateHistoryCommand(Context));
        //                }

        //                if (!processor.Execute())
        //                {
        //                    if (processor.FailureException is InstanceAlreadyExistsException)
        //                    {
        //                        throw processor.FailureException;
        //                    }
        //                    else
        //                    {
        //                        FailUid(uid, true);
        //                        throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason), processor.FailureException);
        //                    }                             
        //                }
        //            }

        //            counter++;
        //            Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled SOP {0} (not yet processed) [{1} of {2}]",
        //                         uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
        //        }
        //        catch (InstanceAlreadyExistsException)
        //        {
        //            CreateWorkQueueEntryForDuplicate(file, Context.WorkQueueItem, uid, Context.DestStorageLocation.ServerPartition);
        //        }
        //    }
        //}

		private void ProcessUidList()
		{
			int counter = 0;
			Platform.Log(LogLevel.Info, "Populating images into study folder.. {0} to go", Context.WorkQueueUidList.Count);

			// Create or Load the Destination StorageLocation.  Note that this may cause 
			// issues with orphan StorageLocations if none of the images below are actually
			// processed.  This must be done becasue the SopInstanceProcessor requires the
			// destination location.
			CreateDestinationStudyStorage();

			StudyProcessorContext context = new StudyProcessorContext(Context.DestStorageLocation);

			// Load the rules engine
			context.SopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Context.WorkQueueItem.ServerPartitionKey);
			context.SopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
			context.SopProcessedRulesEngine.Load();

			// Add the update commands to update the files.  Note that the new Study Instance Uid is already part of this update.
			context.UpdateCommands.AddRange(Parameters.Commands);

			// Add command to update the Series & Sop Instances.
			context.UpdateCommands.Add(new SeriesSopUpdateCommand(UidMapper));

			// Create/Load the Study XML File
			StudyXml xml = LoadStudyXml(Context.DestStorageLocation);

			foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{
				string imagePath = GetReconcileUidPath(uid);
				DicomFile file = new DicomFile(imagePath);
				file.Load();

				try
				{
					string groupID = ServerHelper.GetUidGroup(file, Context.DestStorageLocation.ServerPartition, Context.WorkQueueItem.InsertTime);

				    SopInstanceProcessor sopProcessor = new SopInstanceProcessor(context);
                    ProcessingResult result = sopProcessor.ProcessFile(groupID, file, xml, false, uid, imagePath);
					if (result.Status != ProcessingStatus.Success)
					{
						throw new ApplicationException(String.Format("Unable to reconcile image {0}", file.Filename));
					}

					counter++;
					Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled and Processed SOP {0} [{1} of {2}]",
								 uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, "Unexpected exception when processing reconcile item", e);
					FailUid(uid, true);
				}
			}
		}

		private void CreateDestinationStudyStorage()
		{
			string newStudyInstanceUid = string.Empty;

			// Get the new Study Instance Uid by looking through the update commands
			foreach (BaseImageLevelUpdateCommand command in Parameters.Commands)
			{
				SetTagCommand setTag = command as SetTagCommand;
				if (setTag != null && setTag.Tag.TagValue.Equals(DicomTags.StudyInstanceUid))
				{
					newStudyInstanceUid = setTag.Value;
					break;
				}
			}

			if (string.IsNullOrEmpty(newStudyInstanceUid))
				throw new ApplicationException("Unexpectedly could not find new Study Instance Uid value for Create Study");


			using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
			{
				// Assign new series and instance uid
				if (Context.DestStorageLocation == null)
				{
					processor.AddCommand(new InitializeStorageCommand(Context, newStudyInstanceUid, Context.WorkQueueItemStudyStorage.StudyFolder,
						TransferSyntax.GetTransferSyntax(Context.WorkQueueItemStudyStorage.TransferSyntaxUid)));
				}

				if (!processor.Execute())
				{
					throw new ApplicationException(String.Format("Unable to create Study Storage for study: {0}", newStudyInstanceUid), processor.FailureException);
				}
			}
		}

		private void CreateWorkQueueEntryForDuplicate(DicomFile file, WorkQueue queue, WorkQueueUid uid, ServerPartition partition)
		{
			Platform.Log(LogLevel.Info, "Creating Work Queue Entry for duplicate...");
			String uidGroup = queue.GroupID ?? ServerHelper.GetUidGroup(file, partition, queue.InsertTime );
			using (ServerCommandProcessor commandProcessor = new ServerCommandProcessor("Insert Work Queue entry for duplicate"))
			{
				SopProcessingContext sopProcessingContext = new SopProcessingContext(commandProcessor, Context.DestStorageLocation, uidGroup);
				DicomProcessingResult result = DuplicateSopProcessorHelper.Process(sopProcessingContext, file);
				if (!result.Successful) throw new ApplicationException(result.ErrorMessage);

				commandProcessor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));
				commandProcessor.AddCommand(new DeleteWorkQueueUidCommand(uid));

				commandProcessor.Execute();
			}
		}

		#endregion
	}

	/// <summary>
	/// Command to initialize the study storage record in the database
	/// </summary>
	class InitializeStorageCommand : ServerDatabaseCommand<ReconcileStudyProcessorContext>
	{
		private readonly string _studyInstanceUid;
		private readonly string _studyDate;
		private readonly TransferSyntax _transferSyntax;

		public InitializeStorageCommand(ReconcileStudyProcessorContext context, DicomFile file)
			:base("InitializeStorageCommand", true, context)
		{
			Platform.CheckForNullReference(file, "file");
			_studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
			_studyDate = file.DataSet[DicomTags.StudyDate].ToString();
			_transferSyntax = file.TransferSyntax;
		}

		public InitializeStorageCommand(ReconcileStudyProcessorContext context, string studyInstanceUid,
			string studyDate, TransferSyntax transferSyntax)
			: base("InitializeStorageCommand", true, context)
		{
			Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForNullReference(studyDate, "studyDate");
			Platform.CheckForNullReference(transferSyntax, "transferSyntax");

			_studyInstanceUid = studyInstanceUid;
			_studyDate = studyDate;
			_transferSyntax = transferSyntax;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
		{
			if (Context.DestStorageLocation != null)
				return;
			else
			{
				Context.DestStorageLocation = FindOrCreateStudyStorageLocation();
			}
		}

		private StudyStorageLocation FindOrCreateStudyStorageLocation()
		{
			Platform.CheckForNullReference(UpdateContext, "UpdateContext");
			Platform.CheckForNullReference(Context, "Context");
			Platform.CheckForNullReference(Context.Partition, "Context.Partition");
            
            

			String folder = ServerHelper.ResolveStorageFolder(Context.Partition, _studyInstanceUid, _studyDate, UpdateContext, true);
            
			IQueryStudyStorageLocation locQuery = UpdateContext.GetBroker<IQueryStudyStorageLocation>();
			StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
			locParms.StudyInstanceUid = _studyInstanceUid;
			locParms.ServerPartitionKey = Context.Partition.GetKey();
			IList<StudyStorageLocation> studyLocationList = locQuery.Find(locParms);

			if (studyLocationList.Count == 0)
			{
				// INSERT NEW LOCATION INTO DB

				IStudyStorageEntityBroker selectBroker = UpdateContext.GetBroker<IStudyStorageEntityBroker>();
				StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();

				criteria.ServerPartitionKey.EqualTo(Context.Partition.GetKey());
				criteria.StudyInstanceUid.EqualTo(_studyInstanceUid);

				StudyStorage storage = selectBroker.FindOne(criteria);
				if (storage != null)
				{
					throw new Exception(String.Format("Received SOP Instances for Study in {0} state.  Rejecting image.", storage.StudyStatusEnum.Description));
				}

				FilesystemSelector selector = new FilesystemSelector(FilesystemMonitor.Instance);
				ServerFilesystemInfo filesystem = selector.SelectFilesystem();
				if (filesystem == null)
				{
					Platform.Log(LogLevel.Error, "Unable to select location for storing study.");

					return null;
				}

				IInsertStudyStorage locInsert = UpdateContext.GetBroker<IInsertStudyStorage>();
				InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
				insertParms.ServerPartitionKey = Context.Partition.GetKey();
				insertParms.StudyInstanceUid = _studyInstanceUid;
				insertParms.Folder = folder;
				insertParms.FilesystemKey = filesystem.Filesystem.GetKey();
				insertParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

				if (_transferSyntax.LosslessCompressed)
				{
					insertParms.TransferSyntaxUid = _transferSyntax.UidString;
					insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
				}
				else if (_transferSyntax.LossyCompressed)
				{
					insertParms.TransferSyntaxUid = _transferSyntax.UidString;
					insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
				}
				else
				{
					insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
					insertParms.StudyStatusEnum = StudyStatusEnum.Online;
				}

				studyLocationList = locInsert.Find(insertParms);
			}
			else
			{
				if (!FilesystemMonitor.Instance.CheckFilesystemWriteable(studyLocationList[0].FilesystemKey))
				{
					Platform.Log(LogLevel.Warn, "Unable to find writable filesystem for study {0} on Partition {1}",
								 _studyInstanceUid, Context.Partition.Description);
					return null;
				}
			}

			//TODO:  Do we need to do something to identify a primary storage location?
			// Also, should the above check for writeable location check the other availab
			return studyLocationList[0];
		}
	}
}