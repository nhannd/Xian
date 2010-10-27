#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Open Documentation", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Open Documentation", "Apply")]
	[IconSet("apply", IconScheme.Colour, "PerformingOpenDocumentationSmall.png", "PerformingOpenDocumentationMedium.png", "PerformingOpenDocumentationLarge.png")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Create)]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class PerformingDocumentationTool : Tool<IPerformingWorkflowItemToolContext>
	{
		public override void Initialize()
		{ 
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		public bool Enabled
		{
			get
			{
				return this.Context.SelectedItems.Count == 1
					&& CollectionUtils.FirstElement(this.Context.SelectedItems).OrderRef != null;
			}
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		public void Apply()
		{
			try
			{
				var item = CollectionUtils.FirstElement(this.Context.SelectedItems);

				if (item != null && item.OrderRef != null)
				{
					var document = DocumentManager.Get<PerformingDocumentationDocument>(item.OrderRef);
					if (document == null)
					{
						document = new PerformingDocumentationDocument(item, this.Context.DesktopWindow);
						document.Open();

						var selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
						document.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
					}
					else
					{
						document.Open();
					}
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
