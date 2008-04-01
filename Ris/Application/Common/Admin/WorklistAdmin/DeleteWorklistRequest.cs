using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class DeleteWorklistRequest : DataContractBase
    {
        public DeleteWorklistRequest(EntityRef worklistRef)
        {
            WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;
    }
}
