using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="NavigatorComponentContainer"/>.
    /// </summary>
    public class NavigatorPage : ContainerPage
    {

        private Path _path;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path">The path to this page in the navigation tree</param>
        /// <param name="component">The application component to be displayed by this page</param>
        public NavigatorPage(string path, IApplicationComponent component)
            :base(component)
        {
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForNullReference(component, "component");

            _path = new ClearCanvas.Desktop.Path(path, new ResourceResolver(new Assembly[] { component.GetType().Assembly }));
        }

        /// <summary>
        /// Gets the path to this page.
        /// </summary>
        public Path Path
        {
            get { return _path; }
        }

		public override string ToString()
		{
			return this.Path.ToString();
		}
    }
}
