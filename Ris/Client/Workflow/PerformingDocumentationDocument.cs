#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class PerformingDocumentationDocument : Document
	{
		private readonly ModalityWorklistItemSummary _item;
		private PerformingDocumentationComponent _component;

		public PerformingDocumentationDocument(ModalityWorklistItemSummary item, IDesktopWindow desktopWindow)
			: base(item.OrderRef, desktopWindow)
		{
			if(item == null)
			{
				throw new ArgumentNullException("item");
			}

			_item = item;
		}

		public override string GetTitle()
		{
			return string.Format("Performing - {0} - {1}", PersonNameFormat.Format(_item.PatientName), MrnFormat.Format(_item.Mrn));
		}

		public override bool SaveAndClose()
		{
			_component.SaveDocumentation();
			return base.Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new PerformingDocumentationComponent(_item);
			return _component;
		}

		public override OpenWorkspaceOperationAuditData GetAuditData()
		{
			return new OpenWorkspaceOperationAuditData("Performing", _item);
		}
	}
}