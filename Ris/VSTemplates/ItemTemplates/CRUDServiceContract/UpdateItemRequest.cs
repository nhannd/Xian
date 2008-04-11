using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class Update$fileinputname$Request : DataContractBase
    {
        public Update$fileinputname$Request($fileinputname$Detail detail)
        {
            this.$fileinputname$ = detail;
        }

        [DataMember]
        public $fileinputname$Detail $fileinputname$;
    }
}
