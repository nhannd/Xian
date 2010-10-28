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
    /// Holds parameters that control the creation of a <see cref="Workspace"/>.
    /// </summary>
    public class WorkspaceCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;
        private bool _userClosable = true;  // default to true

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkspaceCreationArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="name"></param>
        public WorkspaceCreationArgs(IApplicationComponent component, string title, string name)
            :base(title, name)
        {
            _component = component;
        }

        /// <summary>
        /// Gets or sets the hosted component.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
            set { _component = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this workspace can be closed directly by the user.
        /// </summary>
        public bool UserClosable
        {
            get { return _userClosable; }
            set { _userClosable = value; }
       }
    }
}
