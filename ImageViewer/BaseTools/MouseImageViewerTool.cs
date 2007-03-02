using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;
using System.Drawing;
using ClearCanvas.Desktop;


namespace ClearCanvas.ImageViewer.BaseTools
{
    /// <summary>
    /// Extends the <see cref="Tool"/> class to provide functionality that is common to mouse tools.
    /// </summary>
    /// <remarks>
    /// A mouse tool is a tool that, when activated, is assigned to a specific mouse button 
    /// and is given the opportunity to respond to mouse events for that button.  Developers 
    /// implementing mouse tools should subclass this class.
    /// </remarks>
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

        private bool _active;
        private event EventHandler _activationChangedEvent;

		private bool _requiresCapture;
		private CursorToken _cursorToken;

		#endregion

		protected MouseImageViewerTool()
        {
			_requiresCapture = true;
        }

		public bool RequiresCapture
		{
			get { return _requiresCapture; }
			protected set { _requiresCapture = value; }
		}

		/// <summary>
		/// Gets the cursor associated with this mouse tool.
		/// </summary>
		public CursorToken CursorToken
		{
			get { return _cursorToken; }
			protected set { _cursorToken = value; }
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
		}

        /// <summary>
        /// Requests that this mouse tool be the active tool for the mouse button to which it
        /// is assigned.
        /// </summary>
		public void Select()
		{
			this.Active = true;
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

		public virtual void Cancel()
		{
		}

		public virtual bool SuppressContextMenu
		{
			get { return false; }
		}

		#endregion

		#region ICursorTokenProvider Members

		public virtual CursorToken GetCursorToken(Point point)
		{
			return this.CursorToken;
		}

		#endregion
	}
}
