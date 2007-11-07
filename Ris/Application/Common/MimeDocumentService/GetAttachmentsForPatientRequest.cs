using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.MimeDocumentService
{
    [DataContract]
    public class GetAttachmentsForPatientRequest : DataContractBase
    {
        public GetAttachmentsForPatientRequest(EntityRef patientRef)
        {
            this.PatientRef = patientRef;
        }

        [DataMember]
        public EntityRef PatientRef;
    }
}
