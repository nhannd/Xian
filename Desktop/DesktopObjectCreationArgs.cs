using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for passing creation parameters to desktop object factories.
    /// </summary>
    public abstract class DesktopObjectCreationArgs
    {
        private string _name;
        private string _title;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DesktopObjectCreationArgs()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        protected DesktopObjectCreationArgs(string title, string name)
        {
            _name = name;
            _title = title;
        }

        /// <summary>
        /// Gets or sets the name for the desktop object.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the title for the desktop object.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
