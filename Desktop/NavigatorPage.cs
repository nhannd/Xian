#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="NavigatorComponentContainer"/>.
    /// </summary>
    public class NavigatorPage : ContainerPage
    {

        private readonly Path _path;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to this page in the navigation tree.</param>
        /// <param name="component">The application component to be displayed by this page</param>
        public NavigatorPage(string path, IApplicationComponent component)
            :this(new Path(path, new ResourceResolver(new [] { component.GetType().Assembly })), component)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to this page in the navigation tree.</param>
		/// <param name="component">The application component to be displayed by this page</param>
		public NavigatorPage(Path path, IApplicationComponent component)
			:base(component)
    	{
			Platform.CheckForNullReference(path, "path");

			_path = path;
    	}

        /// <summary>
        /// Gets the path to this page.
        /// </summary>
        public Path Path
        {
            get { return _path; }
        }

		/// <summary>
		/// Returns <see cref="ClearCanvas.Desktop.Path.ToString"/>.
		/// </summary>
		public override string ToString()
		{
			return this.Path.ToString();
		}
    }
}
