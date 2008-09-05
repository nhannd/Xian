using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    class ReconcileImageContext
    {
        private DicomFile _file;
        private StudyStorageLocation _studyLocation;
        private ServerPartition _partition;
        private ReconcileQueue _reconcileQueue;

        public DicomFile File
        {
            get { return _file; }
            set { _file = value; }
        }

        public StudyStorageLocation StudyLocation
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
    }

}
