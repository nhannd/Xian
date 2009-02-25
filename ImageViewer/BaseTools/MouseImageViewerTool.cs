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
	/// Typically, a mouse tool needs to be decorated with certain attributes in order to work properly.  For
	/// example:
	/// <example>
	/// <code>
	/// [C#]
	///  	[MenuAction("activate", "imageviewer-contextmenu/My Tool", "Select", Flags = ClickActionFlags.CheckAction)]
	///		[MenuAction("activate", "global-menus/MenuTools/MenuStandard/My Tool", "Select", Flags = ClickActionFlags.CheckAction)]
	///		[ButtonAction("activate", "global-toolbars/ToolbarStandard/My Tool", "Select", Flags = ClickActionFlags.CheckAction)]
	///		[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandard/My Tool", "Select", KeyStroke = XKeys.R)]
	///		[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	///		[IconSet("activate", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolToolMedium.png", "Icons.MyToolToolLarge.png")]
	///		[MouseToolButton(XMouseButtons.Left, true)]
	/// </code>
	/// The "Select" parameter in each of the 'Action' attributes refers to the <see cref="MouseImageViewerTool.Select"/> method
	/// which activates (and checks) the tool.  All other tools with the same <see cref="MouseButton"/> are deactivated.
	/// </example>
	/// </para>
	/// <para>
	/// When a tool does not implement the typical mouse button handling behaviour, it should <b>not</b> be 
	/// decorated with any of the above attributes as it will result in unexpected behaviour with regards to the toolbars and menus.
	/// </para>
	/// <para>
	/// A mouse tool can also have an additional mouse button shortcut specified 
	/// (see <see cref="DefaultMouseToolButtonAttribute"/>) that does not require the mouse 
	/// tool to be activated in order to use it.  The value of <see cref="DefaultMouseButtonShortcut"/> need not be 
	/// modified (e.g. Ctrl, Shift),  however if another tool's <see cref="MouseButton"/> has the same value and is active,
	/// it will supersede any <see cref="DefaultMouseButtonShortcut"/> assignments.  Therefore, <see cref="DefaultMouseButtonShortcut"/>
	/// should typically only be assigned a non-modified value when no other <see cref="MouseImageViewerTool"/> has the same
	/// <see cref="MouseButton"/> value.
	/// </para>
	/// <para>
	/// One further piece of functionality that subclasses of <see cref="MouseImageViewerTool"/> can choose to implement
	/// is handling of the mouse wheel.  When decorated with a <see cref="MouseWheelHandlerAttribute"/>, the tool's
	/// <see cref="MouseWheelShortcut"/> will be set upon construction.  The subclass must override the mouse wheel-related
	/// methods in order for the tool to have any effect (see <see cref="StartWheel"/>, <see cref="StopWheel"/>, <see cref="WheelForward"/>,
	/// <see cref="WheelBack"/>).
	/// </para>
	/// </remarks>
	/// <seealso cref="MouseToolButtonAttribute"/>
	/// <seealso cref="DefaultMouseToolButtonAttribute"/>
	public abstract class MouseImageViewerTool :
		ImageViewerTool,
		IMouseButtonHandler,
		IMouseWheelHandler,
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
		private MouseButtonHandlerBehaviour _mousebuttonBehaviour;

		private MouseButtonShortcut _defaultMouseButtonShortcut;
		private event EventHandler _defaultMouseButtonShortcutChanged;

		private MouseWheelShortcut _mouseWheelShortcut;
		private event EventHandler _mouseWheelShortcutChanged;

        private bool _active;
        private event EventHandler _activationChangedEvent;

		private CursorToken _cursorToken;
		
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tooltipPrefix">The tooltip prefix, which usually describes the tool's function.</param>
		protected MouseImageViewerTool(string tooltipPrefix)
			: base()
		{
			_tooltipPrefix = tooltipPrefix;

			_mouseButton = XMouseButtons.None;
			_defaultMouseButtonShortcut = null;
			_mouseWheelShortcut = null;
			_active = false;
		}

		/// <summary>
		/// Gets or sets the tooltip prefix, which is usually a string describing the tool's function.
		/// </summary>
		protected MouseImageViewerTool()
			: this(SR.LabelUnknown)
		{
		}

		/// <summary>
		/// Gets or sets the tooltip prefix for this <see cref="MouseImageViewerTool"/>.
		/// </summary>
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
		/// Gets the tooltip associated with this tool.
		/// </summary>
		/// <remarks>
		/// For mouse tools, this is a combination of <see cref="TooltipPrefix"/> 
		/// and <see cref="MouseButton"/> in the form "Prefix (button)".
		/// </remarks>
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
				OnActivationChanged();
			}
		}

		/// <summary>
		/// Called when the <see cref="Active"/> property changes, thereby firing the <see cref="ActivationChanged"/> event.
		/// </summary>
    	protected virtual void OnActivationChanged()
    	{
    		EventsHelper.Fire(_activationChangedEvent, this, new EventArgs());
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
		/// Overrides <see cref="ToolBase.Initialize"/>.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			MouseToolAttributeProcessor.Process(this);
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
				if (_mouseButton == value)
					return;

				_mouseButton = value;
				EventsHelper.Fire(_mouseButtonChanged, this, EventArgs.Empty);

				//the mouse button assignment affects the tooltip.
				EventsHelper.Fire(_tooltipChangedEvent, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the default mouse button shortcut assigned to this tool.
		/// </summary>
		public MouseButtonShortcut DefaultMouseButtonShortcut
    	{
			get { return _defaultMouseButtonShortcut; }
			set
			{
				if (value != null && value.Equals(_defaultMouseButtonShortcut))
						return;

				_defaultMouseButtonShortcut = value;
				EventsHelper.Fire(_defaultMouseButtonShortcutChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the modified mouse button shortcut assigned to this tool.
		/// </summary>
		/// <seealso cref="DefaultMouseButtonShortcut"/>
		[Obsolete("Now just gets/sets DefaultMouseButtonShortcut.")]
		public MouseButtonShortcut ModifiedMouseButtonShortcut
		{
			get { return DefaultMouseButtonShortcut; }
			set { DefaultMouseButtonShortcut = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="MouseWheelShortcut"/>.
		/// </summary>
		public MouseWheelShortcut MouseWheelShortcut
		{
			get { return _mouseWheelShortcut; }
			set
			{
				if (_mouseWheelShortcut != null && _mouseWheelShortcut.Equals(value))
					return;

				_mouseWheelShortcut = value;
				EventsHelper.Fire(_mouseWheelShortcutChanged, this, EventArgs.Empty);
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
		/// <seealso cref="DefaultMouseButtonShortcutChanged"/>
		[Obsolete("Now just observes DefaultMouseButtonShortcutChanged.")]
		public event EventHandler ModifiedMouseButtonShortcutChanged
		{
			add { DefaultMouseButtonShortcutChanged += value; }
			remove { DefaultMouseButtonShortcutChanged -= value; }
		}
		
		/// <summary>
		/// Fired when the <see cref="DefaultMouseButtonShortcut"/> property has changed.
		/// </summary>
		public event EventHandler DefaultMouseButtonShortcutChanged
		{
			add { _defaultMouseButtonShortcutChanged += value; }
			remove { _defaultMouseButtonShortcutChanged -= value; }
		}

    	/// <summary>
    	/// Fired when the <see cref="MouseWheelShortcut"/> property has changed.
    	/// </summary>
    	public event EventHandler MouseWheelShortcutChanged
    	{
    		add { _mouseWheelShortcutChanged += value; }
    		remove { _mouseWheelShortcutChanged -= value; }
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
    	/// Allows the <see cref="IMouseButtonHandler"/> to override certain default framework behaviour.
    	/// </summary>
    	public MouseButtonHandlerBehaviour Behaviour
		{
			get { return _mousebuttonBehaviour; }
			protected set { _mousebuttonBehaviour = value; }
		}

		#endregion

		#region Mouse Wheel
		#region IMouseWheelHandler Members

		/// <summary>
		/// Called by the framework when mouse wheel activity starts.
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		public virtual void StartWheel()
		{
		}

		/// <summary>
		/// Called by the framework each time the mouse wheel is moved.
		/// </summary>
		/// <remarks>
		/// Unless overridden, this method simply calls <see cref="WheelForward"/> and <see cref="WheelBack"/>.
		/// </remarks>
		public virtual void Wheel(int wheelDelta)
		{
			if (wheelDelta > 0)
				WheelForward();
			else if (wheelDelta < 0)
				WheelBack();
		}

		/// <summary>
		/// Called by the framework to indicate that mouse wheel activity has stopped 
		/// (a short period of time has elapsed without any activity).
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		public virtual void StopWheel()
		{
		}

		#endregion

		/// <summary>
		/// Called when the mouse wheel has moved forward.
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		protected virtual void WheelForward()
		{
		}

		/// <summary>
		/// Called when the mouse wheel has moved back.
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		protected virtual void WheelBack()
		{
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
