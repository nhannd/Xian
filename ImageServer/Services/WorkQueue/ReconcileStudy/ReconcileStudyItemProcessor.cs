#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Processor to handle 'ReconcileStudy' work queue entries
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class ReconcileStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        private ReconcileStudyWorkQueueData _reconcileQueueData;
        private IReconcileProcessor _processor;
        #endregion

        #region Overridden Protected Method
		protected override void ProcessItem(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");
			Platform.CheckForNullReference(item.Data, "item.Data");

		    _reconcileQueueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(WorkQueueItem.Data);

			LoadUids(item);

            InitializeContext();

            SetupProcessor();
				
			if (WorkQueueUidList.Count == 0)
			{
                Platform.Log(LogLevel.Info,
                             "Completing study reconciliation for Study {0}, Patient {1} (PatientId:{2} A#:{3}) on Partition {4}, {5} objects",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description, WorkQueueUidList.Count);
                ExecuteCommands(true);
			}
			else
			{
				Platform.Log(LogLevel.Info,
				             "Reconciling study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}, {5} objects",
				             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
				             Study.AccessionNumber, ServerPartition.Description, WorkQueueUidList.Count);

				ExecuteCommands(false);
			}
		}

        private void ExecuteCommands(bool complete)
        {
            if(_processor!=null)
            {
                using(_processor)
                {
                    _processor.Initialize(_context, complete);

                    if (!_processor.Execute())
                    {
                        FailQueueItem(WorkQueueItem, _processor.FailureReason);
                    }
                    else
                    {
                        if (complete)
                            OnComplete();
                        else
                            OnBatchComplete();
                    }
                }                
            }            
        }
        
        protected override bool CanStart()
        {
            // cannot start if the existing study is scheduled for update
            IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem, new[]
                                              {
                                                  WorkQueueTypeEnum.ReprocessStudy
                                              }, 
                                              null);
			if (!(relatedItems == null || relatedItems.Count == 0))
            {
				PostponeItem("Study is scheduled for reprocess");
            	return false;
            }
        	return true;
        }

        #endregion

        #region Private Mehods

        private void OnComplete()
        {
			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Complete, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            Platform.Log(LogLevel.Info, "Reconciliation completed");
        }

        private void OnBatchComplete()
        {
			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Pending, 
				WorkQueueProcessorDatabaseUpdate.None);
            Platform.Log(LogLevel.Info, "StudyReconcile processed.");
        }

        
        private void SetupProcessor()
        {
            Debug.Assert(_context != null);

            ReconcileCommandXmlParser parser = new ReconcileCommandXmlParser();
            _processor = parser.Parse(_context.History.ChangeDescription);

            Platform.Log(LogLevel.Info, "Using {0}", _processor.Name);
        }

        private void InitializeContext()
        {
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");
            _context = new ReconcileStudyProcessorContext
                       	{
                       		WorkQueueItem = WorkQueueItem,
                       		WorkQueueItemStudyStorage = StorageLocation,
                       		Partition = ServerPartition,
                       		ReconcileWorkQueueData = _reconcileQueueData,
                       		WorkQueueUidList = WorkQueueUidList,
                       		History = WorkQueueItem.StudyHistoryKey != null
                       		          	? StudyHistory.Load(WorkQueueItem.StudyHistoryKey)
                       		          	: null
                       	};
        }

        #endregion
    }
}
