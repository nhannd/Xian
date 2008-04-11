using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class Load$fileinputname$ForEditRequest : DataContractBase
    {
        public Load$fileinputname$ForEditRequest(EntityRef entityRef)
        {
            this.$fileinputname$Ref = entityRef;
        }

        [DataMember]
        public EntityRef $fileinputname$Ref;
    }
}
