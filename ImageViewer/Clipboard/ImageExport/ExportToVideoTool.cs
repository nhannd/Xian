#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToVideo", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToVideo", "Export")]
	[Tooltip("export", "TooltipExportToVideo")]
	[IconSet("export", "Icons.ExportToVideoToolSmall.png", "Icons.ExportToVideoToolSmall.png", "Icons.ExportToVideoToolSmall.png")]
	[EnabledStateObserver("export", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("export", AuthorityTokens.Clipboard.Export.AVI)]
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class ExportToVideoTool : ClipboardTool
	{
		public ExportToVideoTool()
		{
		}

		private void UpdateEnabled()
		{
			bool isOneItemSelected = this.Context.SelectedClipboardItems.Count == 1;
			bool selectedItemIsDisplaySet = isOneItemSelected && this.Context.SelectedClipboardItems[0].Item is IDisplaySet;

			this.Enabled = isOneItemSelected && selectedItemIsDisplaySet;
		}

		public override void Initialize()
		{
			base.Initialize();
		
			UpdateEnabled();
		}

		protected override void  OnSelectionChanged()
		{
			UpdateEnabled();
		}

		public void Export()
		{
			try
			{
				AviExportComponent.Launch(this.Context.DesktopWindow, (ClipboardItem) this.Context.SelectedClipboardItems[0]);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageExportToVideoFailed, Context.DesktopWindow);
			}
		}
	}
}
