using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [DataContract]
    public class ListEnumerationValuesRequest : DataContractBase
    {
        public ListEnumerationValuesRequest()
        {

        }

        public ListEnumerationValuesRequest(string enumerationName)
        {
            this.AssemblyQualifiedClassName = enumerationName;
        }

        [DataMember]
        public string AssemblyQualifiedClassName;
    }
}
