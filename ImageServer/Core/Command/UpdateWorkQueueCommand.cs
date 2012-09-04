#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Command
{
    public class UpdateWorkQueueCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly DicomMessageBase _message;
        private readonly StudyStorageLocation _storageLocation;
        private WorkQueue _insertedWorkQueue;
        private readonly bool _duplicate;
        private readonly string _extension;
    	private readonly string _uidGroupId;

        #endregion

        public UpdateWorkQueueCommand(DicomMessageBase message,
                        StudyStorageLocation location,
                        bool duplicate,
                        string extension)
            : this(message, location, duplicate, extension, null)
        {
        }

        public UpdateWorkQueueCommand(DicomMessageBase message, StudyStorageLocation location, bool duplicate, string extension, string uidGroupId)
            : base("Update/Insert a WorkQueue Entry")
        {
            Platform.CheckForNullReference(message, "Dicom Message object");
            Platform.CheckForNullReference(location, "Study Storage Location");
            
            _message = message;
            _storageLocation = location;
            _duplicate = duplicate;
            _extension = extension;
            _uidGroupId = uidGroupId;
        }

        public WorkQueue InsertedWorkQueue
        {
            get { return _insertedWorkQueue; }
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var insert = updateContext.GetBroker<IInsertWorkQueue>();
            var parms = new InsertWorkQueueParameters
                            {
                                WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess,
                                StudyStorageKey = _storageLocation.GetKey(),
                                ServerPartitionKey = _storageLocation.ServerPartitionKey,
                                SeriesInstanceUid = _message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty),
                                SopInstanceUid = _message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty),
                                ScheduledTime = Platform.Time,
                                WorkQueueGroupID = _uidGroupId
                            };

        	if (_duplicate)
            {
                parms.Duplicate = _duplicate;
                parms.Extension = _extension;
                parms.UidGroupID = _uidGroupId;
            }

            _insertedWorkQueue = insert.FindOne(parms);

            if (_insertedWorkQueue == null)
                throw new ApplicationException("UpdateWorkQueueCommand failed");
        }
    }
}
