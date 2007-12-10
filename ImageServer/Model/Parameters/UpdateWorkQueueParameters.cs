#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;
using System.Xml;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateWorkQueueParameters : UpdateBrokerParameters
    {
        public UpdateWorkQueueParameters()
            : base("WorkQueue")
        {
        }


        public ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartition"] = new UpdateBrokerParameter<ServerEntityKey>("ServerPartition", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubParameters["StudyStorage"] = new UpdateBrokerParameter<ServerEntityKey>("StudyStorage", value); }
        }


        public Device Device
        {
            set { SubParameters["Device"] = new UpdateBrokerParameter<ServerEntity>("Device", value); }
        }

        public WorkQueueTypeEnum WorkQueueType
        {
            set { SubParameters["WorkQueueType"] = new UpdateBrokerParameter<ServerEnum>("WorkQueueType", value); }
        }

        public WorkQueueStatusEnum WorkQueueStatus
        {
            set { SubParameters["WorkQueueStatus"] = new UpdateBrokerParameter<ServerEnum>("WorkQueueStatus", value); }
        }



        public String ProcessorID
        {
            set { SubParameters["ProcessorID"] = new UpdateBrokerParameter<String>("ProcessorID", value); }
        }

        public DateTime ExpirationTime
        {
            set { SubParameters["ExpirationTime"] = new UpdateBrokerParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubParameters["ScheduledTime"] = new UpdateBrokerParameter<DateTime>("ScheduledTime", value); }
        }

        public DateTime InsertTime
        {
            set { SubParameters["InsertTime"] = new UpdateBrokerParameter<DateTime>("InsertTime", value); }
        }

        public int FailureCount
        {
            set { SubParameters["FailureCount"] = new UpdateBrokerParameter<int>("FailureCount", value); }
        }

        public XmlDocument Data
        {
            set { SubParameters["Data"] = new UpdateBrokerParameter<XmlDocument>("Data", value); }
        }

    }
}
