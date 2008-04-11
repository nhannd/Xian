using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace $rootnamespace$
{
    [DataContract]
    public class $fileinputname$Detail : DataContractBase
    {
        public $fileinputname$Detail()
        {
        }

        public $fileinputname$Detail(EntityRef entityRef)
        {
            this.$fileinputname$Ref = entityRef;
        }

        [DataMember]
        public EntityRef $fileinputname$Ref;
    }
}