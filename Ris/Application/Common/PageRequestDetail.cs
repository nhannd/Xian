using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PageRequestDetail : DataContractBase
    {
        [DataMember]
        public int FirstRow;

        [DataMember]
        public int MaxRows;
    }
}
