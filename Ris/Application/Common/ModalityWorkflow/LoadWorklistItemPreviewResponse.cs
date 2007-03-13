using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class LoadWorklistItemPreviewResponse : DataContractBase
    {
        public LoadWorklistItemPreviewResponse(ModalityWorklistPreview preview)
        {
            this.WorklistPreview = preview;
        }

        [DataMember]
        public ModalityWorklistPreview WorklistPreview;
    }
}
