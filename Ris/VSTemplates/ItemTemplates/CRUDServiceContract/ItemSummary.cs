using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class $fileinputname$Summary : DataContractBase
    {
	    public $fileinputname$Summary()
		{
		}

        public $fileinputname$Summary(EntityRef entityRef)
        {
            this.$fileinputname$Ref = entityRef;
        }

        [DataMember]
        public EntityRef $fileinputname$Ref;
    }
}