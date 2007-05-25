using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ModalityProcedureStepTypeDetail : DataContractBase
    {
        public ModalityProcedureStepTypeDetail(string id, string name, ModalityDetail defaultModality)
        {
            this.Id = id;
            this.Name = name;
            this.DefaultModality = defaultModality;
        }

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public ModalityDetail DefaultModality;
    }
}
