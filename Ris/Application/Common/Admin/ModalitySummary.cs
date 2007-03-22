using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class ModalitySummary : DataContractBase
    {
        public ModalitySummary(EntityRef modalityRef, string id, string name)
        {
            this.ModalityRef = modalityRef;
            this.Id = id;
            this.Name = name;
            //this.Active = active;
        }

        [DataMember]
        public EntityRef ModalityRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        //[DataMember]
        //public bool Active;
    }
}
