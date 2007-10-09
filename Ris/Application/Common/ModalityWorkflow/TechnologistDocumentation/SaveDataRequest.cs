using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class SaveDataRequest : DataContractBase
    {
        public SaveDataRequest(EntityRef orderRef, Dictionary<string, string> orderExtendedProperties)
        {
            this.OrderRef = orderRef;
            this.OrderExtendedProperties = orderExtendedProperties;
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public Dictionary<string, string> OrderExtendedProperties;
    }
}
