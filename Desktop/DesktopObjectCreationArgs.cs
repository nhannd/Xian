#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
        /// <param name="title">The title for the <see cref="DesktopObject"/>.</param>
        /// <param name="name">The name/identifier of the <see cref="DesktopObject"/>.</param>
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
