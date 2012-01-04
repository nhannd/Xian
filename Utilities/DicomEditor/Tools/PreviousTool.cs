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
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarPrevious", "Previous")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipPrevious")]
	[IconSet("activate", "Icons.PreviousToolSmall.png", "Icons.PreviousToolSmall.png", "Icons.PreviousToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class PreviousTool : DicomEditorTool
	{
		public PreviousTool() {}

		public void Previous()
		{
			this.Context.DumpManagement.LoadedFileDumpIndex -= 1;
			this.Context.UpdateDisplay();
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			this.Enabled = !(e.IsCurrentTheOnly || e.IsCurrentFirst);
		}
	}
}