using System;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    /// <summary>
    /// Dummy PPS object
    /// </summary>
    [DataContract]
    public class PerformedProcedureStepDetail //: IEquatable<PerformedProcedureStepDetail>
    {
        public PerformedProcedureStepDetail()
            : this(null, null)
        {
        }

        public PerformedProcedureStepDetail(EntityRef ppsRef, string blob)
        {
            this.PpsRef = ppsRef;
            this.Blob = blob;
            this.StartTime = Platform.Time;
        }

        [DataMember] 
        public EntityRef PpsRef;

        [DataMember]
        public string Blob;

        [DataMember]
        public DateTime StartTime;

        [DataMember] 
        public DateTime? EndTime;

        //#region IEquatable

        //public bool Equals(PerformedProcedureStepDetail performedProcedureStepDetail)
        //{
        //    if (performedProcedureStepDetail == null) return false;
        //    return Equals(PpsRef, performedProcedureStepDetail.PpsRef);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (this == obj) return true;
        //    return Equals(obj as PerformedProcedureStepDetail);
        //}

        //public override int GetHashCode()
        //{
        //    return PpsRef != null ? PpsRef.GetHashCode() : 0;
        //}

        //#endregion
    }
}