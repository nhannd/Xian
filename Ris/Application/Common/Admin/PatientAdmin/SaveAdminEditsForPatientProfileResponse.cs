using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class SaveAdminEditsForPatientProfileResponse : DataContractBase
    {
        public SaveAdminEditsForPatientProfileResponse(RegistrationWorklistItem worklistItem)
        {
            this.WorklistItem = worklistItem;
        }

        [DataMember]
        public RegistrationWorklistItem WorklistItem;
    }
}
