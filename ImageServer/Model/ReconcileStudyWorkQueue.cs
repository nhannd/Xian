using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public class ReconcileStudyWorkQueue 
    {
        private Model.WorkQueue _workqueue;
        private StudyStorageLocation _storageLocation;

        public ReconcileStudyWorkQueue(Model.WorkQueue workqueue)
        {
            _workqueue = workqueue;
        }

        public StudyStorageLocation GetStorageLocation()
        {
            if (_storageLocation == null)
            {
                IList<StudyStorageLocation> locations = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(_workqueue.StudyStorageKey));
                _storageLocation = CollectionUtils.FirstElement(locations);
            }

            return _storageLocation;
        }

        public string GetReconcileFolder()
        {
            StudyStorageLocation location = GetStorageLocation();

            string path = Path.Combine(location.FilesystemPath, location.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, _workqueue.Key.ToString());
            return path;
        }
    }
}
