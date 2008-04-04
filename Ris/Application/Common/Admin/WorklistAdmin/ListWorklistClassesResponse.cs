using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListWorklistClassesResponse : DataContractBase
    {
        public ListWorklistClassesResponse(List<WorklistClassSummary> worklistClasses)
        {
            WorklistClasses = worklistClasses;
        }

        [DataMember]
        public List<WorklistClassSummary> WorklistClasses;
    }
}
