using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [ServiceContract]
	public interface IWorklistService<TItemSummary>
		where TItemSummary : DataContractBase
    {
        [OperationContract]
        QueryWorklistResponse<TItemSummary> QueryWorklist(QueryWorklistRequest request);

		[OperationContract]
		TextQueryResponse<TItemSummary> SearchWorklists(WorklistTextQueryRequest request);
	}
}
