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
    /// Extends the <see cref="Tool"/> class to provide functionality that is common to mouse tools
    /// </summary>
    /// <remarks>
    /// A mouse tool is a tool that, when activated, is assigned to a specific mouse button 
    /// and is given the opportunity to respond to mouse events for that button.  Developers 
    /// implementing mouse tools should subclass this class rather than <see cref="Tool"/>.
    /// </remarks>

	public abstract class MouseImageViewerTool :
		ImageViewerTool, 
		IMouseButtonHandler, 
		ICursorTokenProvider
	{
		private int _lastX;
		private int _lastY;
		private int _deltaX;
		private int _deltaY;

        private bool _active;
        private event EventHandler _activationChangedEvent;

		private bool _requiresCapture;
		private CursorToken _cursorToken;
		
        /// <summary>
        /// Constructs a mouse tool.
        /// Subclasses must initialize this constructor with the preferred mouse button
        /// and a preference for being the initially active tool.  There is no guarantee
        /// that this tool will actuall be initially active.
        /// </summary>
        public MouseImageViewerTool()
        {
			_requiresCapture = true;
        }

		public bool RequiresCapture
		{
			get { return _requiresCapture; }
			protected set { _requiresCapture = value; }
		}

		public CursorToken CursorToken
		{
			get { return _cursorToken; }
			protected set { _cursorToken = value; }
		}

		/// <summary>
        /// Reports whether this tool is currently active or not.  Any number of mouse tools
        /// may be assigned to a given mouse button, but only one such tool can be active
        /// at any given time.
        /// </summary>
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
		/// The previous x coordinate of the mouse pointer.
		/// </summary>
		protected int LastX
		{
			get { return _lastX; }
		}

		/// <summary>
		/// The previous y coordinate of the mouse pointer.
		/// </summary>
		protected int LastY
		{
			get { return _lastY; }
		}

		/// <summary>
		/// The change in the x position of the mouse pointer since the previous call to <see cref="OnMouseMove"/>.
		/// </summary>
		protected int DeltaX
		{
			get { return _deltaX; }
		}

		/// <summary>
		/// The change in the y position of the mouse pointer since the previous call to <see cref="OnMouseMove"/>.
		/// </summary>
		protected int DeltaY
		{
			get { return _deltaY; }
		}

		/// <summary>
		/// Notifies that the value of the <see cref="Active"/> property has changed.
		/// </summary>
		public event EventHandler ActivationChanged
		{
			add { _activationChangedEvent += value; }
			remove { _activationChangedEvent -= value; }
		}

		/// <summary>
		/// Initializes the mouse tool.  If this method is overridden by the subclass,
		/// the override must be sure to call the base class method, and should do so
		/// prior to doing its own initialization.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

        /// <summary>
        /// Requests that this mouse tool to be the active tool for the mouse button to which it
        /// is assigned.
        /// </summary>
		public void Select()
		{
			this.Active = true;
		}

		#region IMouseButtonHandler

		public virtual bool Start(IMouseInformation mouseInformation)
		{
			_lastX = mouseInformation.Location.X;
			_lastY = mouseInformation.Location.Y;

			return false;
		}

		public virtual bool Track(IMouseInformation mouseInformation)
		{
			_deltaX = mouseInformation.Location.X - _lastX;
			_deltaY = mouseInformation.Location.Y - _lastY;

			_lastX = mouseInformation.Location.X;
			_lastY = mouseInformation.Location.Y;

			return false;
		}

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

		protected virtual bool IsImageValid(IPresentationImage image)
		{
			if (image == null)
				return false;
			else
				return true;
		}

		#region ICursorTokenProvider Members

		public virtual CursorToken GetCursorToken(Point point)
		{
			return this.CursorToken;
		}

		#endregion
	}
}
