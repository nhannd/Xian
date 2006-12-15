using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

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
        public class SplitPaneHost : ApplicationComponentHost
        {
            private SplitComponentContainer _owner;
			private SplitPane _pane;

            internal SplitPaneHost(
				SplitComponentContainer owner,
				SplitPane pane)
                :base(pane.Component)
            {
				Platform.CheckForNullReference(owner, "owner");
				Platform.CheckForNullReference(pane, "pane");

                _owner = owner;
				_pane = pane;
            }

            public SplitComponentContainer Owner
            {
                get { return _owner; }
            }

            #region ApplicationComponentHost overrides

            public override IDesktopWindow DesktopWindow
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
			this.Pane1 = pane1;
			this.Pane2 = pane2;

			_splitOrientation = splitOrientation;
		}

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitComponentContainer(SplitOrientation splitOrientation)
        {
            _splitOrientation = splitOrientation;
        }


		public SplitPane Pane1
		{
			get { return _pane1; }
            set
            {
                if(_pane1 != null && _pane1.ComponentHost != null && _pane1.ComponentHost.IsStarted)
					throw new InvalidOperationException(SR.ExceptionCannotSetPaneAfterContainerStarted);

                _pane1 = value;
                _pane1.ComponentHost = new SplitPaneHost(this, _pane1);
            }
		}

		public SplitPane Pane2
		{
			get { return _pane2; }
            set
            {
                if (_pane2 != null && _pane2.ComponentHost != null && _pane2.ComponentHost.IsStarted)
					throw new InvalidOperationException(SR.ExceptionCannotSetPaneAfterContainerStarted);

                _pane2 = value;
                _pane2.ComponentHost = new SplitPaneHost(this, _pane2);
            }
        }

		public SplitOrientation SplitOrientation
		{
			get { return _splitOrientation; }
		}

		public override void Start()
        {
			base.Start();

			_pane1.ComponentHost.StartComponent();
            _pane2.ComponentHost.StartComponent();
        }

        public override void Stop()
        {
            _pane1.ComponentHost.StopComponent();
            _pane2.ComponentHost.StopComponent();

            base.Stop();
        }

        public override IActionSet ExportedActions
        {
            get
            {
                // export the actions from both subcomponents
                return _pane1.Component.ExportedActions.Union(_pane2.Component.ExportedActions);
            }
        }

        protected override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { return new IApplicationComponent[] { _pane1.Component, _pane2.Component }; }
        }
    }
}
