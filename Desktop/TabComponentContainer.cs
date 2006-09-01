using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="TabComponent"/>
    /// </summary>
    public class TabComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// An application component that acts as a container for other application components.
    /// The child components are treated as "pages", where each page is a node in a tree.
    /// Only one page is displayed at a time, however, a navigation tree is provided on the side
    /// to aid the user in navigating the set of pages.
    /// </summary>
    [AssociateView(typeof(TabComponentContainerViewExtensionPoint))]
    public class TabComponentContainer : ApplicationComponentContainer
    {
        /// <summary>
        /// Defines an application component host for one page.
        /// </summary>
        public class PageHost : IApplicationComponentHost
        {
            private TabComponentContainer _owner;
            private TabPage _page;
            private IApplicationComponentView _view;
            private bool _started;

            internal PageHost(TabComponentContainer owner, TabPage page)
            {
                _owner = owner;
                _page = page;
                _started = false;
            }

            public TabComponentContainer Owner
            {
                get { return _owner; }
            }

            public TabPage Page
            {
                get { return _page; }
            }

            public bool Started
            {
                get { return _started; }
            }

            public void Start()
            {
                if (!_started)
                {
                    _page.Component.SetHost(this);
                    _page.Component.Start();
					_started = true;
                }
            }

            public void Stop()
            {
                if (_started)
                    _page.Component.Stop();
            }

            public IApplicationComponentView ComponentView
            {
                get
                {
                    if (_view == null)
                    {
                        _view = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_page.Component.GetType());
                        _view.SetComponent(_page.Component);
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

        class TabPageList : ObservableList<TabPage, CollectionEventArgs<TabPage>>
        {
        }


        private TabPageList _pages;

        private int _current;
        private event EventHandler _currentPageChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TabComponentContainer()
        {
            _pages = new TabPageList();
            _pages.ItemAdded += delegate(object sender, CollectionEventArgs<TabPage> args)
                {
                    args.Item.ComponentHost = new PageHost(this, args.Item);
                };
            _pages.ItemRemoved += delegate(object sender, CollectionEventArgs<TabPage> args)
                {
                    args.Item.ComponentHost = null;
                };

            _current = -1;
        }

        public override void Start()
        {
            base.Start();

            MoveTo(0);
        }

        public override void Stop()
        {
            StopAll();
            base.Stop();
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public TabPage CurrentPage
        {
            get { return _pages[_current]; }
            set
            {
				if (this.CurrentPage != value)
				{
					int i = _pages.IndexOf(value);
					if (i > -1 && i != _current)
					{
						MoveTo(i);
					}
				}
            }
        }

        /// <summary>
        /// Notifies that the current page has changed.
        /// </summary>
        public event EventHandler CurrentPageChanged
        {
            add { _currentPageChanged += value; }
            remove { _currentPageChanged -= value; }
        }

        /// <summary>
        /// Returns the current set of pages.
        /// </summary>
        public IList<TabPage> Pages
        {
            get { return _pages; }
        }


        /// <summary>
        /// Moves to the page at the specified index
        /// </summary>
        /// <param name="index"></param>
        private void MoveTo(int index)
        {
            if (index > -1 && index < _pages.Count)
            {
                _current = index;
                TabPage page = _pages[_current];
                if (!page.ComponentHost.Started)
                {
                    page.ComponentHost.Start();
                }

                EventsHelper.Fire(_currentPageChanged, this, new EventArgs());
            }
        }

        /// <summary>
        /// Calls <see cref="IApplicationComponent.Stop"/> on all child components.
        /// </summary>
        private void StopAll()
        {
            foreach (TabPage page in _pages)
            {
                if (page.ComponentHost.Started)
                {
                    page.ComponentHost.Stop();
                }
            }
        }
    }
}
