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
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{

    /// <summary>
    /// Processor to handle 'ReconcileStudy' work queue entries
    /// </summary>
    class ReconcileStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        private ReconcileStudyWorkQueueData _reconcileQueueData;
        //List<IReconcileServerCommand> _commandList = null;
        private IReconcileProcessor _processor;
        #endregion

        #region Overridden Protected Method
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "item.Data");
            
            if (CannotStart())
            {
                WorkQueueSettings settings = WorkQueueSettings.Instance;
                Platform.Log(LogLevel.Info, "Postpone ReconcileStudy entry. [GUID={0}]", item.GetKey());
                PostponeItem(item, Platform.Time.Add(TimeSpan.FromMilliseconds(settings.WorkQueueQueryDelay)),
                                  Platform.Time.Add(TimeSpan.FromSeconds(settings.WorkQueueExpireDelaySeconds)));
            }
            else
            {
                _reconcileQueueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(WorkQueueItem.Data);
                
                LoadUids(item);
                

                if (WorkQueueUidList.Count == 0)
                {
                    Complete();
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Reconcililation started. GUID={0}.", WorkQueueItem.GetKey());

					if (!LoadStorageLocation(item))
					{
						Platform.Log(LogLevel.Warn, "Unable to find readable location when processing ReconcileStudy WorkQueue item, rescheduling");
						PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
						return;
					}

                    InitializeContext();
                    
                    SetupProcessor();
                    
                    ExecuteCommands();
                    
                }
            }
            
        }

        private void ExecuteCommands()
        {
            if(_processor!=null)
            {
                using(_processor)
                {
                    Platform.Log(LogLevel.Info, "Using {0}", _processor.Name);

                    _processor.Initialize(_context);

                    if (!_processor.Execute())
                    {
                        FailQueueItem(WorkQueueItem, _processor.FailureReason);
                    }
                    else
                    {
                        PostProcessing(WorkQueueItem, true, false);
                        Platform.Log(LogLevel.Info, "Reconciliation is completed.");
                    }
                }
                
                
            }
            
        }

        #endregion


        #region Private Methods


        private bool CannotStart()
        {
            // cannot start if the existing study is scheduled for update
            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(WorkQueueItem.ServerPartitionKey);
            criteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);
            criteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] { WorkQueueTypeEnum.WebEditStudy });

            List<Model.WorkQueue> items = FindRelatedWorkQueueItems(WorkQueueItem, criteria);
            return items.Count > 0;
        }


        private void Complete()
        {
            DirectoryUtility.DeleteIfEmpty(_reconcileQueueData.StoragePath);
            PostProcessing(WorkQueueItem, false, true);
            Platform.Log(LogLevel.Info, "Reconciliation completed");
        }

        
        private void SetupProcessor()
        {
            Debug.Assert(_context != null);

            ReconcileCommandXmlParser parser = new ReconcileCommandXmlParser();
            _processor = parser.Parse(_context.History.ChangeDescription);
        }

        private void InitializeContext()
        {
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");
            _context = new ReconcileStudyProcessorContext();
            _context.WorkQueueItem = WorkQueueItem;
            _context.Partition = ServerPartition.Load(StorageLocation.ServerPartitionKey);
            _context.ReconcileWorkQueueData = _reconcileQueueData;
            _context.WorkQueueUidList = WorkQueueUidList;
            _context.History = WorkQueueItem.StudyHistoryKey != null
                                   ? StudyHistory.Load(WorkQueueItem.StudyHistoryKey)
                                   : null;
            
        }
        #endregion

    }

    interface IReconcileProcessor : IDisposable
    {
        void Initialize(ReconcileStudyProcessorContext context);
        bool Execute();
        String FailureReason { get;}
        String Name { get; }
    }

    
}
