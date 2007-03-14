using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class LoadWorklistPreviewResponse : DataContractBase
    {
        public LoadWorklistPreviewResponse(RegistrationWorklistPreview preview)
        {
            this.WorklistPreview = preview;
        }

        [DataMember]
        public RegistrationWorklistPreview WorklistPreview;
    }
}
