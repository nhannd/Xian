using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ModalityProcedureStepSummary : DataContractBase
    {
        public ModalityProcedureStepSummary()
        {
        }

        [DataMember]
        public EnumValueInfo State;

        [DataMember]
        public StaffSummary ScheduledPerformerStaff;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public DateTime? ScheduledEndTime;

        [DataMember]
        public StaffSummary PerformerStaff;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public ModalityProcedureStepTypeDetail Type;
    }
}
