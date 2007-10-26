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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
    /// <summary>
	/// Extends the <see cref="ImageViewerTool"/> class to provide functionality that is common to mouse tools.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A mouse tool is a tool that, when activated, is assigned to a specific mouse button 
    /// (see <see cref="MouseToolButtonAttribute"/>) and is given the opportunity to respond to 
    /// mouse events for that button.  Developers implementing mouse tools should subclass this class.
	/// </para>
	/// <para>
	/// A mouse tool can also have an additional modified mouse button shortcut specified 
	/// (see <see cref="ModifiedMouseToolButtonAttribute"/>) that does not require the mouse 
	/// tool to be activated in order to use it.
	/// </para>
	/// </remarks>
	/// <seealso cref="MouseToolButtonAttribute"/>
	/// <seealso cref="ModifiedMouseToolButtonAttribute"/>
	public abstract class MouseImageViewerTool :
		ImageViewerTool,
		IMouseButtonHandler,
		ICursorTokenProvider
	{
		#region Private fields

		private int _lastX;
		private int _lastY;
		private int _deltaX;
		private int _deltaY;

		private string _tooltipPrefix;
		private event EventHandler _tooltipChangedEvent;

		private XMouseButtons _mouseButton;
		private event EventHandler _mouseButtonChanged;

		private MouseButtonShortcut _modifiedMouseButtonShortcut;
		private event EventHandler _modifiedMouseButtonShortcutChanged;

        private bool _active;
        private event EventHandler _activationChangedEvent;

		private CursorToken _cursorToken;
		
		#endregion

		protected MouseImageViewerTool(string tooltipPrefix)
			: base()
		{
			_tooltipPrefix = tooltipPrefix;

			_mouseButton = XMouseButtons.None;
			_modifiedMouseButtonShortcut = null;
			_active = false;
		}

		protected MouseImageViewerTool()
			: this(SR.LabelUnknown)
		{
		}

		protected virtual string TooltipPrefix
		{
			get { return _tooltipPrefix; }
			set 
			{
				if (_tooltipPrefix == value)
					return;

				_tooltipPrefix = value;
				EventsHelper.Fire(_tooltipChangedEvent, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the cursor token associated with this mouse tool.
		/// </summary>
		protected CursorToken CursorToken
		{
			get { return _cursorToken; }
			set { _cursorToken = value; }
		}

		/// <summary>
		/// Gets the previous x coordinate of the mouse pointer.
		/// </summary>
		protected int LastX
		{
			get { return _lastX; }
		}

		/// <summary>
		/// Gets the previous y coordinate of the mouse pointer.
		/// </summary>
		protected int LastY
		{
			get { return _lastY; }
		}

		/// <summary>
		/// Gets the change in the x position of the mouse pointer since the previous 
		/// call to <see cref="Track"/>.
		/// </summary>
		protected int DeltaX
		{
			get { return _deltaX; }
		}

		/// <summary>
		/// Gets the change in the y position of the mouse pointer since the previous 
		/// call to <see cref="Track"/>.
		/// </summary>
		protected int DeltaY
		{
			get { return _deltaY; }
		}

		/// <summary>
		/// Gets the tooltip associated with this tool.  For mouse tools, this is a combination of 
		/// <see cref="TooltipPrefix"/> and <see cref="MouseButton"/> in the form "Prefix (button)".
		/// </summary>
		public virtual string Tooltip
		{
			get
			{
				return String.Format("{0} ({1})", this.TooltipPrefix, this.MouseButton.ToString());
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this tool is currently active or not.  
		/// </summary>
		/// <remarks>
		/// Any number of mouse tools may be assigned to a given mouse button, but 
		/// only one such tool can be active at any given time.
		/// </remarks>
		public bool Active
		{
			get { return _active; }
			set
			{
				if (value == _active)
					return;

				_active = value;
				EventsHelper.Fire(_activationChangedEvent, this, new EventArgs());
			}
		}

		/// <summary>
		/// Notifies observer(s) that the tooltip has changed.
		/// </summary>
		public virtual event EventHandler TooltipChanged
		{
			add { _tooltipChangedEvent += value; }
			remove { _tooltipChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Active"/> property has changed.
		/// </summary>
		public event EventHandler ActivationChanged
		{
			add { _activationChangedEvent += value; }
			remove { _activationChangedEvent -= value; }
		}

		/// <summary>
		/// Overrides <see cref="ToolBase.Initialize"/>.  Initialization of the <see cref="MouseButton"/> member
		/// is done here using the <see cref="MouseImageViewerToolInitializer.Initialize"/> method.
		/// </summary>
		/// <remarks>
		/// Initializing the tool this way (via <see cref="MouseImageViewerToolInitializer.Initialize"/>)allows 
		/// us to defer runtime value checking, of the <see cref="MouseButton"/> attribute in particular, 
		/// to tool initialization time rather than at construction time.  This way, we can explicitly disallow
		/// assignments like 'None' and the tool will never appear in the toolbar.
		/// </remarks>
		public override void Initialize()
		{
			base.Initialize();
			MouseImageViewerToolAttributeProcessor.Process(this);
		}

        /// <summary>
        /// Requests that this mouse tool be the active tool for the mouse button to which it
        /// is assigned.
        /// </summary>
		public void Select()
		{
			this.Active = true;
		}

		/// <summary>
		/// Gets or sets the mouse button assigned to this tool.
		/// </summary>
		/// <remarks>
		/// It is expected that on creation of this tool, this property will be set to
		/// something other than 'None'.  Currently this is done in the overridden <see cref="Initialize" /> method.
		/// </remarks>
		public XMouseButtons MouseButton
		{
			get
			{
				return _mouseButton;
			}
			set
			{
				if (value == XMouseButtons.None)
					throw new ArgumentException(SR.ExceptionMouseToolMustHaveValidAssignment);

				if (_mouseButton == value)
					return;

				_mouseButton = value;
				EventsHelper.Fire(_mouseButtonChanged, this, EventArgs.Empty);

				//the mouse button assignment affects the tooltip.
				EventsHelper.Fire(_tooltipChangedEvent, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the modified mouse button shortcut assigned to this tool.
		/// </summary>
		public MouseButtonShortcut ModifiedMouseButtonShortcut
		{
			get { return _modifiedMouseButtonShortcut; }
			set
			{
				if (value != null)
				{
					if (value.Equals(_modifiedMouseButtonShortcut))
					return;

					if (!value.IsModified)
						throw new ArgumentException(String.Format(SR.ExceptionAdditionalMouseToolAssignmentsMustBeModified, this.GetType().FullName));
				}

				_modifiedMouseButtonShortcut = value;
				EventsHelper.Fire(_modifiedMouseButtonShortcutChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Fired when the <see cref="MouseButton"/> property has changed.
		/// </summary>
		public event EventHandler MouseButtonChanged
		{
			add { _mouseButtonChanged += value; }
			remove { _mouseButtonChanged -= value; }
		}

		/// <summary>
		/// Fired when the <see cref="ModifiedMouseButtonShortcut"/> property has changed.
		/// </summary>
		public event EventHandler ModifiedMouseButtonShortcutChanged
		{
			add { _modifiedMouseButtonShortcutChanged += value; }
			remove { _modifiedMouseButtonShortcutChanged -= value; }
		}

		#region IMouseButtonHandler

		/// <summary>
		/// Handles a "start mouse" message from the Framework.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>A value indicating whether the start message was handled.</returns>
		/// <remarks>
		/// <para>
		/// In most cases, <see cref="Start"/> corresponds to "mouse down".
		/// </para>
		/// <para>
		/// As a developer, you need to override this method in your 
		/// <see cref="MouseImageViewerTool"/> subclass to add your custom functionality, 
		/// but you should never have to call it; it should only ever have to be 
		/// called by the Framework.
		/// </para>
		/// </remarks>
		public virtual bool Start(IMouseInformation mouseInformation)
		{
			_lastX = mouseInformation.Location.X;
			_lastY = mouseInformation.Location.Y;

			return false;
		}

		/// <summary>
		/// Handles a "track mouse" message from the Framework.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>A value indicating whether the track message was handled.</returns>
		/// <remarks>
		/// <para>
		/// In most cases, <see cref="Track"/> corresponds to "mouse move".
		/// </para>
		/// <para>
		/// As a developer, you need to override this method in your 
		/// <see cref="MouseImageViewerTool"/> subclass to add your custom functionality, 
		/// but you should never have to call it; it should only ever have to be 
		/// called by the Framework.
		/// </para>
		/// </remarks>
		public virtual bool Track(IMouseInformation mouseInformation)
		{
			_deltaX = mouseInformation.Location.X - _lastX;
			_deltaY = mouseInformation.Location.Y - _lastY;

			_lastX = mouseInformation.Location.X;
			_lastY = mouseInformation.Location.Y;

			return false;
		}

		/// <summary>
		/// Handles a "stop mouse" message from the Framework.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>A value indicating whether the stop message was handled.</returns>
		/// <remarks>
		/// <para>
		/// In most cases, <see cref="Stop"/> corresponds to "mouse up".
		/// </para>
		/// <para>
		/// As a developer, you need to override this method in your 
		/// <see cref="MouseImageViewerTool"/> subclass to add your custom functionality, 
		/// but you should never have to call it; it should only ever have to be 
		/// called by the Framework.
		/// </para>
		/// </remarks>
		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			_lastX = 0;
			_lastY = 0;

			return false;
		}

		/// <summary>
		/// Called by the framework when it needs to unexpectedly release capture on the tool, allowing it to do 
		/// any necessary cleanup.  This method should be overridden by any derived classes that need to do cleanup.
		/// </summary>
		public virtual void Cancel()
		{
		}

		/// <summary>
		/// Gets whether or not this tool suppresses the context menu entirely while it is active/has capture.
		/// </summary>
		public virtual bool SuppressContextMenu
		{
			get { return false; }
		}

		/// <summary>
		/// Gets whether or not the <see cref="Track"/> method should be processed for mouse 
		/// coordinates outside the view rectange.
		/// </summary>
		public virtual bool ConstrainToTile
		{
			get { return false; }
		}

		#endregion

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token associated with the tool.
		/// </summary>
		/// <param name="point">The point in destination (view) coordinates.</param>
		/// <returns>a <see cref="CursorToken"/> object that is used to construct the cursor in the view.</returns>
		public virtual CursorToken GetCursorToken(Point point)
		{
			return this.CursorToken;
		}

		#endregion
	}
}
