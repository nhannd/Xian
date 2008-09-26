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
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

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
        List<IReconcileServerCommand> _commandList = null;
        #endregion

        #region Overridden Protected Method
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "item.Data");
            
            if (CannotStart())
            {
                WorkQueueSettings settings = WorkQueueSettings.Instance;
                PostponeItem(item, Platform.Time.Add(TimeSpan.FromMinutes(settings.WorkQueueQueryDelay)),
                                  Platform.Time.Add(TimeSpan.FromSeconds(settings.WorkQueueExpireDelaySeconds)));
            }
            else
            {
                LoadUids(item);

                if (WorkQueueUidList.Count == 0)
                {
                    Complete(item);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Reconcililation started. GUID={0}.", WorkQueueItem.GetKey());

                    _reconcileQueueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(WorkQueueItem.Data);

					if (!LoadStorageLocation(item))
					{
						Platform.Log(LogLevel.Warn, "Unable to find readable location when processing ReconcileStudy WorkQueue item, rescheduling");
						PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
						return;
					}

                    InitializeContext();
                    
                    ProcessUidList();

                    Complete(item);
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


        private void Complete(Model.WorkQueue item)
        {
            PostProcessing(item, false, true);
            Platform.Log(LogLevel.Info, "Reconciliation completed");
        }

        private void ProcessUidList()
        {
            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                ReconcileUid(uid);
            }
        }

        
        private void SetupCommands(ServerCommandProcessor processor)
        {
            // only set up the command once
            if (_commandList==null)
            {
                ReconcileCommandXmlParser parser = new ReconcileCommandXmlParser();
                _commandList = parser.Parse(_context.WorkQueueItem.Data);

                if (_commandList == null || _commandList.Count == 0)
                {
                    // look for commands in the History record
                    if (_context.History != null && _context.History.ChangeDescription!=null)
                        _commandList = parser.Parse(_context.History.ChangeDescription);
                }

                foreach (IReconcileServerCommand command in _commandList)
                {
                    Platform.Log(LogLevel.Info, "Command: {0}", command.Description);
                }
            }


            if (_commandList==null || _commandList.Count==0)
            {
                throw new ApplicationException("Command is not specified for this entry");
            }

            foreach (IReconcileServerCommand command in _commandList)
            {
                command.Context = _context;
                processor.AddCommand(command);
            }
        }

        private void InitializeContext()
        {
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");
            _context = new ReconcileStudyProcessorContext();
            _context.WorkQueueItem = WorkQueueItem;
            _context.Partition = ServerPartition.Load(StorageLocation.ServerPartitionKey);
            _context.Data = _reconcileQueueData;
            _context.History = WorkQueueItem.StudyHistoryKey != null
                                   ? StudyHistory.Load(WorkQueueItem.StudyHistoryKey)
                                   : null;

            _context.ExistingStudyStorageLocation = StorageLocation; // Note: 
            
        }

        private void SetupContext(DicomFile file)
        {
            _context.ReconcileImage = file;
        }


        private void ReconcileUid(WorkQueueUid uid)
        {
            string imagePath = GetCurrentImagePath(uid);
            Debug.Assert(File.Exists(imagePath));
            
            Platform.Log(LogLevel.Debug, "Reconciling image : {0} in {1}", uid.SopInstanceUid, imagePath);
            ProcessFile(uid, imagePath);

            DeleteWorkQueueUid(uid);
            
        }

        private string GetCurrentImagePath(WorkQueueUid uid)
        {
            string path = Path.Combine(_reconcileQueueData.StoragePath, uid.SopInstanceUid +".dcm");
            return path;
        }

        private void ProcessFile(WorkQueueUid uid, string path)
        {
            Platform.Log(LogLevel.Debug, "Reconciling {0}", path);
            DicomFile file = new DicomFile(path);
            file.Load(DicomReadOptions.StorePixelDataReferences);
            
            using(ServerCommandProcessor processor = new ServerCommandProcessor("Reconcile File"))
            {
                SetupContext(file);
                SetupCommands(processor);
                
                if (!processor.Execute())
                {
                    throw new ApplicationException(
                        String.Format("Unable to reconcile dicom file {0} : {1}", path, processor.FailureReason));
                }
                else
                {
                    // image has been reconciled. It can be deleted.
                    File.Delete(path);
                    DirectoryUtility.DeleteIfEmpty(Directory.GetParent(path).FullName);
                }
            }
            
        }

        #endregion

    }

    

    
}
