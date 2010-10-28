#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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