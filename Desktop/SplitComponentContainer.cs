using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="TabComponent"/>
    /// </summary>
    public class SplitComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(SplitComponentContainerViewExtensionPoint))]
    public class SplitComponentContainer : ApplicationComponentContainer
    {
        public class SplitPaneHost : IApplicationComponentHost
        {
            private SplitComponentContainer _owner;
			private SplitPane _pane;
            private IApplicationComponentView _view;
            private bool _started;

            internal SplitPaneHost(
				SplitComponentContainer owner,
				SplitPane pane)
            {
				Platform.CheckForNullReference(owner, "owner");
				Platform.CheckForNullReference(pane, "pane");

                _owner = owner;
				_pane = pane;
                _started = false;
            }

            public SplitComponentContainer Owner
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
                    _pane.Component.SetHost(this);
                    _pane.Component.Start();
					_started = true;
                }
            }

            public void Stop()
            {
                if (_started)
                    _pane.Component.Stop();
            }

            public IApplicationComponentView ComponentView
            {
                get
                {
                    if (_view == null)
                    {
                        _view = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_pane.Component.GetType());
                        _view.SetComponent(_pane.Component);
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


		private SplitPane _leftPane;
		private SplitPane _rightPane;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitComponentContainer(SplitPane leftPane, SplitPane rightPane)
        {
			_leftPane = leftPane;
			_rightPane = rightPane;

			_leftPane.ComponentHost = new SplitPaneHost(this, _leftPane);
			_rightPane.ComponentHost = new SplitPaneHost(this, _rightPane);
		}

		public SplitPane LeftPane
		{
			get { return _leftPane; }
		}

		public SplitPane RightPane
		{
			get { return _rightPane; }
		}

		public override void Start()
        {
			base.Start();

			_leftPane.ComponentHost.Start();
			_rightPane.ComponentHost.Start();
        }

        public override void Stop()
        {
            _leftPane.Component.Stop();
			_rightPane.Component.Stop();

            base.Stop();
        }
    }
}
