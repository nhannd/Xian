using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class LoadModalityForEditRequest : DataContractBase
    {
        [DataMember]
        public EntityRef ModalityRef;
    }
}
