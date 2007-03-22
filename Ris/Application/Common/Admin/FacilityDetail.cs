using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class FacilityDetail : DataContractBase
    {
        public FacilityDetail(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }

        public FacilityDetail()
        {
        }

        [DataMember]
        public string Code;

        [DataMember]
        public string Name;
    }
}