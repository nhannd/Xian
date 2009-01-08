using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionRejectReasonComponent : ReasonSelectionComponentBase
	{
		protected override List<EnumValueInfo> GetReasonChoices()
		{
			List<EnumValueInfo> choices = new List<EnumValueInfo>();

			Platform.GetService<ITranscriptionWorkflowService>(
				delegate(ITranscriptionWorkflowService service)
				{
					GetRejectReasonChoicesResponse response =
						service.GetRejectReasonChoices(new GetRejectReasonChoicesRequest());
					choices.AddRange(response.Choices);
				});

			return choices;
		}
	}
}