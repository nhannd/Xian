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
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarDelete", "Delete")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuDelete", "Delete")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDelete")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class DeleteTool : DicomEditorTool
	{
		private bool _promptForAll;

		public DeleteTool() : base(true) {}

		public void Delete()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteSelectedTags, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				bool tagDeleted = false;
				bool applyToAll = false;

				if (_promptForAll)
				{
					if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteSelectedTagsFromAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
						applyToAll = true;
				}

				foreach (DicomEditorTag tag in this.Context.SelectedTags)
				{
					if (tag.TagId != 0)
					{
						this.Context.DumpManagement.DeleteTag(tag.TagId, applyToAll);
						tagDeleted = true;
					}
				}

				if (tagDeleted)
					this.Context.UpdateDisplay();
				else
					this.Context.DesktopWindow.ShowMessageBox(SR.MessageNoTagsWereDeleted, MessageBoxActions.Ok);
			}
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