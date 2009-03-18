using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    public class UpdateWorkQueueCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly DicomMessageBase _message;
        private readonly StudyStorageLocation _storageLocation;
        private WorkQueue _insertedWorkQueue;
        private readonly bool _duplicate;
        private readonly string _extension;
        #endregion

        public UpdateWorkQueueCommand(DicomMessageBase message, StudyStorageLocation location, bool duplicate, string extension)
            : base("Update/Insert a WorkQueue Entry", true)
        {
            Platform.CheckForNullReference(message, "Dicom Message object");
            Platform.CheckForNullReference(location, "Study Storage Location");

            _message = message;
            _storageLocation = location;
            _duplicate = duplicate;
            _extension = extension;
        }

        public WorkQueue InsertedWorkQueue
        {
            get { return _insertedWorkQueue; }
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            IInsertWorkQueue insert = updateContext.GetBroker<IInsertWorkQueue>();
            InsertWorkQueueParameters parms = new InsertWorkQueueParameters();
            parms.WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess;
            parms.StudyStorageKey = _storageLocation.GetKey();
            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.SeriesInstanceUid = _message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
            parms.SopInstanceUid = _message.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
            parms.ScheduledTime = Platform.Time;
            parms.ExpirationTime = Platform.Time.AddMinutes(5.0);
            parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;

            if (_duplicate)
            {
                parms.Duplicate = _duplicate;
                parms.Extension = _extension;
            }

            _insertedWorkQueue = insert.FindOne(parms);

            if (_insertedWorkQueue == null)
                throw new ApplicationException("UpdateWorkQueueCommand failed");
        }
    }
}
