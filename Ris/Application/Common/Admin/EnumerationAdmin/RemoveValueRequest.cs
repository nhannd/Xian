using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [DataContract]
    public class RemoveValueRequest : DataContractBase
    {
        public RemoveValueRequest()
        {

        }

        public RemoveValueRequest(string enumerationName, EnumValueInfo value)
        {
            this.AssemblyQualifiedClassName = enumerationName;
            this.Value = value;
        }

        [DataMember]
        public string AssemblyQualifiedClassName;

        [DataMember]
        public EnumValueInfo Value;
    }
}
