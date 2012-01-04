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
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarCreate", "Create")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuCreate", "Create")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipCreate")]
	[IconSet("activate", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class CreateTool : DicomEditorTool
	{
		public CreateTool() : base(true) {}

		public void Create()
		{
			DicomEditorCreateToolComponent creator = new DicomEditorCreateToolComponent();
			ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, creator, SR.TitleCreateTag);
			if (result == ApplicationComponentExitCode.Accepted)
			{
				try
				{
					this.Context.DumpManagement.EditTag(creator.TagId, creator.Value, false);
				}
				catch (DicomException)
				{
					this.Context.DesktopWindow.ShowMessageBox(SR.MessageTagCannotBeCreated, MessageBoxActions.Ok);
					return;
				}

				this.Context.UpdateDisplay();
			}
		}

		protected override void OnSelectedTagChanged(object sender, EventArgs e)
		{
			if (this.Context.SelectedTags != null && this.Context.SelectedTags.Count > 1)
				this.Enabled = false;
			else
				this.Enabled = true;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}