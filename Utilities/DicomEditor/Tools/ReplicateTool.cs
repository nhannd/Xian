#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarReplicate", "Replicate")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuReplicate", "Replicate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReplicate")]
	[IconSet("activate", IconScheme.Colour, "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class ReplicateTool : DicomEditorTool
	{
		public ReplicateTool() : base(true) {}

		public void Replicate()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmReplicateTagsInAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				foreach (DicomEditorTag tag in this.Context.SelectedTags)
				{
					this.Context.DumpManagement.EditTag(tag.TagId, tag.Value, true);
				}
				this.Context.UpdateDisplay();
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