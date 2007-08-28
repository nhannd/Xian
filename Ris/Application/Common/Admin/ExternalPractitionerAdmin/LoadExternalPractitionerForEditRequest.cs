using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class LoadExternalPractitionerForEditRequest : DataContractBase
    {
        public LoadExternalPractitionerForEditRequest(EntityRef pracRef)
        {
            this.PractitionerRef = pracRef;
        }

        [DataMember]
        public EntityRef PractitionerRef;
    }
}
