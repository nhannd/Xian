using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class DiscontinueModalityPerformedProcedureStepRequest : DataContractBase
    {
        public DiscontinueModalityPerformedProcedureStepRequest(EntityRef mppsRef)
        {
            MppsRef = mppsRef;
        }

        [DataMember]
        public EntityRef MppsRef;
    }
}