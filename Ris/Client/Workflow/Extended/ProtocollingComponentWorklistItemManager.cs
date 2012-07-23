#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public class ProtocollingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItemSummary, IReportingWorkflowService>
	{
		public ProtocollingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode<TWorklistITem>(ReportingWorklistItemSummary worklistItem)
		{
			throw new NotSupportedException("Protocolling component mode should be initialized externally.  ReportingWorklistItemSummary does not have enough context.");
		}

		protected override string TaskName
		{
			get { return "Protocolling"; }
		}
	}

	public static class ProtocollingComponentModes
	{
		public static IContinuousWorkflowComponentMode Assign = new AssignProtocolMode();
		public static IContinuousWorkflowComponentMode Edit = new EditProtocolMode();
		public static IContinuousWorkflowComponentMode Review = new ReviewProtocolMode();
	}

	public class AssignProtocolMode : ContinuousWorkflowComponentMode
	{
		public AssignProtocolMode()
			: base(true, true, true)
		{
		}
	}

	public class EditProtocolMode : ContinuousWorkflowComponentMode
	{
		public EditProtocolMode()
			: base(false, false, false)
		{
		}
	}

	public class ReviewProtocolMode : ContinuousWorkflowComponentMode
	{
		public ReviewProtocolMode()
			: base(false, false, false)
		{
		}
	}
}