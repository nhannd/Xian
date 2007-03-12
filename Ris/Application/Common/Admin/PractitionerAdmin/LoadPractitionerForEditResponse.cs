using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class LoadPractitionerForEditResponse : DataContractBase
    {
        public LoadPractitionerForEditResponse(EntityRef practitionerRef, StaffDetail practitionerDetail)
        {
            this.PractitionerRef = practitionerRef;
            this.PractitionerDetail = practitionerDetail;
        }

        [DataMember]
        public EntityRef PractitionerRef;

        [DataMember]
        public PractitionerDetail PractitionerDetail;
    }
}
