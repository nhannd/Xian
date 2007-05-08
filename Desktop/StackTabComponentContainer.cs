using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="StackTabComponentContainer"/>
    /// </summary>
    [ExtensionPoint]
    public class StackTabComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public enum StackStyle
    {
        /// <summary>
        /// Only one stack can be open at the same time.  
        /// Each stack can be open/closed by clicking on the title bar itself, which act as a button.
        /// </summary>
        ShowOneOnly = 0,

        /// <summary>
        /// Multiple stack can be open at the same time.  
        /// Each stack can be open/closed by clicking on the Down/Up arrow on the title bar.
        /// </summary>
        ShowMultiple = 1
    }

    /// <summary>
    /// StackTabComponentContainer class
    /// </summary>
    [AssociateView(typeof(StackTabComponentContainerViewExtensionPoint))]
    public class StackTabComponentContainer : PagedComponentContainer<TabPage>
    {
        private StackStyle _stackStyle;

        /// <summary>
        /// Constructor
        /// </summary>
        public StackTabComponentContainer(StackStyle stackStyle)
        {
            _stackStyle = stackStyle;
        }

        public StackStyle StackStyle
        {
            get { return _stackStyle; }
        }
    }
}
