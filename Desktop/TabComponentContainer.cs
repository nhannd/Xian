using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="TabComponent"/>
    /// </summary>
    public class TabComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// An application component that acts as a container for other application components.
    /// The child components are treated as "pages", where each page is a node in a tree.
    /// Only one page is displayed at a time, however, a navigation tree is provided on the side
    /// to aid the user in navigating the set of pages.
    /// </summary>
    [AssociateView(typeof(TabComponentContainerViewExtensionPoint))]
    public class TabComponentContainer : PagedComponentContainer<TabPage>
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public TabComponentContainer()
        {
        }
    }
}
