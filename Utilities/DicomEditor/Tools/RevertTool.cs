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
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarRevert", "Revert")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuRevert", "Revert")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipRevert")]
	[IconSet("activate", IconScheme.Colour, "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class RevertTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private bool _promptForAll;

        public RevertTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
            this.Context.TagEdited += new EventHandler(OnTagEditted);
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

        private void Revert()
        {
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevert, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                if (_promptForAll)
                {
                    if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevertAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                    {
                        this.Context.DumpManagement.RevertEdits(true);
                        this.Context.UpdateDisplay();
                        this.Enabled = false;
                    }
                    else
                    {
                        this.Context.DumpManagement.RevertEdits(false);
                        this.Context.UpdateDisplay();
                        this.Enabled = false;
                    }
                }
                else
                {
                    this.Context.DumpManagement.RevertEdits(false);
                    this.Context.UpdateDisplay();
                    this.Enabled = false;
                }
            }
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            _promptForAll = !e.IsCurrentTheOnly;
            this.Enabled = e.HasCurrentBeenEditted == true ? true : false;
        }

        protected void OnTagEditted(object sender, EventArgs e)
        {
            this.Enabled = true;
		}

		private void OnIsLocalFileChanged(object sender, EventArgs e) {
			this.Enabled = base.Context.IsLocalFile;
		}  
    }
}

//    [ButtonAction("activate", "dicomeditor-toolbar/Revert All")]
//    [MenuAction("activate", "dicomeditor-contextmenu/Revert All")]
//    [ClickHandler("activate", "RevertAll")]
//    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
//    [Tooltip("activate", "Revert all files")]
//    [IconSet("activate", IconScheme.Colour, "Icons.RevertSmall.png", "Icons.RevertSmall.png", "Icons.RevertSmall.png")]
//    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
//    class RevertAllTool : Tool<DicomEditorComponent.DicomEditorToolContext>
//    {
//        private bool _enabled;
//        private event EventHandler _enabledChanged;

//        public RevertAllTool()
//        {
//        }

//        public override void Initialize()
//        {
//            base.Initialize();
//            this.Enabled = true;
//            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
//        }

//        public bool Enabled
//        {
//            get { return _enabled; }
//            protected set
//            {
//                if (_enabled != value)
//                {
//                    _enabled = value;
//                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
//                }
//            }
//        }

//        /// <summary>
//        /// Notifies that the Enabled state of this tool has changed.
//        /// </summary>
//        public event EventHandler EnabledChanged
//        {
//            add { _enabledChanged += value; }
//            remove { _enabledChanged -= value; }
//        }

//        private void RevertAll()
//        {
//            if (Platform.ShowMessageBox("ALL the loaded files will be reverted to their last saved state.  All edits will be lost.  Continue?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
//            {
//                this.Context.DumpManagement.RevertAllEdits();
//                this.Context.UpdateDisplay();
//            }
//        }

//        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
//        {
//            this.Enabled = !e.IsCurrentTheOnly;
//        }
//    }
//}
