using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Holds parameters that control the creation of a <see cref="Workspace"/>.
    /// </summary>
    public class WorkspaceCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;

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
    }
}
