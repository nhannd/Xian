using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class List$fileinputname$sResponse : DataContractBase
    {
        public List$fileinputname$sResponse(List<$fileinputname$Summary> $fileinputname$s)
        {
            this.$fileinputname$s = $fileinputname$s;
        }

        [DataMember]
        public List<$fileinputname$Summary> $fileinputname$s;
    }
}
