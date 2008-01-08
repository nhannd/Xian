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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using ClearCanvas.ImageServer.Enterprise;

   public class SeriesUpdateColumns : EntityUpdateColumns
   {
       public SeriesUpdateColumns()
       : base("Series")
       {}
        public System.String Modality
        {
            set { SubParameters["Modality"] = new EntityUpdateColumn<System.String>("Modality", value); }
        }
        public System.Int32 NumberOfSeriesRelatedInstances
        {
            set { SubParameters["NumberOfSeriesRelatedInstances"] = new EntityUpdateColumn<System.Int32>("NumberOfSeriesRelatedInstances", value); }
        }
        public System.String PerformedProcedureStepStartDate
        {
            set { SubParameters["PerformedProcedureStepStartDate"] = new EntityUpdateColumn<System.String>("PerformedProcedureStepStartDate", value); }
        }
        public System.String PerformedProcedureStepStartTime
        {
            set { SubParameters["PerformedProcedureStepStartTime"] = new EntityUpdateColumn<System.String>("PerformedProcedureStepStartTime", value); }
        }
        public System.String SeriesDescription
        {
            set { SubParameters["SeriesDescription"] = new EntityUpdateColumn<System.String>("SeriesDescription", value); }
        }
        public System.String SeriesInstanceUid
        {
            set { SubParameters["SeriesInstanceUid"] = new EntityUpdateColumn<System.String>("SeriesInstanceUid", value); }
        }
        public System.String SeriesNumber
        {
            set { SubParameters["SeriesNumber"] = new EntityUpdateColumn<System.String>("SeriesNumber", value); }
        }
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartitionKey"] = new EntityUpdateColumn<ClearCanvas.ImageServer.Enterprise.ServerEntityKey>("ServerPartitionKey", value); }
        }
        public System.String SourceApplicationEntityTitle
        {
            set { SubParameters["SourceApplicationEntityTitle"] = new EntityUpdateColumn<System.String>("SourceApplicationEntityTitle", value); }
        }
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey StudyKey
        {
            set { SubParameters["StudyKey"] = new EntityUpdateColumn<ClearCanvas.ImageServer.Enterprise.ServerEntityKey>("StudyKey", value); }
        }
        public StudyStatusEnum StudyStatusEnum
        {
            set { SubParameters["StudyStatusEnum"] = new EntityUpdateColumn<StudyStatusEnum>("StudyStatusEnum", value); }
        }
    }
}
