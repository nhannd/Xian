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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// Dummy PPS object
    /// </summary>
    [DataContract]
    public class PerformedProcedureStepDetail : IEquatable<PerformedProcedureStepDetail>
    {
        public PerformedProcedureStepDetail()
            : this(Platform.Time)
        {       
        }

        public PerformedProcedureStepDetail(
            DateTime startTime)
        {
            PpsRef = null;
            IsNew = true;
            CreationTime = Platform.Time;
            LastUpdateTime = Platform.Time;
            StartTime = startTime;
            EndTime = null;
            Blob = null;
        }

        public PerformedProcedureStepDetail(
            EntityRef ppsRef, 
            DateTime creationTime, 
            DateTime lastUpdateTime, 
            DateTime startTime, 
            DateTime? endTime, 
            string blob)
        {
            PpsRef = ppsRef;
            IsNew = false;
            CreationTime = creationTime;
            LastUpdateTime = lastUpdateTime;
            StartTime = startTime;
            EndTime = endTime;
            Blob = blob;
        }

        [DataMember] 
        public EntityRef PpsRef;

        [DataMember]
        public bool IsNew;

        [DataMember]
        public DateTime CreationTime;

        [DataMember]
        public DateTime LastUpdateTime;

        [DataMember]
        public StaffSummary Staff; 

        [DataMember]
        public DateTime StartTime;

        [DataMember] 
        public DateTime? EndTime;

        [DataMember]
        public string Blob;

        public bool Equals(PerformedProcedureStepDetail performedProcedureStepDetail)
        {
            if (performedProcedureStepDetail == null) return false;
            if (!Equals(CreationTime, performedProcedureStepDetail.CreationTime)) return false;
            if (!Equals(LastUpdateTime, performedProcedureStepDetail.LastUpdateTime)) return false;
            if (!Equals(StartTime, performedProcedureStepDetail.StartTime)) return false;
            if (!Equals(EndTime, performedProcedureStepDetail.EndTime)) return false;
            if (!Equals(Blob, performedProcedureStepDetail.Blob)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as PerformedProcedureStepDetail);
        }

        public override int GetHashCode()
        {
            int result = CreationTime.GetHashCode();
            result = 29*result + LastUpdateTime.GetHashCode();
            result = 29*result + StartTime.GetHashCode();
            result = 29*result + EndTime.GetHashCode();
            result = 29*result + (Blob != null ? Blob.GetHashCode() : 0);
            return result;
        }
    }
}