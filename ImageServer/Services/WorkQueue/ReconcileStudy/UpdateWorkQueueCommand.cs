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
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{


    internal class UpdateWorkQueueCommand :
        ServerDatabaseCommand<ReconcileStudyProcessorContext, UpdateWorkQueueCommand.CommandParameters>,
        IReconcileServerCommand
    {

        /// <summary>
        /// Presents parameters passed to <see cref="UpdateWorkQueueCommand"/>
        /// </summary>
        internal class CommandParameters
        {
            public string   Extension;
            public bool     IsDuplicate;
            public string   SeriesInstanceUid;
            public string   SopInstanceUid;
        }

        public UpdateWorkQueueCommand(ReconcileStudyProcessorContext context, CommandParameters parameters)
            : base("Update/Insert a ReconcilePostProcess WorkQueue Entry", true, context, parameters)
        {
            Platform.CheckForNullReference(parameters, "parameters");

        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            Platform.CheckForNullReference(Context.DestStorageLocation, "Study Storage Location"); 
            
            IInsertWorkQueue insert = updateContext.GetBroker<IInsertWorkQueue>();
            InsertWorkQueueParameters parms = new InsertWorkQueueParameters();
        	parms.WorkQueueTypeEnum = WorkQueueTypeEnum.ReconcilePostProcess;
            parms.StudyStorageKey = Context.DestStorageLocation.GetKey();
            parms.ServerPartitionKey = Context.DestStorageLocation.ServerPartitionKey;
            parms.SeriesInstanceUid = Parameters.SeriesInstanceUid;
            parms.SopInstanceUid = Parameters.SopInstanceUid;
            parms.Duplicate = Parameters.IsDuplicate; 
            parms.ScheduledTime = Platform.Time;
            parms.ExpirationTime = Platform.Time.AddMinutes(5.0);
            parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;
            parms.Extension = Parameters.Extension;
            if (insert.FindOne(parms) == null)
                throw new ApplicationException("UpdateWorkQueueCommand failed");
        }

    }
}
