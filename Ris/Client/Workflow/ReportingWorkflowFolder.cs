#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class ReportingWorkflowFolder : WorklistFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
	{
		public ReportingWorkflowFolder()
			: base(new ReportingWorklistTable())
		{
		}
	}
}
