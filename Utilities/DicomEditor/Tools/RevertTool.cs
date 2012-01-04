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

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarRevert", "Revert")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuRevert", "Revert")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRevert")]
	[IconSet("activate", "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class RevertTool : DicomEditorTool
	{
		private bool _promptForAll;

		public RevertTool() : base(true) {}

		public void Revert()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevert, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				bool revertAll = false;

				if (_promptForAll)
				{
					if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevertAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
						revertAll = true;
				}

				this.Context.DumpManagement.RevertEdits(revertAll);
				this.Context.UpdateDisplay();
				this.Enabled = false;
			}
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			_promptForAll = !e.IsCurrentTheOnly;
			this.Enabled = e.HasCurrentBeenEdited;
		}

		protected override void OnTagEdited(object sender, EventArgs e)
		{
			this.Enabled = true;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}