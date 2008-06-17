using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[ServiceContract]
	public interface IWorkflowService<TItemSummary>
		where TItemSummary : DataContractBase
	{
		[OperationContract]
		GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest<TItemSummary> request);
	}
}
