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

	public enum SplitOrientation
	{
		Horizontal = 0,
		Vertical = 1
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


		private SplitPane _pane1;
		private SplitPane _pane2;
		private SplitOrientation _splitOrientation;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitComponentContainer(
			SplitPane pane1, 
			SplitPane pane2, 
			SplitOrientation splitOrientation)
        {
			_pane1 = pane1;
			_pane2 = pane2;

			_pane1.ComponentHost = new SplitPaneHost(this, _pane1);
			_pane2.ComponentHost = new SplitPaneHost(this, _pane2);

			_splitOrientation = splitOrientation;
		}

		public SplitPane Pane1
		{
			get { return _pane1; }
		}

		public SplitPane Pane2
		{
			get { return _pane2; }
		}

		public SplitOrientation SplitOrientation
		{
			get { return _splitOrientation; }
		}

		public override void Start()
        {
			base.Start();

			_pane1.ComponentHost.Start();
			_pane2.ComponentHost.Start();
        }

        public override void Stop()
        {
            _pane1.Component.Stop();
			_pane2.Component.Stop();

            base.Stop();
        }
    }
}
