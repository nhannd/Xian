using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Holds parameters that control the creation of a <see cref="DialogBox"/>.
    /// </summary>
    public class DialogBoxCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DialogBoxCreationArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="name"></param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name)
            :base(title, name)
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
    }
}
