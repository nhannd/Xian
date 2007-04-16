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
        public CheckInProcedureRequest(List<EntityRef> requestedProcedures)
        {
            this.RequestedProcedures = requestedProcedures;
        }

        [DataMember]
        public List<EntityRef> RequestedProcedures;
    }
}
