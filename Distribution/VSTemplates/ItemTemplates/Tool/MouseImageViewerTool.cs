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
using ClearCanvas.ImageViewer.InputManagement;

namespace $rootnamespace$
{
    // This template provides the boiler-plate code for creating a mouse image viewer tool.
    // A mouse image viewer tool is a tool that, when activated, is assigned to a specific 
	// mouse button and is given the opportunity to respond to mouse events for that button.
	// Mouse tools may also respond to mouse wheel events, if desired.

    // Declares a menu action with action ID "activate"
    // TODO: Change the action path hint to your desired menu path, or
    // remove this attribute if you do not want to create a menu item for this tool
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsMyTools/$tool$", "Select", Flags = ClickActionFlags.CheckAction)]

    // Declares a toolbar button action with action ID "activate"
    // TODO: Change the action path hint to your desired toolbar path, or
    // remove this attribute if you do not want to create a toolbar button for this tool
    [ButtonAction("activate", "global-toolbars/ToolbarMyTools/$tool$", "Select", Flags = ClickActionFlags.CheckAction)]

	/// Declares that the tooltip value is returned by the "Tooltip" property and changes
	/// to its value are communicated through the "TooltipChanged" event
	/// both of these are defined in the base MouseImageViewerTool class
    [TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]

	// Specifies which mouse button is assigned to this tool
	// TODO: Replace default mouse button assignment with your own and whether
	// the tool should be initially active (cannot be guaranteed since more than
	// one tool maybe assigned to the same mouse button)
	[MouseToolButton(XMouseButtons.Left, true)]

	// Indicates that the mouse wheel should be handled by this tool when the
	// specified keyboard modifier is pressed and the mouse wheel is activated
	// TODO: Replace the assigned modifier(s), or remove this attribute altogether
	// If you choose to remove the attribute, you can also remove the corresponding
	// mouse wheel methods in the body of the tool class
	[MouseWheelHandler(ModifierFlags.Shift)]

    // Specifies icon resources to use for the "activate" action
    // TODO: Replace the icon resource names with your desired icon resources
    [IconSet("activate", IconScheme.Colour, "Icons.$tool$Small.png", "Icons.$tool$Medium.png", "Icons.$tool$Large.png")]

    // Do not modify this attribute
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    
    // Specifies that the enablement of the "apply" action in the user-interface
    // is controlled by observing a boolean property named "Enabled", listening to
    // an event named "EnabledChanged" for changes to this property
	// both "Enabled" and "EnabledChanged" are defined in the base class ImageViewerTool
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

    // Declares this tool as an extension of the ImageViewerToolExtensionPoint extension point
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]

    public class $tool$ : MouseImageViewerTool
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
        public $tool$()
			: base("TODO: put the 'Friendly Name' that should appear in the tooltip here")
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

        /// <summary>
        /// Called by framework when the assigned mouse button is pressed in an <see cref="ITile"/>.
        /// </summary>
        /// <returns>True if the event was handled, false otherwise.</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

            // TODO: Add your handler code here

            return true;
        }

        /// <summary>
        /// Called by the framework as the mouse moves while the assigned mouse button
        /// is pressed.
        /// </summary>
        /// <returns>True if the event was handled, false otherwise.</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

            // TODO: Add your handler code here

            return true;
        }

        /// <summary>
        /// Called by the framework when the assigned mouse button is released.
        /// </summary>
        /// <returns>
		/// True should be returned if this tool needs to maintain mouse 'capture'
		/// (e.g a tool that draws a line and requires multiple mouse clicks, or calls to Start,
		/// so capture must be retained until the line has been completely drawn).
		/// If false is returned, this tool will lose capture until the appropriate
		/// mouse button is clicked again.
		/// </returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

            // TODO: Add your handler code here

            return true;
        }

		// TODO: add mouse wheel handling code to these methods, 
		// or remove them if they are not needed

		/// <summary>
		/// Called by the framework when mouse wheel activity starts.
		/// </summary>
		public override void StartWheel()
		{
			//TODO: add mouse wheel handling code here.
		}

		/// <summary>
		/// Called when the mouse wheel has moved forward.
		/// </summary>
		protected override void WheelForward()
		{
			//TODO: add mouse wheel handling code here.
		}

		/// <summary>
		/// Called when the mouse wheel has moved back.
		/// </summary>
		protected override void WheelBack()
		{
			//TODO: add mouse wheel handling code here.
		}

		/// <summary>
		/// Called by the framework to indicate that mouse wheel activity has stopped 
		/// (a short period of time has elapsed without any activity).
		/// </summary>
		public override void StopWheel()
		{
			//TODO: add mouse wheel handling code here.
		}
	}
}
