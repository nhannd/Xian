#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
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
        private readonly string _uidRelativePath;
        private string _uidGroupId;

        #endregion

        public UpdateWorkQueueCommand(DicomMessageBase message,
                        StudyStorageLocation location,
                        bool duplicate,
                        string extension)
            : this(message, location, duplicate, extension, null, null)
        {
        }

        public UpdateWorkQueueCommand(DicomMessageBase message, 
                        StudyStorageLocation location, 
                        bool duplicate, 
                        string extension,
                        string uidGroupId,
                        string uidRelativePath
            )
            : base("Update/Insert a WorkQueue Entry", true)
        {
            Platform.CheckForNullReference(message, "Dicom Message object");
            Platform.CheckForNullReference(location, "Study Storage Location");
            
            _message = message;
            _storageLocation = location;
            _duplicate = duplicate;
            _extension = extension;
            _uidGroupId = uidGroupId;
            _uidRelativePath = uidRelativePath;
        }

        public WorkQueue InsertedWorkQueue
        {
            get { return _insertedWorkQueue; }
        }

        protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
        {
            IInsertWorkQueue insert = updateContext.GetBroker<IInsertWorkQueue>();
            InsertWorkQueueParameters parms = new InsertWorkQueueParameters();
            parms.WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess;
            parms.StudyStorageKey = _storageLocation.GetKey();
            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.SeriesInstanceUid = _message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            parms.SopInstanceUid = _message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            parms.ScheduledTime = Platform.Time;
            parms.ExpirationTime = Platform.Time.AddMinutes(5.0);
            parms.WorkQueueGroupID = _uidGroupId;

            if (_duplicate)
            {
                parms.Duplicate = _duplicate;
                parms.Extension = _extension;
                parms.UidGroupID = _uidGroupId;
                parms.UidRelativePath = _uidRelativePath;
            }

            _insertedWorkQueue = insert.FindOne(parms);

            if (_insertedWorkQueue == null)
                throw new ApplicationException("UpdateWorkQueueCommand failed");
        }
    }
}
