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
    public class ServerPartitionInsertParameters : ProcedureParameters
    {
        public ServerPartitionInsertParameters()
            : base("InsertServerPartition")
        {
        }

        public bool Enabled
        {
            set { SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }
        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }
        public String AeTitle
        {
            set { SubCriteria["AeTitle"] = new ProcedureParameter<String>("AeTitle", value); }
        }
        public int Port
        {
            set { SubCriteria["Port"] = new ProcedureParameter<int>("Port", value); }
        }
        public String PartitionFolder
        {
            set { SubCriteria["PartitionFolder"] = new ProcedureParameter<String>("PartitionFolder", value); }
        }
        public DuplicateSopPolicyEnum DuplicateSopPolicyEnum
        {
            set { SubCriteria["DuplicateSopPolicyEnum"] = new ProcedureParameter<ServerEnum>("DuplicateSopPolicyEnum", value); }
        }
        public int DefaultRemotePort
        {
            set { SubCriteria["DefaultRemotePort"] = new ProcedureParameter<int>("DefaultRemotePort", value); }
        }
        public bool AcceptAnyDevice
        {
            set { SubCriteria["AcceptAnyDevice"] = new ProcedureParameter<bool>("AcceptAnyDevice", value); }
        }
        public bool AutoInsertDevice
        {
            set { SubCriteria["AutoInsertDevice"] = new ProcedureParameter<bool>("AutoInsertDevice", value); }
        }
        public bool MatchPatientsName
        {
            set { SubCriteria["MatchPatientsName"] = new ProcedureParameter<bool>("MatchPatientsName", value); }
        }
        public bool MatchPatientId
        {
            set { SubCriteria["MatchPatientId"] = new ProcedureParameter<bool>("MatchPatientId", value); }
        }
        public bool MatchAccessionNumber
        {
            set { SubCriteria["MatchAccessionNumber"] = new ProcedureParameter<bool>("MatchAccessionNumber", value); }
        }
        public bool MatchPatientsBirthDate
        {
            set { SubCriteria["MatchPatientsBirthDate"] = new ProcedureParameter<bool>("MatchPatientsBirthDate", value); }
        }
        public bool MatchIssuerOfPatientId
        {
            set { SubCriteria["MatchIssuerOfPatientId"] = new ProcedureParameter<bool>("MatchIssuerOfPatientId", value); }
        }
        public bool MatchPatientsSex
        {
            set { SubCriteria["MatchPatientsSex"] = new ProcedureParameter<bool>("MatchPatientsSex", value); }
        }

        public bool AuditDeleteStudy
        {
            set { SubCriteria["AuditDeleteStudy"] = new ProcedureParameter<bool>("AuditDeleteStudy", value); }
        }
    }
}
