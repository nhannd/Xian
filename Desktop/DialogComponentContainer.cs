#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
	/// Defines an extension point for views onto the <see cref="DialogComponentContainer"/>.
    /// </summary>
    public class DialogComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// A container class for hosting <see cref="IApplicationComponent"/>s in a dialog
	/// box that can be dismissed with Ok or Cancel.
	/// </summary>
    [AssociateView(typeof(DialogComponentContainerViewExtensionPoint))]
    public class DialogComponentContainer : ApplicationComponentContainer
    {
        //Todo (Jon): Can this be made private?

		/// <summary>
		/// Host object for <see cref="DialogContent"/>.
		/// </summary>
		public class DialogContentHost : ApplicationComponentHost
        {
            private DialogComponentContainer _owner;
			private DialogContent _content;

			internal DialogContentHost(
				DialogComponentContainer owner,
				DialogContent content)
                :base(content.Component)
            {
				Platform.CheckForNullReference(owner, "owner");
				Platform.CheckForNullReference(content, "content");

                _owner = owner;
				_content = content;
            }

			/// <summary>
			/// Gets the owner <see cref="DialogComponentContainer" />.
			/// </summary>
            public DialogComponentContainer Owner
            {
                get { return _owner; }
            }

            #region ApplicationComponentHost overrides

			/// <summary>
			/// Gets the associated command history object.
			/// </summary>
			/// <exception cref="NotSupportedException">The host does not support command history.</exception>
			public override CommandHistory CommandHistory
            {
                get { return _owner.Host.CommandHistory; }
            }

			/// <summary>
			/// Gets the associated desktop window.
			/// </summary>
			public override DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

			/// <summary>
			/// Gets or sets the title displayed in the user-interface.
			/// </summary>
			/// <exception cref="NotSupportedException">The host does not support titles.</exception>
			public override string Title
            {
                get { return _owner.Host.Title; }
                set { _owner.Host.Title = value; }
            }

            #endregion
        }


		private DialogContent _content;
        private DialogContentHost _contentHost;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DialogComponentContainer(DialogContent content)
		{
			_content = content;
            _contentHost = new DialogContentHost(this, _content);
		}

		/// <summary>
		/// The <see cref="DialogContent"/> to be hosted in the <see cref="DialogComponentContainer"/>.
		/// </summary>
		public DialogContent Content
		{
			get { return _content; }
		}

		/// <summary>
		/// The host object for the <see cref="DialogContent"/>.
		/// </summary>
        public DialogContentHost ContentHost
        {
            get { return _contentHost; }
        }

        #region ApplicationComponent overrides

		/// <summary>
		/// Starts this component and the <see cref="ContentHost"/>.
		/// </summary>
		///  <remarks>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Start()
        {
			base.Start();

			_contentHost.StartComponent();
        }

		/// <summary>
		/// Stops this component and the <see cref="ContentHost"/>.
		/// </summary>
		/// <remarks>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Stop()
        {
            _contentHost.StopComponent();

            base.Stop();
        }

        #endregion

        #region ApplicationComponentContainer overrides

		/// <summary>
		/// Gets an enumeration of the contained components.
		/// </summary>
		public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { return new IApplicationComponent[] { _contentHost.Component }; }
        }

		/// <summary>
		/// Gets an enumeration of the components that are currently visible.
		/// </summary>
		public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get { return this.ContainedComponents; }
        }

		/// <summary>
		/// Does nothing, since the hosted component is started by default.
		/// </summary>
		public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted component is started by default
        }

		/// <summary>
		/// Does nothing, since the hosted component is visible by default.
		/// </summary>
		public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted component is visible by default
        }

        #endregion

        #region Presentation Model

		/// <summary>
		/// Called by the view to indicate the user dismissed the dialog with "Ok"; the <see cref="ApplicationComponent.ExitCode"/>
		/// is set to <see cref="ApplicationComponentExitCode.Accepted"/>.
		/// </summary>
        public void OK()
		{
			this.ExitCode = ApplicationComponentExitCode.Accepted;
			this.Host.Exit();
		}

		/// <summary>
		/// Called by the view to indicate the user dismissed the dialog with "Cancel"; the <see cref="ApplicationComponent.ExitCode"/>
		/// is set to <see cref="ApplicationComponentExitCode.None"/>.
		/// </summary>
		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

        #endregion
    }
}
