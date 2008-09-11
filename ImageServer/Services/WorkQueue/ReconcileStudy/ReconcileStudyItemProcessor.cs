using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{

    /// <summary>
    /// Processor to handle 'ReconcileStudy' work queue entries
    /// </summary>
    class ReconcileStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        private ReconcileDicomFileCommand _reconcileCommand;
        private UpdateDicomFileCommand _updateDicomFileCommand;
        private ReconcileStudyWorkQueueData _reconcileQueueData;
        #endregion

        #region Overridden Protected Method
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "item.Data");
            Platform.CheckForNullReference(item.StudyHistoryKey, "item.StudyHistoryKey");

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
                    _reconcileQueueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(WorkQueueItem.Data);

                    LoadStorageLocation(item);

                    ProcessUidList();

                    PostProcessing(item, WorkQueueUidList.Count > 0, WorkQueueUidList.Count == 0);
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
        }

        private void ProcessUidList()
        {
            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                ReconcileUid(uid);
            }
        }

        private void SetupCommands(DicomFile file)
        {
            if (_updateDicomFileCommand==null)
            {
                XmlDocument changeDescXml = _context.History.ChangeDescription;
                XmlNode updateImagesNode = changeDescXml.SelectSingleNode("UpdateImages");
                if (updateImagesNode != null)
                {
                    UpdateImagesCommandXmlSpecification specs = new UpdateImagesCommandXmlSpecification();
                    specs.Parse(changeDescXml.SelectSingleNode("UpdateImages"));
                    _updateDicomFileCommand = new UpdateDicomFileCommand(specs.ActionList);
                }
            }

            if (_reconcileCommand==null)
                _reconcileCommand = new ReconcileDicomFileCommand();


            _updateDicomFileCommand.DicomFile = file;
            _reconcileCommand.Context = _context;
            _reconcileCommand.DicomFile = file;
        }

        private void SetupContext(DicomFile file)
        {
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");

            _context = new ReconcileStudyProcessorContext();
            _context.WorkQueueItem = WorkQueueItem;
            _context.Partition = ServerPartition.Load(StorageLocation.ServerPartitionKey);
            _context.Data = _reconcileQueueData;
            _context.History = StudyHistory.Load(WorkQueueItem.StudyHistoryKey);
            _context.ExistingStudyStorageLocation = StorageLocation;   
        }


        private void ReconcileUid(WorkQueueUid uid)
        {
            string imagePath = GetCurrentImagePath(uid);
            Debug.Assert(File.Exists(imagePath));
            
            Platform.Log(LogLevel.Info, "Reconciling image : {0} in {1}", uid.SopInstanceUid, imagePath);
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
            
            DicomFile file = new DicomFile(path);
            file.Load(DicomReadOptions.StorePixelDataReferences);

            SetupContext(file);
            SetupCommands(file);

            using(ServerCommandProcessor processor = new ServerCommandProcessor("Reconcile File"))
            {
                processor.AddCommand(_updateDicomFileCommand);
                processor.AddCommand(_reconcileCommand);

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
