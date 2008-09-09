using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Processor to handle 'ReconcileStudy' work queue entries
    /// </summary>
    class ReconcileStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        #endregion

        #region Overridden Protected Method
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "item.Data");

            LoadStorageLocation(item);
            LoadUids(item);

            SetupContext(item);

            ProcessUidList();

            PostProcessing(item, true, WorkQueueUidList.Count==0);
        }
        #endregion


        #region Private Methods
        private void ProcessUidList()
        {
            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                ReconcileUid(uid);
            }
        }

        private void SetupContext(Model.WorkQueue item)
        {
            _context = new ReconcileStudyProcessorContext();
            _context.WorkQueueItem = item;
            _context.Partition = ServerPartition.Load(StorageLocation.ServerPartitionKey);
            _context.Data = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(_context.WorkQueueItem.Data);
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
            string path = Path.Combine(_context.Data.StoragePath, uid.SopInstanceUid +".dcm");
            return path;
        }

        private void ProcessFile(WorkQueueUid uid, string path)
        {
            // TODO: Simulate what the StorageSCP does here
            // create a StorageLocation if neeeded.
            // create the folder for the StorageLocation if needed
            // store the image files in this folder
            // create StudyProcess work queue entry

            File.Delete(path);
            DirectoryUtility.DeleteIfEmpty(Directory.GetParent(path).FullName);
        }

        #endregion

    }
}
