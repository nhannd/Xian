using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class StopModalityPerformedProcedureStepRequest : DataContractBase
    {
        public StopModalityPerformedProcedureStepRequest(EntityRef mppsRef, Dictionary<string, string> extendedProperties)
        {
            MppsRef = mppsRef;
            this.ExtendedProperties = extendedProperties;
        }

        [DataMember]
        public EntityRef MppsRef;

        [DataMember]
        public Dictionary<string, string> ExtendedProperties;
    }
}