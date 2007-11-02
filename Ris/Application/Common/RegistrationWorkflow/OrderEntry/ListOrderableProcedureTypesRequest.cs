using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ListOrderableProcedureTypesRequest : DataContractBase
    {
        public ListOrderableProcedureTypesRequest(List<EntityRef> orderedProcedureTypes)
        {
            this.OrderedProcedureTypes = orderedProcedureTypes;
        }

        /// <summary>
        /// The set of procedure types that are already ordered.
        /// </summary>
        [DataMember]
        public List<EntityRef> OrderedProcedureTypes;
    }
}
