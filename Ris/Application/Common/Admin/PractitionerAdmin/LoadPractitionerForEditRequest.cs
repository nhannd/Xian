using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class LoadPractitionerForEditRequest : DataContractBase
    {
        [DataMember]
        public EntityRef PractitionerRef;
    }
}
