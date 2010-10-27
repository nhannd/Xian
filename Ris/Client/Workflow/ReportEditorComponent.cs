#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReportEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReportEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ReportEditorComponentViewExtensionPoint))]
	public class ReportEditorComponent : ReportEditorComponentBase<IReportEditorContext, ReportEditorCloseReason>, IReportEditor
	{
		public ReportEditorComponent(IReportEditorContext context)
			: base(context)
		{
		}

		protected override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.ReportPreviewPageUrl; }
		}
	}
}
