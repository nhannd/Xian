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

        public PageRequestDetail(int firstRow, int maxRows)
        {
            this.FirstRow = firstRow;
            this.MaxRows = maxRows;
        }

        [DataMember]
        public int FirstRow;

        [DataMember]
        public int MaxRows;
    }
}
