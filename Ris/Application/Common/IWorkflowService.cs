using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[ServiceContract]
	public interface IWorkflowService
	{
		[OperationContract]
		GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);
	}
}
