#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReportingPreliminaryDiagnosisTool : PreliminaryDiagnosisConversationTool<ReportingWorklistItemSummary, IReportingWorkflowItemToolContext>
	{
		protected override string TemplatesXml
		{
			get
			{
				return PreliminaryDiagnosisSettings.Default.RadiologyTemplatesXml;
			}
		}

		protected override string SoftKeysXml
		{
			get
			{
				return PreliminaryDiagnosisSettings.Default.RadiologySoftKeysXml;
			}
		}
	}
}