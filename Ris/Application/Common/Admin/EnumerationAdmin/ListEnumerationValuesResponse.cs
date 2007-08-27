using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [DataContract]
    public class ListEnumerationValuesResponse : DataContractBase
    {
        public ListEnumerationValuesResponse()
        {

        }

        public ListEnumerationValuesResponse(List<EnumValueInfo> values)
        {
            this.Values = values;
        }

        [DataMember]
        public List<EnumValueInfo> Values;
    }
}
