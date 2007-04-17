using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public abstract class PagedDataContractBase : DataContractBase
    {
        private PageRequestDetail _pageRequest;

        [DataMember]
        public PageRequestDetail PageRequest
        {
            get
            {
                if (_pageRequest == null)
                {
                    _pageRequest = new PageRequestDetail();
                }
                return _pageRequest;
            }
            set
            {
                _pageRequest = value;
            }
        }
    }
}
