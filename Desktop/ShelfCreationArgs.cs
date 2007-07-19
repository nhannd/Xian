using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Holds parameters that control the creation of a <see cref="Shelf"/>.
    /// </summary>
    public class ShelfCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;
        private ShelfDisplayHint _displayHint;

        /// <summary>
        /// Constructor
        /// </summary>
        public ShelfCreationArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <param name="displayHint"></param>
        public ShelfCreationArgs(IApplicationComponent component, string title, string name, ShelfDisplayHint displayHint)
            : base(title, name)
        {
            _component = component;
            _displayHint = displayHint;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="name"></param>
        public ShelfCreationArgs(IApplicationComponent component, string title, string name)
            : this(component, name, title, ShelfDisplayHint.None)
        {
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
        /// Gets or sets the display hint that affects the initial positioning of the shelf.
        /// </summary>
        public ShelfDisplayHint DisplayHint
        {
            get { return _displayHint; }
            set { _displayHint = value; }
        }
    }
}
