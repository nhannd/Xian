using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class List$fileinputname$sRequest : PagedDataContractBase
    {
        public List$fileinputname$sRequest()
        {
        }

        public List$fileinputname$sRequest(SearchResultPage page)
            :base(page)
        {
        }
    }
}
