#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
        public bool AcceptLatestReport
        {
            set { SubCriteria["AcceptLatestReport"] = new ProcedureParameter<bool>("AcceptLatestReport", value); }
        }
    }
}
