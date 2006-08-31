using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="NavigatorComponentContainer"/>.
    /// </summary>
    public class NavigatorPage
    {

        private IApplicationComponent _component;
        private Path _path;
        private NavigatorComponentContainer.PageHost _host;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path">The path to this page in the navigation tree</param>
        /// <param name="component">The application component to be displayed by this page</param>
        public NavigatorPage(string path, IApplicationComponent component)
        {
            _path = new ClearCanvas.Desktop.Path(path, new ResourceResolver(new Assembly[] { component.GetType().Assembly }));
            _component = component;
        }

        /// <summary>
        /// Gets the path to this page.
        /// </summary>
        public Path Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Gets the component that is displayed on this page.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets the component host for this page.  For internal use only.
        /// </summary>
        public NavigatorComponentContainer.PageHost ComponentHost
        {
            get { return _host; }
            set { _host = value; }
        }

		public override string ToString()
		{
			return this.Path.ToString();
		}
    }
}
