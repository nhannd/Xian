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
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="TabComponent"/>
    /// </summary>
    public class DialogComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(DialogComponentContainerViewExtensionPoint))]
    public class DialogComponentContainer : ApplicationComponentContainer
    {
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

            public DialogComponentContainer Owner
            {
                get { return _owner; }
            }

            #region ApplicationComponentHost overrides

            public override CommandHistory CommandHistory
            {
                get { return _owner.Host.CommandHistory; }
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

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
        /// Default constructor
        /// </summary>
        public DialogComponentContainer(
			DialogContent content)
		{
			_content = content;
            _contentHost = new DialogContentHost(this, _content);
		}

		public DialogContent Content
		{
			get { return _content; }
		}

        public DialogContentHost ContentHost
        {
            get { return _contentHost; }
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
			base.Start();

			_contentHost.StartComponent();
        }

        public override void Stop()
        {
            _contentHost.StopComponent();

            base.Stop();
        }

        #endregion

        #region ApplicationComponentContainer overrides

        public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { return new IApplicationComponent[] { _contentHost.Component }; }
        }

        public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get { return this.ContainedComponents; }
        }

        public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted component is started by default
        }

        public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted component is visible by default
        }

        #endregion

        #region Presentation Model

        public void OK()
		{
			this.ExitCode = ApplicationComponentExitCode.Normal;
			this.Host.Exit();
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion
    }
}
