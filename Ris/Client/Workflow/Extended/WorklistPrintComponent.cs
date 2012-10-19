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
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Extension point for views onto <see cref="WorklistPrintComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class WorklistPrintComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistPrintComponent class.
	/// </summary>
	[AssociateView(typeof(WorklistPrintComponentViewExtensionPoint))]
	public class WorklistPrintComponent : ApplicationComponent
	{
		private WorklistPrintViewComponent _worklistPrintPreviewComponent;
		private ChildComponentHost _worklistPrintPreviewComponentHost;

		private readonly WorklistPrintViewComponent.PrintContext _printContext;

		public WorklistPrintComponent(string folderSystemName, string folderName, string folderDescription, int totalCount, List<object> items)
		{
			_printContext = new WorklistPrintViewComponent.PrintContext(folderSystemName, folderName, folderDescription, totalCount, items);
		}

		public override void Start()
		{
			_worklistPrintPreviewComponent = new WorklistPrintViewComponent(_printContext);
			_worklistPrintPreviewComponentHost = new ChildComponentHost(this.Host, _worklistPrintPreviewComponent);
			_worklistPrintPreviewComponentHost.StartComponent();

			base.Start();
		}

		public ApplicationComponentHost WorklistPrintPreviewComponentHost
		{
			get { return _worklistPrintPreviewComponentHost; }
		}

		public void Print()
		{
			if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(SR.MessagePrintWorklist, MessageBoxActions.YesNo))
				return;

			// print the rendered document
			_worklistPrintPreviewComponent.PrintDocument();

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Close()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}
	}
}
