using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Represents the context of image reconciliation scheduling when an image is processed by the 'StudyProcess' processor
    /// </summary>
    public class ReconcileImageContext
    {
        #region Private Members
        private DicomFile _file;
        private Study _existingStudy;
        private StudyStorageLocation _existStudyLocation;
        private ServerPartition _partition;
        private StudyIntegrityQueue _studyIntegrityQueue;
        private StudyHistory _history;
        private StudyStorageLocation _destinationStudyLocation;
        private bool _isDuplicate;

        private string _storagePath;
        #endregion

        #region Public Properties

        /// <summary>
        /// The dicom file to be reconciled
        /// </summary>
        public DicomFile File
        {
            get { return _file; }
            set { _file = value; }
        }

        /// <summary>
        /// The location of the existing study that has the same Study Instance Uid
        /// </summary>
        public StudyStorageLocation CurrentStudyLocation
        {
            get { return _existStudyLocation; }
            set { _existStudyLocation = value; }
        }

        /// <summary>
        /// The existing study that has the same Study Instance Uid
        /// </summary>
        public Study CurrentStudy
        {
            get { return _existingStudy; }
            set { _existingStudy = value; }
        }

        /// <summary>
        /// The Server Partition where the reconciliation takes place
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        /// <summary>
        /// The reconcile queue entry created as a result of the reconciliation
        /// </summary>
        public StudyIntegrityQueue ReconcileQueue
        {
            get { return _studyIntegrityQueue; }
            set { _studyIntegrityQueue = value; }
        }

        /// <summary>
        /// The <see cref="StudyHistory"/> record for the existing study.
        /// </summary>
        public StudyHistory History
        {
            get { return _history; }
            set { _history = value; }
        }

        public StudyStorageLocation DestinationStudyLocation
        {
            get { return _destinationStudyLocation; }
            set { _destinationStudyLocation = value; }
        }

        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }

        public bool IsDuplicate
        {
            get { return _isDuplicate; }
            set { _isDuplicate = value; }
        }

        #endregion
    }

}
