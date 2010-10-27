#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Holds parameters that control the creation of a <see cref="DesktopWindow"/>.
    /// </summary>
    public class DesktopWindowCreationArgs : DesktopObjectCreationArgs
    {
        private string _toolbarSite;
        private string _menuSite;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DesktopWindowCreationArgs()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DesktopWindowCreationArgs(string title, string name)
            :base(title, name)
        {
        }

        /// <summary>
        /// Gets or sets the toolbar site that this window will use.
        /// </summary>
        public string ToolbarSite
        {
            get { return _toolbarSite; }
            set { _toolbarSite = value; }
        }

        /// <summary>
        /// Gets or sets the menu site that this window will use.
        /// </summary>
        public string MenuSite
        {
            get { return _menuSite; }
            set { _menuSite = value; }
        }
    }
}
