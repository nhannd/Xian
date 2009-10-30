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
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertStudyIntegrityQueueParameters: ProcedureParameters
    {
        public InsertStudyIntegrityQueueParameters()
            : base("InsertStudyIntegrityQueue")
        {

        }

        public String StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }


        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public String SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<String>("SeriesInstanceUid", value); }
        }

        public String SopInstanceUid 
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<String>("SopInstanceUid", value); }
        }

        public String SeriesDescription
        {
            set { SubCriteria["SeriesDescription"] = new ProcedureParameter<String>("SeriesDescription", value); }
        }
        
        public XmlDocument StudyData
        {
            set { SubCriteria["StudyData"] = new ProcedureParameter<XmlDocument>("StudyData", value); }
        }

		public XmlDocument Details
        {
            set { SubCriteria["Details"] = new ProcedureParameter<XmlDocument>("Details", value); }
        }


        public ServerEnum StudyIntegrityReasonEnum
        {
            set { SubCriteria["StudyIntegrityReasonEnum"] = new ProcedureParameter<ServerEnum>("StudyIntegrityReasonEnum", value); }
        }

        public String GroupID
        {
            set { SubCriteria["GroupID"] = new ProcedureParameter<String>("GroupID", value); }
        }
        public String UidRelativePath
        {
            set { SubCriteria["UidRelativePath"] = new ProcedureParameter<String>("UidRelativePath", value); }
        }
	
    }
}
