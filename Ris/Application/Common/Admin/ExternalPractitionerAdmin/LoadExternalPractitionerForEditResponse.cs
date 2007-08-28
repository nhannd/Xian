using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class LoadExternalPractitionerForEditResponse : DataContractBase
    {
        public LoadExternalPractitionerForEditResponse(EntityRef pracRef, ExternalPractitionerDetail pracDetail)
        {
            this.PractitionerRef = pracRef;
            this.PractitionerDetail = pracDetail;
        }

        [DataMember]
        public EntityRef PractitionerRef;

        [DataMember]
        public ExternalPractitionerDetail PractitionerDetail;
    }
}
