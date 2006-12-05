using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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
        public class PageHost : ApplicationComponentHost
        {
            private TabComponentContainer _owner;
            private TabPage _page;

            internal PageHost(TabComponentContainer owner, TabPage page)
                :base(page.Component)
            {
                _owner = owner;
                _page = page;
            }

            public TabComponentContainer Owner
            {
                get { return _owner; }
            }

            public TabPage Page
            {
                get { return _page; }
            }

            #region ApplicationComponentHost overrides

            public override IDesktopWindow DesktopWindow
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

        protected override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get
            {
                return CollectionUtils.Map<TabPage, IApplicationComponent>(_pages,
                    delegate(TabPage p) { return p.Component; });
            }
        }

        public override void ShowValidation(bool show)
        {
            // propagate to each page
            base.ShowValidation(show);

            if (show)
            {
                // if there are no errors on the current page, find the first page with errors and switch to it
                if (!this.CurrentPage.Component.HasValidationErrors)
                {
                    TabPage firstPageWithErrors = CollectionUtils.SelectFirst<TabPage>(_pages,
                        delegate(TabPage p) { return p.Component.HasValidationErrors; });
                    if (firstPageWithErrors != null)
                        this.CurrentPage = firstPageWithErrors;
                }
            }
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
                if (!page.ComponentHost.IsStarted)
                {
                    page.ComponentHost.StartComponent();
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
                if (page.ComponentHost.IsStarted)
                {
                    page.ComponentHost.StopComponent();
                }
            }
        }

    }
}
