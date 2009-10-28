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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Reconcile.MergeStudy
{
	/// <summary>
	/// Command for reconciling images by merging new images into an existing study.
	/// </summary>
	/// <remark>
	/// </remark>
	class MergeStudyCommand : ReconcileCommandBase
	{
		#region Private Members
		
		private int _failedCount;
		private int _processedCount;
		private StudyStorageLocation _destinationStudyStorage;
		private readonly bool _updateDestination;
		private readonly bool _complete;
		private readonly List<BaseImageLevelUpdateCommand> _commands;

		#endregion

		#region Properties

		public StudyStorageLocation Location
		{
			get { return _destinationStudyStorage; }
		}

		#endregion

        #region Constructors
		/// <summary>
		/// Creates an instance of <see cref="MergeStudyCommand"/>
		/// </summary>
		public MergeStudyCommand(ReconcileStudyProcessorContext context, bool updateDestination, List<BaseImageLevelUpdateCommand> commands, bool complete)
			: base("Merge Study", true, context)
		{
			_updateDestination = updateDestination;
			_commands = commands;
			_complete = complete;
		}
		#endregion

		#region Overriden Protected Methods
		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");

			_destinationStudyStorage = Context.History.DestStudyStorageKey != null 
				? StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.DestStudyStorageKey))[0] 
				: Context.WorkQueueItemStudyStorage;

            EnsureStudyCanBeUpdated(_destinationStudyStorage);

			if (_updateDestination)
				UpdateExistingStudy();
            
			LoadMergedStudyEntities();

            try
            {
                LoadUidMappings();

                if (Context.WorkQueueUidList.Count>0)
                {
                    ProcessUidList();
                    LogResult();
                }
            }
            finally
            {
                UpdateHistory(_destinationStudyStorage);
            }

			if (_complete)
			{
				StudyRulesEngine engine = new StudyRulesEngine(_destinationStudyStorage, Context.Partition);
				engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed, theProcessor);
			}  
		}

        protected void LoadUidMappings()
        {
            // Load the mapping for the study
			if (_destinationStudyStorage != null)
            {
				string path = Path.Combine(_destinationStudyStorage.GetStudyPath(), "UidMap.xml");
                if (File.Exists(path))
                {
                    UidMapXml xml = new UidMapXml();
					xml.Load(_destinationStudyStorage);
                    UidMapper = new UidMapper(xml);

                    UidMapper.SeriesMapUpdated += UidMapper_SeriesMapUpdated;
                }
            }
        }

		#endregion

		#region Protected Methods
	
		protected override void OnUndo()
		{
		}

		#endregion

		#region Private Members
		
		private void LogResult()
		{
			StringBuilder log = new StringBuilder();
			log.AppendFormat("Destination location: {0}", _destinationStudyStorage.GetStudyPath());
			log.AppendLine();
			if (_failedCount > 0)
			{
				log.AppendFormat("{0} images failed to be reconciled.", _failedCount);
				log.AppendLine();
			}
            
			log.AppendFormat("{0} images have been reconciled and will be processed.", _processedCount);
			log.AppendLine();
			Platform.Log(LogLevel.Info, log);
		}

		private void UpdateExistingStudy()
		{

			Platform.Log(LogLevel.Info, "Updating existing study...");
			using(ServerCommandProcessor updateProcessor = new ServerCommandProcessor("Update Study"))
			{
				UpdateStudyCommand studyUpdateCommand = new UpdateStudyCommand(Context.Partition, _destinationStudyStorage, _commands, ServerRuleApplyTimeEnum.SopProcessed);
				updateProcessor.AddCommand(studyUpdateCommand);
				if (!updateProcessor.Execute())
				{
					throw new ApplicationException(
						String.Format("Unable to update existing study: {0}", updateProcessor.FailureReason));
				}
			}            
		}

		private void ProcessUidList()
		{
			Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);

			StudyProcessorContext context = new StudyProcessorContext(_destinationStudyStorage);

			// Load the rules engine
			context.SopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Context.WorkQueueItem.ServerPartitionKey);
			context.SopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
			context.SopProcessedRulesEngine.Load();

			// Add the update commands to
			context.UpdateCommands.AddRange(BuildUpdateCommandList());

            // Add command to update the Series & Sop Instances.
            context.UpdateCommands.Add(new SeriesSopUpdateCommand(Context.WorkQueueItemStudyStorage, _destinationStudyStorage, UidMapper));

            // Load the Study XML File
			StudyXml xml = LoadStudyXml(_destinationStudyStorage);
            PrintUpdateCommands(context.UpdateCommands);
            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{
				// Load the file outside the try/catch block so it can be
				// referenced in the c
				string imagePath = GetReconcileUidPath(uid);
				DicomFile file = new DicomFile(imagePath);

				try
				{
					file.Load();

					string groupID = ServerHelper.GetUidGroup(file, Context.Partition, Context.WorkQueueItem.InsertTime);

				    SopInstanceProcessor sopProcessor = new SopInstanceProcessor(context);
                    ProcessingResult result = sopProcessor.ProcessFile(groupID, file, xml, false, uid, GetReconcileUidPath(uid));
					if (result.Status != ProcessingStatus.Success)
					{
						throw new ApplicationException(String.Format("Unable to reconcile image {0}", file.Filename));
					}

					_processedCount++;

					Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled SOP {0} [{1} of {2}]", uid.SopInstanceUid, _processedCount, Context.WorkQueueUidList.Count);
				}
				catch (Exception e)
				{
					if (e is InstanceAlreadyExistsException
						|| e.InnerException != null && e.InnerException is InstanceAlreadyExistsException)
					{
						DuplicateSopProcessorHelper.CreateDuplicateSIQEntry(file, _destinationStudyStorage, GetReconcileUidPath(uid),
												   Context.WorkQueueItem, uid);
					}
					else
						FailUid(uid, true);
					_failedCount++;
				}
			}
		}

		private void LoadMergedStudyEntities()
		{
			StudyStorage storage = StudyStorage.Load(_destinationStudyStorage.Key);
			_destinationStudyStorage = StudyStorageLocation.FindStorageLocations(storage)[0];
		}


		private List<BaseImageLevelUpdateCommand> BuildUpdateCommandList()
		{
			List<BaseImageLevelUpdateCommand> updateCommandList = new List<BaseImageLevelUpdateCommand>();
            
			ImageUpdateCommandBuilder builder = new ImageUpdateCommandBuilder();
			updateCommandList.AddRange(builder.BuildCommands<StudyMatchingMap>(_destinationStudyStorage));
            
			return updateCommandList;
		}

		#endregion

		#region Private Static Methods
		private static void PrintUpdateCommands(IEnumerable<BaseImageLevelUpdateCommand> updateCommandList)
		{
			StringBuilder log = new StringBuilder();
			log.AppendLine("Update on merged images:");
			foreach (BaseImageLevelUpdateCommand cmd in updateCommandList)
			{
				log.AppendLine(String.Format("\t{0}", cmd));
			}
			Platform.Log(LogLevel.Info, log);
		}

		#endregion
	}
}