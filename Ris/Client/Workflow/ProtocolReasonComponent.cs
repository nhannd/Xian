using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocolReasonComponent : ReasonSelectionComponentBase
	{
		protected override List<EnumValueInfo> GetReasonChoices()
		{
			List<EnumValueInfo> choices = new List<EnumValueInfo>();

			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					GetSuspendRejectReasonChoicesResponse response =
						service.GetSuspendRejectReasonChoices(new GetSuspendRejectReasonChoicesRequest());
					choices.AddRange(response.SuspendRejectReasonChoices);
				});

			return choices;
		}
	}
}