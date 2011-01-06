#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Holds parameters that control the creation of a <see cref="DialogBox"/>.
    /// </summary>
    public class DialogBoxCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;
        private DialogSizeHint _sizeHint;
        private Size _size = Size.Empty;
    	private bool _allowUserResize = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DialogBoxCreationArgs()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to be hosted in the dialog.</param>
        /// <param name="title">The title to assign to the dialog.</param>
        /// <param name="name">The name/identifier of the dialog.</param>
        /// <param name="sizeHint">The size hint for the dialog.</param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name, DialogSizeHint sizeHint)
            :base(title, name)
        {
            _component = component;
            _sizeHint = sizeHint;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to be hosted in the dialog.</param>
        /// <param name="title">The title to assign to the dialog.</param>
        /// <param name="name">The name/identifier of the dialog.</param>
        /// <param name="size">The size of the dialog in pixels.</param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name, Size size)
            : base(title, name)
        {
            _component = component;
            _size = size;
        }

    	/// <summary>
    	/// Constructor.
    	/// </summary>
    	/// <param name="component">The component to be hosted in the dialog.</param>
    	/// <param name="title">The title to assign to the dialog.</param>
    	/// <param name="name">The name/identifier of the dialog.</param>
    	/// <param name="allowUserResize">A value indicating whether or not the user should be allowed to resize the dialog.</param>
    	public DialogBoxCreationArgs(IApplicationComponent component, string title, string name, bool allowUserResize)
    		: this(component, title, name, DialogSizeHint.Auto)
    	{
    		_allowUserResize = allowUserResize;
    	}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to be hosted in the dialog.</param>
        /// <param name="title">The title to assign to the dialog.</param>
        /// <param name="name">The name/identifier of the dialog.</param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name)
            : base(title, name)
        {
            _component = component;
        }

        /// <summary>
        /// Gets or sets the component to host.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
            set { _component = value; }
        }

        /// <summary>
        /// Gets or sets the size hint for the dialog box.
        /// </summary>
        /// <seealso cref="Size"/>
        public DialogSizeHint SizeHint
        {
            get { return _sizeHint; }
            set { _sizeHint = value; }
        }

        /// <summary>
        /// Gets or sets an explicit size for the dialog in pixels.  If specified, this property will override <see cref="SizeHint"/>. 
        /// </summary>
        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }

		/// <summary>
		/// Gets or sets a value indicating whether or not the user should be allowed to resize the dialog.
		/// </summary>
    	public bool AllowUserResize
    	{
			get { return _allowUserResize; }
			set { _allowUserResize = value; }
    	}
    }
}
