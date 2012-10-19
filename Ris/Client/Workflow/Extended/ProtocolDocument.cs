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
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Document container for <see cref="ProtocolEditorComponent"/>
	/// </summary>
	class ProtocolDocument : Document
	{
		#region Private Members

		private readonly ReportingWorklistItemSummary _item;
		private readonly IContinuousWorkflowComponentMode _mode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private ProtocollingComponent _component;

		#endregion

		#region Constructor

		public ProtocolDocument(ReportingWorklistItemSummary item, IContinuousWorkflowComponentMode mode, IReportingWorkflowItemToolContext context)
			: base(item.OrderRef, context.DesktopWindow)
		{
			_item = item;
			_mode = mode;
			_folderName = context.SelectedFolder.Name;

			if (context.SelectedFolder is ReportingWorkflowFolder)
			{
				_worklistRef = ((ReportingWorkflowFolder)context.SelectedFolder).WorklistRef;
				_worklistClassName = ((ReportingWorkflowFolder)context.SelectedFolder).WorklistClassName;
			}
			else
			{
				_worklistRef = null;
				_worklistClassName = null;
			}
		}

		#endregion

		#region Document overrides

		public override string GetTitle()
		{
			return GetTitle(_item);
		}

		public override bool SaveAndClose()
		{
			_component.Save(true);
			return Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new ProtocollingComponent(_item, _mode, _folderName, _worklistRef, _worklistClassName);
			return _component;
		}

		public override OpenWorkspaceOperationAuditData GetAuditData()
		{
			return new OpenWorkspaceOperationAuditData("Protocolling", _item);
		}

		#endregion

		public static string GetTitle(ReportingWorklistItemSummary item)
		{
			return string.Format("Protocol - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}

		public static string StripTitle(string title)
		{
			return title.Replace("Protocol - ", "");
		}
	}
}
