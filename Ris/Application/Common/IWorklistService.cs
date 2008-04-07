using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common
{
    [ServiceContract]
    public interface IWorklistService<TItemSummary>
    {
        [OperationContract]
        QueryWorklistResponse<TItemSummary> QueryWorklist(QueryWorklistRequest request);
    }
}
