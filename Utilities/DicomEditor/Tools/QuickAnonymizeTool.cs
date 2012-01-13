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
	[ButtonAction("activate", "dicomeditor-contextmenu/ToolbarQuickAnonymize", "Apply")]
	[MenuAction("activate", "dicomeditor-toolbar/MenuQuickAnonymize", "Apply")]
	[Tooltip("activate", "TooltipQuickAnonymize")]
	[IconSet("activate", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class QuickAnonymizeTool : DicomEditorTool
	{
		private bool _promptForAll;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public QuickAnonymizeTool() : base(true) {}

		public void Apply()
		{
			bool applyToAll = false;

			if (_promptForAll)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmAnonymizeAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
					applyToAll = true;
			}

			this.Anonymize(applyToAll);
			this.Context.UpdateDisplay();
		}

		private void Anonymize(bool applyToAll)
		{
			IDicomEditorDumpManagement dump = this.Context.DumpManagement;

			dump.Anonymize(applyToAll);
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			_promptForAll = !e.IsCurrentTheOnly;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}