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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarReplicate", "Replicate")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuReplicate", "Replicate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReplicate")]
	[IconSet("activate", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class ReplicateTool : DicomEditorTool
	{
		public ReplicateTool() : base(true) {}

		public void Replicate()
		{
			// if it's not a root level tag, it's part of a sequence. if it's not editable, it's SQ or OB or OW or UN or ?? and thus cannot be set by string value
			if (CollectionUtils.Contains(Context.SelectedTags, t => !t.IsRootLevelTag || !t.IsEditable()))
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageSequenceBinaryReplicationNotSupported, MessageBoxActions.Ok);
				return;
			}

			if (Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmReplicateTagsInAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				foreach (DicomEditorTag tag in Context.SelectedTags)
				{
					Context.DumpManagement.EditTag(tag.TagId, tag.Value, true);
				}
				Context.UpdateDisplay();
			}
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			this.Enabled = !e.IsCurrentTheOnly;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}