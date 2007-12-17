#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateStudyParameters : UpdateBrokerParameters
    {
        public UpdateStudyParameters()
            : base("Study")
        {
        }


        public ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartition"] = new UpdateBrokerParameter<ServerEntityKey>("ServerPartition", value); }
        }

        public ServerEntityKey PatientKey
        {
            set { SubParameters["Patient"] = new UpdateBrokerParameter<ServerEntityKey>("Patient", value); }
        }


        public String StudyInstanceUid
        {
            set { SubParameters["StudyInstanceUid"] = new UpdateBrokerParameter<String>("StudyInstanceUid", value); }
        }

        public String PatientName
        {
            set { SubParameters["PatientName"] = new UpdateBrokerParameter<String>("PatientName", value); }
        }

        public String PatientId
        {
            set { SubParameters["PatientId"] = new UpdateBrokerParameter<String>("PatientId", value); }
        }

        public String PatientsBirthDate
        {
            set { SubParameters["PatientsBirthDate"] = new UpdateBrokerParameter<String>("PatientsBirthDate", value); }
        }

        public String PatientsSex
        {
            set { SubParameters["PatientsSex"] = new UpdateBrokerParameter<String>("PatientsSex", value); }
        }

        public String StudyDate
        {
            set { SubParameters["StudyDate"] = new UpdateBrokerParameter<String>("StudyDate", value); }
        }

        public String StudyTime
        {
            set { SubParameters["StudyTime"] = new UpdateBrokerParameter<String>("StudyTime", value); }
        }

        public String AccessionNumber
        {
            set { SubParameters["AccessionNumber"] = new UpdateBrokerParameter<String>("AccessionNumber", value); }
        }
        public String StudyId
        {
            set { SubParameters["StudyId"] = new UpdateBrokerParameter<String>("StudyId", value); }
        }
        public String StudyDescription
        {
            set { SubParameters["StudyDescription"] = new UpdateBrokerParameter<String>("StudyDescription", value); }
        }
        public String ReferringPhysiciansName
        {
            set { SubParameters["ReferringPhysiciansName"] = new UpdateBrokerParameter<String>("ReferringPhysiciansName", value); }
        }

        public int NumberOfStudyRelatedSeries
        {
            set { SubParameters["NumberOfStudyRelatedSeries"] = new UpdateBrokerParameter<int>("NumberOfStudyRelatedSeries", value); }
        }

        public int NumberOfStudyRelatedInstances
        {
            set { SubParameters["NumberOfStudyRelatedInstances"] = new UpdateBrokerParameter<int>("NumberOfStudyRelatedInstances", value); }
        }
        public StudyStatusEnum StudyStatusEnum
        {
            set { SubParameters["StudyStatus"] = new UpdateBrokerParameter<ServerEnum>("StudyStatus", value); }
        }
        
        
    }
}
