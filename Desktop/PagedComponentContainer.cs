using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    public abstract class ContainerPage
    {
        private IApplicationComponent _component;

        public ContainerPage(IApplicationComponent component)
        {
            _component = component;
        }

        public IApplicationComponent Component
        {
            get { return _component; }
        }
    }

    public class PagedComponentContainer<TPage> : ApplicationComponentContainer
        where TPage : ContainerPage
    {

        /// <summary>
        /// Defines an application component host for one page.
        /// </summary>
        public class PageHost : ApplicationComponentHost
        {
            private PagedComponentContainer<TPage> _container;

            internal PageHost(PagedComponentContainer<TPage> container, ContainerPage page)
                : base(page.Component)
            {
                _container = container;
            }

            #region ApplicationComponentHost overrides

            public override IDesktopWindow DesktopWindow
            {
                get { return _container.Host.DesktopWindow; }
            }

            #endregion
        }

        class PageList : ObservableList<TPage, CollectionEventArgs<TPage>>
        {
        }


        private PageList _pages;
        private Dictionary<ContainerPage, PageHost> _mapPageToHost;

        private int _current;
        private event EventHandler _currentPageChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PagedComponentContainer()
        {
            _mapPageToHost = new Dictionary<ContainerPage, PageHost>();
            _pages = new PageList();
            _pages.ItemAdded += delegate(object sender, CollectionEventArgs<TPage> args)
                {
                    _mapPageToHost.Add(args.Item, new PageHost(this, args.Item));
                };
            _pages.ItemRemoved += delegate(object sender, CollectionEventArgs<TPage> args)
                {
                    _mapPageToHost.Remove(args.Item);
                };

            _current = -1;
        }

        /// <summary>
        /// Returns the current set of pages.
        /// </summary>
        public IList<TPage> Pages
        {
            get { return _pages; }
        }

        #region ApplicationComponent overrides

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

        #endregion

        #region ApplicationComponentContainer overrides

        public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get
            {
                return CollectionUtils.Map<ContainerPage, IApplicationComponent>(_pages,
                    delegate(ContainerPage p) { return p.Component; });
            }
        }

        public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get
            {
                return new IApplicationComponent[] { this.CurrentPage.Component };
            }
        }

        public override void EnsureVisible(IApplicationComponent component)
        {
            TPage page = CollectionUtils.SelectFirst<TPage>(_pages,
                delegate(TPage p) { return p.Component == component; });

            this.CurrentPage = page;
        }

        public override void EnsureStarted(IApplicationComponent component)
        {
            ContainerPage page = CollectionUtils.SelectFirst<ContainerPage>(_pages,
                delegate(ContainerPage p) { return p.Component == component; });

            EnsureStarted(page);
        }

        #endregion

        #region Presentation Model

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public TPage CurrentPage
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

        public int CurrentPageIndex
        {
            get { return _current; }
        }

        /// <summary>
        /// Notifies that the current page has changed.
        /// </summary>
        public event EventHandler CurrentPageChanged
        {
            add { _currentPageChanged += value; }
            remove { _currentPageChanged -= value; }
        }

        public IApplicationComponentView GetPageView(ContainerPage page)
        {
            PageHost host = _mapPageToHost[page];
            return host.ComponentView;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Moves to the page at the specified index
        /// </summary>
        /// <param name="index"></param>
        protected virtual void MoveTo(int index)
        {
            if (index > -1 && index < _pages.Count)
            {
                _current = index;
                EnsureStarted(_pages[_current]);

                EventsHelper.Fire(_currentPageChanged, this, new EventArgs());
            }
        }

        protected void EnsureStarted(ContainerPage page)
        {
            PageHost host = _mapPageToHost[page];
            if (!host.IsStarted)
            {
                host.StartComponent();
                page.Component.ModifiedChanged += Component_ModifiedChanged;
            }
        }

        /// <summary>
        /// Calls <see cref="IApplicationComponent.Stop"/> on all child components.
        /// </summary>
        private void StopAll()
        {
            foreach (TPage page in _pages)
            {
                PageHost host = _mapPageToHost[page];
                if (host.IsStarted)
                {
                    host.StopComponent();
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
            return CollectionUtils.Contains<ContainerPage>(_pages,
                delegate(ContainerPage page) { return page.Component.IsStarted && page.Component.Modified; });
        }

        private void Component_ModifiedChanged(object sender, EventArgs e)
        {
            this.Modified = AnyPageModified();
            OnComponentModifiedChanged((IApplicationComponent)sender);
        }

        protected virtual void OnComponentModifiedChanged(IApplicationComponent component)
        {
            // do nothing
        }

        #endregion
    }
}
