using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.ImageViewer
{
    /// <summary>
    /// Extends the <see cref="Tool"/> class to provide functionality that is common to mouse tools
    /// </summary>
    /// <remarks>
    /// A mouse tool is a tool that, when activated, is assigned to a specific mouse button 
    /// and is given the opportunity to respond to mouse events for that button.  Developers 
    /// implementing mouse tools should subclass this class rather than <see cref="Tool"/>.
    /// </remarks>
    public abstract class MouseTool : Tool<IImageViewerToolContext>, IUIEventHandler
	{
		// Protected attributes
        private XMouseButtons _mouseButton;

		private int _lastX;
		private int _lastY;
		private int _deltaX;
		private int _deltaY;

        private bool _active;
        private event EventHandler _activationChangedEvent;

        private bool _initiallyActive;
		private bool _requiresCapture;

        /// <summary>
        /// Constructs a mouse tool.
        /// Subclasses must initialize this constructor with the preferred mouse button
        /// and a preference for being the initially active tool.  There is no guarantee
        /// that this tool will actuall be initially active.
        /// </summary>
        /// <param name="mouseButton">The button to which this tool is assigned.</param>
        /// <param name="initiallyActive">A preference for being the initially active tool.</param>
        public MouseTool(
			XMouseButtons mouseButton, 
			bool initiallyActive)
        {
            _mouseButton = mouseButton;
            _initiallyActive = initiallyActive;
			_requiresCapture = false;
        }

        /// <summary>
        /// Constructs a mouse tool.
        /// Subclasses must initialize this constructor with the preferred mouse button.
        /// </summary>
        /// <param name="mouseButton">The button to which this tool is assigned.</param>
        public MouseTool(XMouseButtons mouseButton)
            : this(mouseButton, false)
        {
        }

        /// <summary>
        /// Initializes the mouse tool.  If this method is overridden by the subclass,
        /// the override must be sure to call the base class method, and should do so
        /// prior to doing its own initialization.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

			ImageViewerComponent viewer = this.Context.Viewer as ImageViewerComponent;
            viewer.MouseButtonToolMap.MouseToolMapped += OnMouseButtonToolMapped;

            // attempt to honour the initiallyActive request
            // there is no guarantee the request won't be superceded
            // by a request from another tool
            if (_initiallyActive)
                viewer.MouseButtonToolMap[_mouseButton] = this;
        }

        /// <summary>
        /// Reports whether this tool is currently active or not.  Any number of mouse tools
        /// may be assigned to a given mouse button, but only one such tool can be active
        /// at any given time.
        /// </summary>
        public bool Active
        {
            get { return _active; }
        }

		public bool RequiresCapture
		{
			get { return _requiresCapture; }
			protected set { _requiresCapture = value; }
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
        /// The mouse button to which this tool is assigned.
        /// </summary>
        protected XMouseButtons MouseButton
        {
            get { return _mouseButton; }
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
        /// Requests that this mouse tool to be the active tool for the mouse button to which it
        /// is assigned.
        /// </summary>
		public void Select()
		{
			ImageViewerComponent viewer = this.Context.Viewer as ImageViewerComponent;
			viewer.MouseButtonToolMap[_mouseButton] = this;
        }

        private void OnMouseButtonToolMapped(object sender, MouseButtonToolMappedEventArgs e)
        {
            if (e.MouseButton == _mouseButton)
            {
                _active = (e.NewTool == this);
                EventsHelper.Fire(_activationChangedEvent, this, new EventArgs());
            }
        }

		#region IUIEventHandler Members

		public virtual bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

            if (e.Button != this.MouseButton)
				return false;

			_deltaX = e.X - _lastX;
			_deltaY = e.Y - _lastY;

			_lastX = e.X;
			_lastY = e.Y;

			return true;
		}

		public virtual bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

            if (e.Button != this.MouseButton)
				return false;

			if (_requiresCapture && e.MouseCapture.GetCapture() != this)
				e.MouseCapture.SetCapture(this, e);

			_lastX = e.X;
			_lastY = e.Y;

			return true;
		}

        public virtual bool OnMouseUp(XMouseEventArgs e)
        {
			if (e.MouseCapture.GetCapture() == this)
				e.MouseCapture.ReleaseCapture();

			return false;
        }

        public virtual bool OnMouseWheel(XMouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnKeyDown(XKeyEventArgs e)
        {
            return false;
        }

        public virtual bool OnKeyUp(XKeyEventArgs e)
        {
            return false;
        }

		#endregion
    }
}
