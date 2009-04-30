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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Clipboard;
using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ButtonAction("edit", KeyImageClipboard.ToolbarSite + "/ToolbarEditKeyImageInformation", "Edit")]
	[Tooltip("edit", "TooltipEditKeyImageInformation")]
	[IconSet("edit", IconScheme.Colour, "Icons.EditKeyImageInformationToolSmall.png", "Icons.EditKeyImageInformationToolMedium.png", "Icons.EditKeyImageInformationToolLarge.png")]
	[EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	internal class EditKeyImageInformationTool : Tool<IClipboardToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public EditKeyImageInformationTool()
		{
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }	
			remove { _enabledChanged -= value; }	
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.ClipboardItemsChanged += OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged += OnSelectionChanged;
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			this.Context.ClipboardItemsChanged -= OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged -= OnSelectionChanged;

			base.Dispose(disposing);
		}

		private void OnClipboardItemsChanged(object sender, EventArgs e)
		{
			Enabled = Context.ClipboardItems.Count > 0;
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Enabled = Context.ClipboardItems.Count > 0;
		}

		public void Edit()
		{
			//TODO: can we use an override to add actions?
			KeyImageInformationEditorComponent.Launch(this.Context.DesktopWindow);
		}
	}
}
