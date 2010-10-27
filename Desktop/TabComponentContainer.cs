#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Defines an extension point for views onto the <see cref="TabComponentContainer"/>.
    /// </summary>
	public sealed class TabComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Hosts multiple <see cref="IApplicationComponent"/>s in a tabbed view.
    /// </summary>
	[AssociateView(typeof(TabComponentContainerViewExtensionPoint))]
    public class TabComponentContainer : PagedComponentContainer<TabPage>
    {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TabComponentContainer()
        {
        }
    }
}
