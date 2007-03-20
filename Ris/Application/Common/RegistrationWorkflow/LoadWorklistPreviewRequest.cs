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
        public LoadWorklistPreviewRequest(RegistrationWorklistItem item)
        {
            this.WorklistItem = item;
        }

        [DataMember]
        public RegistrationWorklistItem WorklistItem;
    }
}
