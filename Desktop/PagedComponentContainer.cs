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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Helper class for <see cref="PagedComponentContainer"/>.
    /// </summary>
    public abstract class ContainerPage
    {
        private IApplicationComponent _component;

        public ContainerPage(IApplicationComponent component)
        {
            _component = component;
        }

        /// <summary>
        /// Gets the component associated with the page.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }
    }

    /// <summary>
    /// Abstract base class for application component containers that support multiple pages.
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
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

            public override DesktopWindow DesktopWindow
            {
                get { return _container.Host.DesktopWindow; }
            }

            public override string Title
            {
                get { return _container.Host.Title; }
                // individual components cannot set the title for the container
                set { throw new NotSupportedException(); }
            }
            
            #endregion
        }

        class PageList : ObservableList<TPage, CollectionEventArgs<TPage>>
        {
			public PageList()
			{
			}
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

            if (_current < 0)
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
				if (_current < 0 || this.CurrentPage != value)
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
        /// Gets the index of the current page.
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _current; }
        }

        /// <summary>
        /// Occurs when the current page has changed.
        /// </summary>
        public event EventHandler CurrentPageChanged
        {
            add { _currentPageChanged += value; }
            remove { _currentPageChanged -= value; }
        }

        /// <summary>
        /// Gets the view for the specified page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
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

                try
                {
                    EnsureStarted(_pages[_current]);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
                finally
                {
                    // inform view that page has changed
                    EventsHelper.Fire(_currentPageChanged, this, new EventArgs());
                }
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
