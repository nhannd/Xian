#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionDocument : Document
	{
		private readonly ReportingWorklistItemSummary _worklistItem;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private TranscriptionComponent _component;

		public TranscriptionDocument(ReportingWorklistItemSummary worklistItem, IReportingWorkflowItemToolContext context)
			: base(worklistItem.ProcedureStepRef, context.DesktopWindow)
		{
			_worklistItem = worklistItem;
			_folderName = context.SelectedFolder.Name;

			if(context.SelectedFolder is TranscriptionWorkflowFolder)
			{
				_worklistRef = ((TranscriptionWorkflowFolder)context.SelectedFolder).WorklistRef;
				_worklistClassName = ((TranscriptionWorkflowFolder)context.SelectedFolder).WorklistClassName;
			}
			else
			{
				_worklistRef = null;
				_worklistClassName = null;
			}
		}

		public override string GetTitle()
		{
			return TranscriptionDocument.GetTitle(_worklistItem);
		}

		public override bool SaveAndClose()
		{
			_component.SaveReport(true);
			return base.Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new TranscriptionComponent(_worklistItem, _folderName, _worklistRef, _worklistClassName);
			return _component;
		}

		public override OpenWorkspaceOperationAuditData GetAuditData()
		{
			return new OpenWorkspaceOperationAuditData("Transcription", _worklistItem);
		}

		public static string GetTitle(ReportingWorklistItemSummary item)
		{
			return string.Format("Transcription - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}

		public static string StripTitle(string title)
		{
			return title.Replace("Transcription - ", "");
		}
	}
}