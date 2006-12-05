using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="NavigatorComponent"/>
    /// </summary>
    public class NavigatorComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// An application component that acts as a container for other application components.
    /// The child components are treated as "pages", where each page is a node in a tree.
    /// Only one page is displayed at a time, however, a navigation tree is provided on the side
    /// to aid the user in navigating the set of pages.
    /// </summary>
    [AssociateView(typeof(NavigatorComponentContainerViewExtensionPoint))]
    public class NavigatorComponentContainer : ApplicationComponentContainer
    {
        /// <summary>
        /// Defines an application component host for one page.
        /// </summary>
        public class PageHost : ApplicationComponentHost
        {
            private NavigatorComponentContainer _owner;
            private NavigatorPage _page;

            internal PageHost(NavigatorComponentContainer owner, NavigatorPage page)
                :base(page.Component)
            {
                _owner = owner;
                _page = page;
            }

            public NavigatorComponentContainer Owner
            {
                get { return _owner; }
            }

            public NavigatorPage Page
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

        class NavigatorPageList : ObservableList<NavigatorPage, CollectionEventArgs<NavigatorPage>>
        {
        }




        private NavigatorPageList _pages;

        private int _current;
        private event EventHandler _currentPageChanged;

        private bool _forwardEnabled;
        private event EventHandler _forwardEnabledChanged;

        private bool _backEnabled;
        private event EventHandler _backEnabledChanged;

        private bool _acceptEnabled;
        private event EventHandler _acceptEnabledChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NavigatorComponentContainer()
        {
            _pages = new NavigatorPageList();
            _pages.ItemAdded += delegate(object sender, CollectionEventArgs<NavigatorPage> args)
                {
                    args.Item.ComponentHost = new PageHost(this, args.Item);
                };
            _pages.ItemRemoved += delegate(object sender, CollectionEventArgs<NavigatorPage> args)
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
                return CollectionUtils.Map<NavigatorPage, IApplicationComponent>(_pages,
                    delegate(NavigatorPage p) { return p.Component; });
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
                    NavigatorPage firstPageWithErrors = CollectionUtils.SelectFirst<NavigatorPage>(_pages, 
                        delegate(NavigatorPage p) { return p.Component.HasValidationErrors; });
                    if (firstPageWithErrors != null)
                        this.CurrentPage = firstPageWithErrors;
                }
            }
        }


        #region Presentation Model

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public NavigatorPage CurrentPage
        {
            get { return _pages[_current]; }
            set
            {
                int i = _pages.IndexOf(value);
                if (i > -1 && i != _current)
                {
                    MoveTo(i);
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
        /// Accesses the list of pages
        /// </summary>
        public IList<NavigatorPage> Pages
        {
            get { return _pages; }
        }

        /// <summary>
        /// Advances to the next page
        /// </summary>
        public void Forward()
        {
            MoveTo(_current + 1);
        }

        /// <summary>
        /// Indicates whether it is possible to advance one page.  True unless the current
        /// page is the last page.
        /// </summary>
        public bool ForwardEnabled
        {
            get { return _forwardEnabled; }
            protected set
            {
                if (_forwardEnabled != value)
                {
                    _forwardEnabled = value;
                    EventsHelper.Fire(_forwardEnabledChanged, this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Notifies that the <see cref="ForwardEnabled"/> property has changed.
        /// </summary>
        public event EventHandler ForwardEnabledChanged
        {
            add { _forwardEnabledChanged += value; }
            remove { _forwardEnabledChanged -= value; }
        }

        /// <summary>
        /// Sets the current page back to the previous page.
        /// </summary>
        public void Back()
        {
            MoveTo(_current - 1);
        }

        /// <summary>
        /// Indicates whether it is possible to go back one page.  True unless the current page
        /// is the first page.
        /// </summary>
        public bool BackEnabled
        {
            get { return _backEnabled; }
            protected set
            {
                if (_backEnabled != value)
                {
                    _backEnabled = value;
                    EventsHelper.Fire(_backEnabledChanged, this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Notifies that the <see cref="BackEnabled"/> property has changed.
        /// </summary>
        public event EventHandler BackEnabledChanged
        {
            add { _backEnabledChanged += value; }
            remove { _backEnabledChanged -= value; }
        }
        
        /// <summary>
        /// Causes the component to exit, accepting any changes made by the user. Override this method
        /// if desired.
        /// </summary>
        public virtual void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        /// <summary>
        /// Indicates whether the accept button should be enabled.
        /// </summary>
        public bool AcceptEnabled
        {
            get { return _acceptEnabled; }
            protected set
            {
                if (_acceptEnabled != value)
                {
                    _acceptEnabled = value;
                    EventsHelper.Fire(_acceptEnabledChanged, this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Notifies that the <see cref="AcceptEnabled"/> property has changed.
        /// </summary>
        public event EventHandler AcceptEnabledChanged
        {
            add { _acceptEnabledChanged += value; }
            remove { _acceptEnabledChanged -= value; }
        }

        /// <summary>
        /// Causes the component to exit, discarding any changes made by the user.  Override this method
        /// if desired.
        /// </summary>
        public virtual void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion


        /// <summary>
        /// Moves to the page at the specified index
        /// </summary>
        /// <param name="index"></param>
        private void MoveTo(int index)
        {
            if (index > -1 && index < _pages.Count)
            {
                _current = index;
                NavigatorPage page = _pages[_current];
                if (!page.ComponentHost.IsStarted)
                {
                    page.ComponentHost.StartComponent();
                    page.Component.ModifiedChanged += Component_ModifiedChanged;
                }

                EventsHelper.Fire(_currentPageChanged, this, new EventArgs());

                this.ForwardEnabled = (_current < _pages.Count - 1);
                this.BackEnabled = (_current > 0);
            }
        }

        /// <summary>
        /// Calls <see cref="IApplicationComponent.Stop"/> on all child components.
        /// </summary>
        private void StopAll()
        {
            foreach (NavigatorPage page in _pages)
            {
                if (page.ComponentHost.IsStarted)
                {
                    page.ComponentHost.StopComponent();
                    page.Component.ModifiedChanged -= Component_ModifiedChanged;
                }
            }
        }

        /// <summary>
        /// True if <see cref="IApplicatonComponent.Modified"/> returns true for any child component.
        /// </summary>
        /// <returns></returns>
        private bool AnyPageModified()
        {
            foreach (NavigatorPage page in _pages)
            {
                if (page.Component.Modified)
                    return true;
            }
            return false;
        }

        private void Component_ModifiedChanged(object sender, EventArgs e)
        {
            this.AcceptEnabled = this.Modified = AnyPageModified();
        }

    }
}
