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

            public override IDesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
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
