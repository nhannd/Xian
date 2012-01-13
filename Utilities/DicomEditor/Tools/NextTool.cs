#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarNext", "Next")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipNext")]
	[IconSet("activate", "Icons.NextToolSmall.png", "Icons.NextToolSmall.png", "Icons.NextToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class NextTool : DicomEditorTool
	{
		public NextTool() {}

		public void Next()
		{
			this.Context.DumpManagement.LoadedFileDumpIndex += 1;
			this.Context.UpdateDisplay();
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			this.Enabled = !(e.IsCurrentTheOnly || e.IsCurrentLast);
		}
	}
}