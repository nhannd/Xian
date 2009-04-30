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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides "Close Assistant" services, which inform the user of workspaces that require attention prior
    /// to a desktop window close or application quit.
    /// </summary>
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class CloseHelperTool : Tool<IDesktopToolContext>
    {
        private Shelf _closeHelperShelf;

		/// <summary>
		/// Constructor.
		/// </summary>
        public CloseHelperTool()
        {

        }

    	///<summary>
    	/// Called by the framework to allow the tool to initialize itself.
    	///</summary>
    	/// <remarks>
		/// This method will be called after <see cref="ITool.SetContext" /> has been called, which guarantees that 
		/// the tool will have access to its context when this method is called.
		/// </remarks>
    	public override void Initialize()
        {
            this.Context.DesktopWindow.Closing += new EventHandler<ClosingEventArgs>(WindowClosingEventHandler);

            base.Initialize();
        }

        private void WindowClosingEventHandler(object sender, ClosingEventArgs e)
        {
            // if interaction not allowed, or already cancelled, don't do anything here
            if (e.Interaction != UserInteraction.Allowed || e.Cancel)
                return;

            // find all the workspaces that can't be closed
            DesktopWindow window = (DesktopWindow)sender;
            bool showHelper = CollectionUtils.Contains<Workspace>(window.Workspaces,
                delegate(Workspace w) { return !w.QueryCloseReady(); });

            if (showHelper)
            {
                e.Cancel = true;
                ShowShelf(window);
            }
        }

        private void ShowShelf(DesktopWindow window)
        {
            // the shelf is not currently open
            if (_closeHelperShelf == null)
            {
                // launch it
                CloseHelperComponent component = new CloseHelperComponent();
                _closeHelperShelf = ApplicationComponent.LaunchAsShelf(window, component,
					SR.TitleCloseAssistant,
					"Close Assistant",
                    ShelfDisplayHint.DockLeft);
                _closeHelperShelf.Closed += delegate { _closeHelperShelf = null; };
            }
            else
            {
                _closeHelperShelf.Activate();
            }

            CloseHelperComponent helper = (CloseHelperComponent)_closeHelperShelf.Component;
            helper.Refresh();
        }
    }
}
