using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Encapsulated the context of the image reconciliation operation.
    /// </summary>
    public class ReconcileStudyProcessorContext
    {
        #region Private Members
        private Model.WorkQueue _item;
        private ReconcileStudyWorkQueueData _data;
        private ServerPartition _partition;
        private StudyHistory _history;
        private StudyStorageLocation _destStudyStorageLocation;
        private Study _destStudy;
        private ServerFilesystemInfo _targetFilesystem;
        private IList<WorkQueueUid> _workQueueUidList;
        private DicomFile _reconcileImage;
        #endregion

        #region Public Properties
        /// <summary>
        /// The 'ReconcileStudy' <see cref="WorkQueue"/> item.
        /// </summary>
        public Model.WorkQueue WorkQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// The server partition associated with <see cref="WorkQueueItem"/>
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        /// <summary>
        /// The "decoded" queue data associated with <see cref="WorkQueueItem"/>
        /// </summary>
        public ReconcileStudyWorkQueueData ReconcileWorkQueueData
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// The <see cref="StudyHistory"/> associated with the <see cref="WorkQueueItem"/>
        /// </summary>
        public StudyHistory History
        {
            get { return _history; }
            set { _history = value; }
        }

        /// <summary>
        /// The <see cref="StudyStorageLocation"/> of the resultant study which the images will be reconciled to.
        /// </summary>
        public StudyStorageLocation DestStorageLocation
        {
            get { return _destStudyStorageLocation; }
            set { _destStudyStorageLocation = value; }
        }

        public ServerFilesystemInfo DestFilesystem
        {
            get { return _targetFilesystem; }
            set { _targetFilesystem = value; }
        }

        public IList<WorkQueueUid> WorkQueueUidList
        {
            get { return _workQueueUidList; }
            set { _workQueueUidList = value; }
        }

        public DicomFile ReconcileImage
        {
            get { return _reconcileImage; }
            set { _reconcileImage = value; }
        }

        public Study DestStudy
        {
            get { return _destStudy; }
            set { _destStudy = value; }
        }

        #endregion

    }

}
