using System;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
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