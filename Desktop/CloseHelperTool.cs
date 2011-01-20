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
