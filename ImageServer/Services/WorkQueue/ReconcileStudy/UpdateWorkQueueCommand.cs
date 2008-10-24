using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{

    public class UpdateWorkQueueCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly DicomMessageBase _message;
        private readonly StudyStorageLocation _storageLocation;
        private readonly string _extension;
        #endregion

        public UpdateWorkQueueCommand(DicomMessageBase message, StudyStorageLocation location, string extension)
            : base("Update/Insert a WorkQueue Entry", true)
        {
            Platform.CheckForNullReference(message, "Dicom Message object");
            Platform.CheckForNullReference(location, "Study Storage Location");

            _message = message;
            _storageLocation = location;
            _extension = extension;
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            IInsertWorkQueueStudyProcess insert = updateContext.GetBroker<IInsertWorkQueueStudyProcess>();
            WorkQueueStudyProcessInsertParameters parms = new WorkQueueStudyProcessInsertParameters();
            parms.StudyStorageKey = _storageLocation.GetKey();
            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.SeriesInstanceUid = _message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
            parms.SopInstanceUid = _message.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
            parms.ScheduledTime = Platform.Time;
            parms.ExpirationTime = Platform.Time.AddMinutes(5.0);
            parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;

            insert.Execute(parms);


        }
    }
}
