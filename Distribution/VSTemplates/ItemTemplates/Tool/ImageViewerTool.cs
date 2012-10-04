#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace $rootnamespace$
{
    // This template provides the boiler-plate code for creating a basic image viewer tool
    // that performs a single action when its menu item or toolbar button is clicked.

    // Declares a menu action with action ID "apply"
    // TODO: Change the action path hint to your desired menu path, or
    // remove this attribute if you do not want to create a menu item for this tool
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/$tool$", "Apply")]

    // Declares a toolbar button action with action ID "apply"
    // TODO: Change the action path hint to your desired toolbar path, or
    // remove this attribute if you do not want to create a toolbar button for this tool
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/$tool$", "Apply")]

	/// Declares a keyboard action with action ID "apply"
	/// TODO: change the KeyStroke to the desired value, or
	/// remove this attribute if you do not want to create a keyboard shortcut for this tool
	[KeyboardAction("apply", "imageviewer-keyboard/$tool$", "Apply", KeyStroke = XKeys.A)]

    // Specifies tooltip text for the "apply" action
    // TODO: Replace tooltip text
    [Tooltip("apply", "Place tooltip text here")]

    // Specifies icon resources to use for the "apply" action
    // TODO: Replace the icon resource names with your desired icon resources
    [IconSet("apply", IconScheme.Colour, "Icons.$tool$Small.png", "Icons.$tool$Medium.png", "Icons.$tool$Large.png")]
    
    // Specifies that the enablement of the "apply" action in the user-interface
    // is controlled by observing a boolean property named "Enabled", listening to
    // an event named "EnabledChanged" for changes to this property
	// both "Enabled" and "EnabledChanged" are defined in the base class ImageViewerTool
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    // Declares this tool as an extension of the ImageViewerToolExtensionPoint extension point
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]

    public class $tool$: ImageViewerTool
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
        public $tool$()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
            
            // subscribe to any relevant events
            // TODO: you may wish to add other event subscriptions here and/or delete this if it is not needed
			this.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
        }

		/// <summary>
		/// Called when the tool is disposed.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
            // TODO: unsubscribe from all events that have previously been subscribed to or remove if not needed
            this.ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

			base.Dispose(disposing);
		}


		/// Note: you may change the name of the 'Apply' method as desired, but be sure to change the
        /// corresponding parameter in the MenuAction, ButtonAction and KeyboardAction attributes

		/// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            // TODO: add code here to implement the functionality of the tool
        }

		/// <summary>
		/// Event Handler for <see cref="EventBroker.TileSelected"/>.
		/// </summary>
		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
            base.OnTileSelected(sender, e);
			
			// TODO: add code to handle this event if necessary,
            // or optionally delete this handler if not needed
		}
		
		/// <summary>
		/// Event Handler for <see cref="EventBroker.PresentationImageSelected"/>.
		/// </summary>
        protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
        {
            base.OnPresentationImageSelected(sender, e);

			// TODO: add code to handle this event if necessary,
            // or optionally delete this handler if not needed
        }

        /// <summary>
        /// Event handler called when an image is about to be drawn.
        /// </summary>
        private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
        {
            // TODO: add code to handle this event if necessary,
            // or optionally delete this handler if not needed
        }
    }
}
