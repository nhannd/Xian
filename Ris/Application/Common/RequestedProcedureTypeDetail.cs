using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class RequestedProcedureTypeDetail : DataContractBase
    {
        public RequestedProcedureTypeDetail(string id, string name, List<ModalityProcedureStepTypeDetail> mpsTypes)
        {
            this.Id = id;
            this.Name = name;
            this.ModalityProcedureStepTypes = mpsTypes;
        }

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public List<ModalityProcedureStepTypeDetail> ModalityProcedureStepTypes;
    }
}
