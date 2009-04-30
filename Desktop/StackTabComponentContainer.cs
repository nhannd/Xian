#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
