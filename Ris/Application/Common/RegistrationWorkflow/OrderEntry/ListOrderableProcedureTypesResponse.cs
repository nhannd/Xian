using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ListOrderableProcedureTypesResponse : DataContractBase
    {
        public ListOrderableProcedureTypesResponse(List<RequestedProcedureTypeSummary> orderableProcedureTypes)
        {
            this.OrderableProcedureTypes = orderableProcedureTypes;
        }

        /// <summary>
        /// The set of procedure types that can be additionally ordered.
        /// </summary>
        [DataMember]
        public List<RequestedProcedureTypeSummary> OrderableProcedureTypes;
    }
}
