using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class AddExternalPractitionerRequest : DataContractBase
    {
        public AddExternalPractitionerRequest(ExternalPractitionerDetail pracDetail)
        {
            this.PractitionerDetail = pracDetail;
        }

        [DataMember]
        public ExternalPractitionerDetail PractitionerDetail;
    }
}
