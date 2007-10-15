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

using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    /// <summary>
    /// Criteria for selects against the <see cref="Patient"/> table.
    /// </summary>
    public class PatientSelectCriteria: SelectCriteria
    {
        public PatientSelectCriteria()
            : base("Patient")
        {}

        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ServerPartitionKey"))
                {
                    this.SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["ServerPartitionKey"];
            }
        }

        [DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientName
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientName"))
                {
                    this.SubCriteria["PatientName"] = new SearchCondition<string>("PatientName");
                }
                return (ISearchCondition<string>)this.SubCriteria["PatientName"];
            }
        }

        [DicomField(DicomTags.PatientId, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientId"))
                {
                    this.SubCriteria["PatientId"] = new SearchCondition<string>("PatientId");
                }
                return (ISearchCondition<string>)this.SubCriteria["PatientId"];
            }
        }

        [DicomField(DicomTags.IssuerOfPatientId, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> IssuerOfPatientId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("IssuerOfPatientId"))
                {
                    this.SubCriteria["IssuerOfPatientId"] = new SearchCondition<string>("IssuerOfPatientId");
                }
                return (ISearchCondition<string>)this.SubCriteria["IssuerOfPatientId"];
            }
        }
    }
}
