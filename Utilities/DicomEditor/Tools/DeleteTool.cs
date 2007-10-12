#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarDelete", "Delete")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuDelete", "Delete")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipDelete")]
	[IconSet("activate", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class DeleteTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private bool _promptForAll;

        public DeleteTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
        }

        public bool Enabled
        {
            get { return _enabled; }
            protected set
            {
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

        private void Delete()
        {
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteSelectedTags, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                if (_promptForAll)
                {
                    if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteSelectedTagsFromAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                    {
                        foreach (DicomEditorTag tag in this.Context.SelectedTags)
                        {
                            this.Context.DumpManagement.DeleteTag(tag.TagId, true);
                        }
                        this.Context.UpdateDisplay();
                    }
                    else
                    {
                        foreach (DicomEditorTag tag in this.Context.SelectedTags)
                        {
                            this.Context.DumpManagement.DeleteTag(tag.TagId, false);
                        }
                        this.Context.UpdateDisplay();
                    }
                }
                else
                {
                    foreach (DicomEditorTag tag in this.Context.SelectedTags)
                    {
                        this.Context.DumpManagement.DeleteTag(tag.TagId, false);
                    }
                    this.Context.UpdateDisplay();
                }
            }
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            _promptForAll = !e.IsCurrentTheOnly;
        }
    }
}
