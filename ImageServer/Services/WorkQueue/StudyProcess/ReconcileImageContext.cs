using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    public class ReconcileImageContext
    {
        private DicomFile _file;
        private StudyStorageLocation _studyLocation;
        private ServerPartition _partition;
        private ReconcileQueue _reconcileQueue;
        private StudyHistory _history;

        private string _storagePath;

        public DicomFile File
        {
            get { return _file; }
            set { _file = value; }
        }

        public StudyStorageLocation CurrentStudyLocation
        {
            get { return _studyLocation; }
            set { _studyLocation = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public ReconcileQueue ReconcileQueue
        {
            get { return _reconcileQueue; }
            set { _reconcileQueue = value; }
        }

        public StudyHistory History
        {
            get { return _history; }
            set { _history = value; }
        }

        public string TempStoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }
    }

}
