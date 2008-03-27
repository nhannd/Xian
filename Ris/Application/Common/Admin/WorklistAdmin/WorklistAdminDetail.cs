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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using System;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{


    [DataContract]
    public class WorklistAdminDetail : DataContractBase
    {
        [DataContract]
        public class TimePoint
        {
            public TimePoint(DateTime? fixedTime, long resolution)
            {
                this.FixedTime = fixedTime;
                this.Resolution = resolution;
            }

            public TimePoint(TimeSpan? relativeTime, long resolution)
            {
                this.RelativeTime = relativeTime;
                this.Resolution = resolution;
            }

            [DataMember]
            public DateTime? FixedTime;

            [DataMember]
            public TimeSpan? RelativeTime;

            [DataMember]
            public long Resolution;
        }


        public WorklistAdminDetail()
        {
            this.Users = new List<string>();
            this.ProcedureTypeGroups = new List<ProcedureTypeGroupSummary>();
            this.Facilities = new List<FacilitySummary>();
            this.PatientClasses = new List<EnumValueInfo>();
            this.OrderPriorities = new List<EnumValueInfo>();
            this.Portabilities = new List<bool>();
        }


        public WorklistAdminDetail(EntityRef entityRef, string name, string description, string worklistType)
            : this()
        {
            EntityRef = entityRef;
            Name = name;
            Description = description;
            WorklistType = worklistType;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public string WorklistType;

        [DataMember]
        public List<ProcedureTypeGroupSummary> ProcedureTypeGroups;

        [DataMember]
        public List<FacilitySummary> Facilities;

        [DataMember]
        public bool FilterByWorkingFacility;

        [DataMember]
        public List<EnumValueInfo> OrderPriorities;

        [DataMember]
        public List<EnumValueInfo> PatientClasses;

        [DataMember]
        public List<bool> Portabilities;

        [DataMember]
        public TimePoint StartTime;

        [DataMember]
        public TimePoint EndTime;

        [DataMember]
        public List<string> Users;
    }
}