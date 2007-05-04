using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PageRequestDetail : DataContractBase
    {
        public static readonly int Ignore = -1;

        public PageRequestDetail()
        {
            FirstRow = PageRequestDetail.Ignore;
            MaxRows = PageRequestDetail.Ignore;
        }

        [DataMember]
        public int FirstRow;

        [DataMember]
        public int MaxRows;
    }
}
