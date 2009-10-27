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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Reconcile.ProcessAsIs
{
	internal class ProcessAsIsCommand : ReconcileCommandBase
	{
		private StudyStorageLocation _destinationStudyStorage;
		private readonly bool _complete;

		public StudyStorageLocation Location
		{
			get { return _destinationStudyStorage; }
		}
   
		/// <summary>
		/// Creates an instance of <see cref="ProcessAsIsCommand"/>
		/// </summary>
		public ProcessAsIsCommand(ReconcileStudyProcessorContext context, bool complete)
			: base("Process As-is Command", true, context)
		{
			_complete = complete;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");
			
			DetermineTargetLocation();
		
			EnsureStudyCanBeUpdated(_destinationStudyStorage);

            try
            {
                ProcessUidList();
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

		private void DetermineTargetLocation()
		{
			if (Context.History.DestStudyStorageKey!=null)
			{
				_destinationStudyStorage =
					StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.StudyStorageKey))[0];

			}
			else
			{
				_destinationStudyStorage = Context.WorkQueueItemStudyStorage;
				Context.History.DestStudyStorageKey = _destinationStudyStorage.Key;
			}
		}

		protected override void OnUndo()
		{
			// undo is done  in SaveFile()
		}

		private void ProcessUidList()
		{
			int counter = 0;
			Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);

			StudyProcessorContext context = new StudyProcessorContext(_destinationStudyStorage);

			// Load the rules engine
			context.SopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Context.WorkQueueItem.ServerPartitionKey);
			context.SopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
			context.SopProcessedRulesEngine.Load();

			// Load the Study XML File
			StudyXml xml = LoadStudyXml(_destinationStudyStorage);

			foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{
				string imagePath = GetReconcileUidPath(uid);
				DicomFile file = new DicomFile(imagePath);
			
				try
				{
					file.Load();
					
					string groupID = ServerHelper.GetUidGroup(file, _destinationStudyStorage.ServerPartition, Context.WorkQueueItem.InsertTime);

				    SopInstanceProcessor sopProcessor = new SopInstanceProcessor(context);
					ProcessingResult result = sopProcessor.ProcessFile(groupID, file, xml, false, uid, GetReconcileUidPath(uid));
					if (result.Status != ProcessingStatus.Success)
					{
						throw new ApplicationException(String.Format("Unable to reconcile image {0}", file.Filename));
					}

					counter++;
			
					Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled SOP {0} [{1} of {2}]",
					             uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
				}
				catch (Exception e)
				{
					if (e is InstanceAlreadyExistsException
						|| e.InnerException != null && e.InnerException is InstanceAlreadyExistsException)
					{
						CreatDuplicateSIQEntry(file, Context.WorkQueueItem, uid);
					}
					else
						FailUid(uid, true);
				}
			}
		}

		private void CreatDuplicateSIQEntry(DicomFile file, WorkQueue queue, WorkQueueUid uid)
		{
			Platform.Log(LogLevel.Info, "Creating Work Queue Entry for duplicate...");
			String uidGroup = queue.GroupID ?? queue.GetKey().Key.ToString();
			using (ServerCommandProcessor commandProcessor = new ServerCommandProcessor("Insert Work Queue entry for duplicate"))
			{
				SopProcessingContext sopProcessingContext = new SopProcessingContext(commandProcessor, _destinationStudyStorage, uidGroup);
				DicomProcessingResult result = DuplicateSopProcessorHelper.Process(sopProcessingContext, file);
				if (!result.Successful)
				{
					FailUid(uid, true);
					return;
				}

				commandProcessor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));
				commandProcessor.AddCommand(new DeleteWorkQueueUidCommand(uid));

				if (!commandProcessor.Execute())
				{
					Platform.Log(LogLevel.Error,"Unexpected error when creating duplicate study integrity queue entry: {0}", commandProcessor.FailureReason);
					FailUid(uid, true);
				}
			}
		}
	}
}