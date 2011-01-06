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
	//[MenuAction("activate", "global-menus/MenuTools/MenuToolsMyTools/SaveTool")]
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarSave", "Save")]
	[Tooltip("activate", "TooltipSave")]
	[IconSet("activate", IconScheme.Colour, "Icons.SaveToolSmall.png", "Icons.SaveToolSmall.png", "Icons.SaveToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class SaveTool : DicomEditorTool
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public SaveTool() : base(true) {}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Save()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmSaveAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				this.Context.DumpManagement.SaveAll();
				this.Context.UpdateDisplay();
			}
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}