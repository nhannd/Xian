using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class Load$fileinputname$ForEditResponse : DataContractBase
    {
        public Load$fileinputname$ForEditResponse($fileinputname$Detail detail)
        {
            this.$fileinputname$ = detail;
        }

        [DataMember]
        public $fileinputname$Detail $fileinputname$;
    }
}
