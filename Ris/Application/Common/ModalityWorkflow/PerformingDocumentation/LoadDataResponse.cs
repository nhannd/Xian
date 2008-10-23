using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.PerformingDocumentation
{
    [DataContract]
    public class LoadDataResponse : DataContractBase
    {
        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public Dictionary<string, string> OrderExtendedProperties;

        [DataMember]
        public List<OrderNoteDetail> OrderNotes;

        /// <summary>
        /// Radiologist that has been assigned to read these procedures, or null if none has been assigned.
        /// </summary>
        [DataMember]
        public StaffSummary AssignedInterpreter;
    }
}
