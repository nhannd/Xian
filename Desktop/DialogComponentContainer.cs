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
        public class DialogContentHost : IApplicationComponentHost
        {
            private DialogComponentContainer _owner;
			private DialogContent _content;
            private IApplicationComponentView _view;
            private bool _started;

			internal DialogContentHost(
				DialogComponentContainer owner,
				DialogContent content)
            {
				Platform.CheckForNullReference(owner, "owner");
				Platform.CheckForNullReference(content, "content");

                _owner = owner;
				_content = content;
                _started = false;
            }

            public DialogComponentContainer Owner
            {
                get { return _owner; }
            }

            public bool Started
            {
                get { return _started; }
            }

            public void Start()
            {
                if (!_started)
                {
                    _content.Component.SetHost(this);
                    _content.Component.Start();
					_started = true;
                }
            }

            public void Stop()
            {
                if (_started)
                    _content.Component.Stop();
            }

            public IApplicationComponentView ComponentView
            {
                get
                {
                    if (_view == null)
                    {
                        _view = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_content.Component.GetType());
                        _view.SetComponent(_content.Component);
                    }
                    return _view;
                }
            }

            #region IApplicationComponentHost Members

            public void Exit()
            {
                throw new NotSupportedException();
            }

            public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
            {
                return Platform.ShowMessageBox(message, buttons);
            }

            public CommandHistory CommandHistory
            {
                get { return _owner.Host.CommandHistory; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

            #endregion
        }


		private DialogContent _content;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogComponentContainer(
			DialogContent content)
		{
			_content = content;
			_content.ComponentHost = new DialogContentHost(this, _content);
		}

		public DialogContent Content
		{
			get { return _content; }
		}

		public override void Start()
        {
			base.Start();

			_content.ComponentHost.Start();
        }

        public override void Stop()
        {
            _content.Component.Stop();

            base.Stop();
        }

		public void OK()
		{
			this.ExitCode = ApplicationComponentExitCode.Normal;
			Host.Exit();
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.Cancelled;
			Host.Exit();
		}
    }
}
