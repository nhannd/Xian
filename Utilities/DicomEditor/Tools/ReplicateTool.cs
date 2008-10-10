#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	//TODO: create DicomEditorTool base class and factor out enabled code.

	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarReplicate", "Replicate")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuReplicate", "Replicate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipReplicate")]
	[IconSet("activate", IconScheme.Colour, "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class ReplicateTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public ReplicateTool()
        {
        }
        
        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
			this.Context.IsLocalFileChanged += new EventHandler(OnIsLocalFileChanged);
        }

        public bool Enabled
        {
            get { return _enabled; }
			protected set 
			{
				value = value & this.Context.IsLocalFile;
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        private void Replicate()
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

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            this.Enabled = !e.IsCurrentTheOnly;
		}

		private void OnIsLocalFileChanged(object sender, EventArgs e) {
			this.Enabled = base.Context.IsLocalFile;
		}  
    }
}
