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

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateSeriesParameters : UpdateBrokerParameters
    {
        public UpdateSeriesParameters()
            : base("Series")
        {
        }


        public ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartition"] = new UpdateBrokerParameter<ServerEntityKey>("ServerPartition", value); }
        }

        public Study Study
        {
            set { SubParameters["Study"] = new UpdateBrokerParameter<ServerEntity>("Study", value); }
        }


        public String SeriesInstanceUid
        {
            set { SubParameters["SeriesInstanceUid"] = new UpdateBrokerParameter<String>("SeriesInstanceUid", value); }
        }

        public String Modality
        {
            set { SubParameters["Modality"] = new UpdateBrokerParameter<String>("Modality", value); }
        }

        public String SeriesNumber
        {
            set { SubParameters["SeriesNumber"] = new UpdateBrokerParameter<String>("SeriesNumber", value); }
        }

        public String SeriesDescription
        {
            set { SubParameters["SeriesDescription"] = new UpdateBrokerParameter<String>("SeriesDescription", value); }
        }

        public String PerformedProcedureStepStartDate
        {
            set { SubParameters["PerformedProcedureStepStartDate"] = new UpdateBrokerParameter<String>("PerformedProcedureStepStartDate", value); }
        }

        public String PerformedProcedureStepStartTime
        {
            set { SubParameters["PerformedProcedureStepStartTime"] = new UpdateBrokerParameter<String>("PerformedProcedureStepStartTime", value); }
        }
    
        public String SourceAeTitle
        {
            set { SubParameters["SourceAeTitle"] = new UpdateBrokerParameter<String>("SourceAeTitle", value); }
        }
    
        public int NumberOfSeriesRelatedInstances
        {
            set { SubParameters["NumberOfSeriesRelatedInstances"] = new UpdateBrokerParameter<int>("NumberOfSeriesRelatedInstances", value); }
        }


        public StudyStatusEnum StudyStatusEnum
        {
            set { SubParameters["StudyStatus"] = new UpdateBrokerParameter<ServerEnum>("StudyStatus", value); }
        }
        
        
    }
}
