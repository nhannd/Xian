#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="StackTabComponentContainer"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class StackTabComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// An enumeration describing the style of a <see cref="StackTabComponentContainer"/>.
	/// </summary>
    public enum StackStyle
    {
        /// <summary>
        /// Only one stack can be open at the same time.  
        /// </summary>
        /// <remarks>
		/// Each stack can be open/closed by clicking on the title bar itself, which act as a button.
		/// </remarks>
        ShowOneOnly = 0,

        /// <summary>
        /// Multiple stack can be open at the same time.  
        /// </summary>
        /// <remarks>
		/// Each stack can be open/closed by clicking on the Down/Up arrow on the title bar.
		/// </remarks>
        ShowMultiple = 1
    }

    /// <summary>
    /// The <see cref="StackTabComponentContainer"/> hosts <see cref="IApplicationComponent"/>s in a
    /// 'stacked' UI representation.
    /// </summary>
    [AssociateView(typeof(StackTabComponentContainerViewExtensionPoint))]
    public class StackTabComponentContainer : PagedComponentContainer<StackTabPage>
    {
        private readonly StackStyle _stackStyle;
		private readonly bool _openAllTabsInitially;

        /// <summary>
        /// Constructor.
        /// </summary>
		public StackTabComponentContainer(StackStyle stackStyle, bool openAllTabsInitially)
        {
            _stackStyle = stackStyle;
        	_openAllTabsInitially = openAllTabsInitially;
        }

		/// <summary>
		/// Gets the <see cref="StackStyle"/> of the container.
		/// </summary>
        public StackStyle StackStyle
        {
            get { return _stackStyle; }
        }

		/// <summary>
		/// Gets the settings for opening all tabs initially.  This is applicable to StackStyle.ShowMultiple only
		/// </summary>
		public bool OpenAllTabsInitially
		{
			get { return _openAllTabsInitially; }
		}
	}
}
