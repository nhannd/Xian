#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A host for components that are children of other components.
	/// </summary>
    public class ChildComponentHost : ApplicationComponentHost
    {
        private readonly IApplicationComponentHost _parentHost;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parentHost">The object that hosts the <paramref name="childComponent"/>'s parent component.</param>
		/// <param name="childComponent">The child application component being hosted.</param>
        public ChildComponentHost(IApplicationComponentHost parentHost, IApplicationComponent childComponent)
            : base(childComponent)
        {
            Platform.CheckForNullReference(parentHost, "parentHost");

            _parentHost = parentHost;
        }

		/// <summary>
		/// Gets the <see cref="DesktopWindow"/> that owns the parent component.
		/// </summary>
        public override DesktopWindow DesktopWindow
        {
            get { return _parentHost.DesktopWindow; }
        }

		/// <summary>
		/// Gets the title of the parent host.
		/// </summary>
        public override string Title
        {
            get { return _parentHost.Title; }
        }

    }
}
