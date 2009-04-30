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
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateWorkQueueParameters : ProcedureParameters
    {
        public UpdateWorkQueueParameters()
            : base("UpdateWorkQueue")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public WorkQueueStatusEnum WorkQueueStatusEnum
        {
            set { SubCriteria["WorkQueueStatusEnum"] = new ProcedureParameter<ServerEnum>("WorkQueueStatusEnum", value); }
        }

        public DateTime ExpirationTime 
        {
            set { SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public int FailureCount
        {
            set { SubCriteria["FailureCount"] = new ProcedureParameter<int>("FailureCount", value); }
        }

        public string ProcessorID
        {
            set { SubCriteria["ProcessorID"] = new ProcedureParameter<string>("ProcessorID", value); }
        }

        public string FailureDescription
        {
            set { SubCriteria["FailureDescription"] = new ProcedureParameter<string>("FailureDescription", value); }
        }

		public QueueStudyStateEnum QueueStudyStateEnum
		{
			set { SubCriteria["QueueStudyStateEnum"] = new ProcedureParameter<ServerEnum>("QueueStudyStateEnum", value); }
		}
    }
}
