using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class LoadWorklistPreviewRequest : DataContractBase
    {
        [DataMember]
        public RegistrationWorklistItem WorklistItem;
    }
}
