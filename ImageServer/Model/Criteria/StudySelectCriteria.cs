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
    /// Criteria for sleects against the <see cref="Study"/> table.
    /// </summary>
    public class StudySelectCriteria : SelectCriteria
    {
        public StudySelectCriteria()
            : base("Study")
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

        public ISearchCondition<ServerEntityKey> PatientKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientKey"))
                {
                    this.SubCriteria["PatientKey"] = new SearchCondition<ServerEntityKey>("PatientKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["PatientKey"];
            }
        }

        [DicomField(DicomTags.StudyInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyInstanceUid
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyInstanceUid"))
                {
                    this.SubCriteria["StudyInstanceUid"] = new SearchCondition<string>("StudyInstanceUid");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyInstanceUid"];
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
                return (ISearchCondition<string>) this.SubCriteria["PatientId"];
            }
        }

        [DicomField(DicomTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientsBirthDate
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientsBirthDate"))
                {
                    this.SubCriteria["PatientsBirthDate"] = new SearchCondition<string>("PatientsBirthDate");
                }
                return (ISearchCondition<string>) this.SubCriteria["PatientsBirthDate"];
            }
        }

        [DicomField(DicomTags.PatientsSex, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientsSex
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientsSex"))
                {
                    this.SubCriteria["PatientsSex"] = new SearchCondition<string>("PatientsSex");
                }
                return (ISearchCondition<string>)this.SubCriteria["PatientsSex"];
            }
        }

        [DicomField(DicomTags.StudyDate, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyDate
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyDate"))
                {
                    this.SubCriteria["StudyDate"] = new SearchCondition<string>("StudyDate");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyDate"];
            }
        }

        [DicomField(DicomTags.StudyTime, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyTime"))
                {
                    this.SubCriteria["StudyTime"] = new SearchCondition<string>("StudyTime");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyTime"];
            }
        }

        [DicomField(DicomTags.AccessionNumber, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> AccessionNumber
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("AccessionNumber"))
                {
                    this.SubCriteria["AccessionNumber"] = new SearchCondition<string>("AccessionNumber");
                }
                return (ISearchCondition<string>)this.SubCriteria["AccessionNumber"];
            }
        }

        [DicomField(DicomTags.StudyId, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyId"))
                {
                    this.SubCriteria["StudyId"] = new SearchCondition<string>("StudyId");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyId"];
            }
        }

        [DicomField(DicomTags.StudyDescription, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyDescription
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyDescription"))
                {
                    this.SubCriteria["StudyDescription"] = new SearchCondition<string>("StudyDescription");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyDescription"];
            }
        }

        [DicomField(DicomTags.ReferringPhysiciansName, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> ReferringPhysiciansName
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReferringPhysiciansName"))
                {
                    this.SubCriteria["ReferringPhysiciansName"] = new SearchCondition<string>("ReferringPhysiciansName");
                }
                return (ISearchCondition<string>)this.SubCriteria["ReferringPhysiciansName"];
            }
        }

        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the Series table.
        /// </summary>
        /// <remarks>
        /// A <see cref="SeriesSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="Series"/> tables is automatically added into the <see cref="SeriesSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<SelectCriteria> SeriesRelatedEntityCondition
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesRelatedEntityCondition"))
                {
                    this.SubCriteria["SeriesRelatedEntityCondition"] = new RelatedEntityCondition<SelectCriteria>("SeriesRelatedEntityCondition");
                }
                return (IRelatedEntityCondition<SelectCriteria>)this.SubCriteria["SeriesRelatedEntityCondition"];
            }
        }
    }
}
