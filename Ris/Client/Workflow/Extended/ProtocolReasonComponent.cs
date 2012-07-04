#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
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