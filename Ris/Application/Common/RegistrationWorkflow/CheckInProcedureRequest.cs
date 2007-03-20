using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class CheckInProcedureRequest : DataContractBase
    {
        public CheckInProcedureRequest(List<EntityRef> requestedProcedures, EntityRef staffRef)
        {
            this.RequestedProcedures = requestedProcedures;
            this.Staff = staffRef;
        }

        [DataMember(IsRequired = true)]
        public List<EntityRef> RequestedProcedures;

        [DataMember(IsRequired = true)]
        public EntityRef Staff;
    }
}
