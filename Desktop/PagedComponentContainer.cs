#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Helper class for <see cref="PagedComponentContainer{TPage}"/>.
    /// </summary>
    public abstract class ContainerPage
    {
        private readonly IApplicationComponent _component;
    	private bool _lazyStart = true;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="component">The <see cref="IApplicationComponent"/> to host 
		/// in a page within the <see cref="PagedComponentContainer{TPage}"/>.</param>
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

		/// <summary>
		/// Gets or sets a value indicating whether the component will be started lazily.
		/// </summary>
		/// <remarks>
		/// This property is true by default, meaning the component will not be started
		/// until the containing page is accessed.  Changing this to false will cause
		/// the component to start when the container is started.
		/// </remarks>
    	public bool LazyStart
    	{
			get { return _lazyStart; }
			set { _lazyStart = value; }
    	}
    }

    /// <summary>
    /// Abstract base class for application component containers that support multiple pages.
    /// </summary>
    /// <typeparam name="TPage">The type of the page in the container.</typeparam>
    public class PagedComponentContainer<TPage> : ApplicationComponentContainer
        where TPage : ContainerPage
    {
        /// <summary>
        /// Defines an application component host for one page.
        /// </summary>
        private class PageHost : ApplicationComponentHost
        {
            private readonly PagedComponentContainer<TPage> _container;

            internal PageHost(PagedComponentContainer<TPage> container, ContainerPage page)
                : base(page.Component)
            {
                _container = container;
            }

            #region ApplicationComponentHost overrides

			/// <summary>
			/// Gets the <see cref="DesktopWindow"/> that owns the <see cref="PagedComponentContainer{TPage}"/>.
			/// </summary>
            public override DesktopWindow DesktopWindow
            {
                get { return _container.Host.DesktopWindow; }
            }

			/// <summary>
			/// Gets the title of the <see cref="PagedComponentContainer{TPage}"/>.
			/// </summary>
			/// <remarks>
			/// The set method is unsupported and will throw an exception.
			/// </remarks>
            public override string Title
            {
                get { return _container.Host.Title; }
                // individual components cannot set the title for the container
                set { throw new NotSupportedException(); }
            }
            
            #endregion
        }

        private class PageList : ObservableList<TPage>
        {
        	private readonly PagedComponentContainer<TPage> _owner;

			public PageList(PagedComponentContainer<TPage> owner)
			{
				_owner = owner;
			}

			protected override void OnItemAdded(ListEventArgs<TPage> e)
			{
				// create page host
				_owner._mapPageToHost.Add(e.Item, new PageHost(_owner, e.Item));

				// fire event
				base.OnItemAdded(e);

				// if this is the first page, move to it
				if (_owner._current == -1)
					_owner.MoveTo(0);
			}

			protected override void OnItemRemoved(ListEventArgs<TPage> e)
			{
				// fire event
				base.OnItemRemoved(e);

				if (_owner.Pages.Count == 0)
					_owner._current = -1;


				// stop page component and remove page host
				_owner.EnsureStopped(e.Item);
				_owner._mapPageToHost.Remove(e.Item);
			}
        }


        private readonly PageList _pages;
        private readonly Dictionary<ContainerPage, PageHost> _mapPageToHost;

        private int _current;
        private event EventHandler _currentPageChanged;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PagedComponentContainer()
        {
            _mapPageToHost = new Dictionary<ContainerPage, PageHost>();
            _pages = new PageList(this);

            _current = -1;
        }

        /// <summary>
        /// Returns the current set of pages.
        /// </summary>
        public ObservableList<TPage> Pages
        {
            get { return _pages; }
        }

        #region ApplicationComponent overrides

    	/// <summary>
    	/// Called by the host to initialize the application component.
    	/// </summary>
    	///  <remarks>
    	/// <para>
    	/// Automatically moves to and starts the first contained page.
    	/// </para>
    	/// <para>
    	/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </para>
    	/// </remarks>
    	public override void Start()
        {
    		foreach (TPage page in _pages)
    		{
    			if(!page.LazyStart)
					EnsureStarted(page);
    		}

            if (_current < 0)
				MoveTo(0);

			base.Start();
		}

    	/// <summary>
    	/// Called by the host when the application component is being terminated.
    	/// </summary>
    	/// <remarks>
    	/// <para>
    	/// Calls <see cref="ApplicationComponent.Stop"/> on all contained <see cref="IApplicationComponent"/>s.
    	/// </para>
    	/// <para>
    	/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </para>
		/// </remarks>
    	public override void Stop()
        {
            StopAll();
            base.Stop();
        }

        #endregion

        #region ApplicationComponentContainer overrides

    	/// <summary>
    	/// Gets an enumeration of the contained components.
    	/// </summary>
    	public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get
            {
                return CollectionUtils.Map<ContainerPage, IApplicationComponent>(_pages,
                    delegate(ContainerPage p) { return p.Component; });
            }
        }

    	/// <summary>
    	/// Gets an enumeration of the contained components that are currently visible.
    	/// </summary>
    	public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get
            {
                return new IApplicationComponent[] { this.CurrentPage.Component };
            }
        }

    	/// <summary>
    	/// Ensures that the specified component is visible.
    	/// </summary>
    	public override void EnsureVisible(IApplicationComponent component)
        {
            TPage page = CollectionUtils.SelectFirst<TPage>(_pages,
                delegate(TPage p) { return p.Component == component; });

            this.CurrentPage = page;
        }

    	/// <summary>
    	/// Ensures that the specified component has been started.
    	/// </summary>
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
        public IApplicationComponentView GetPageView(ContainerPage page)
        {
            PageHost host = _mapPageToHost[page];
            return host.ComponentView;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Moves to the page at the specified index.
        /// </summary>
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

		/// <summary>
		/// Ensures that the specified <see cref="ContainerPage"/> is started, regardless of whether or not it is visible.
		/// </summary>
        protected void EnsureStarted(ContainerPage page)
        {
            PageHost host = _mapPageToHost[page];
            if (!host.IsStarted)
            {
                page.Component.ModifiedChanged += Component_ModifiedChanged;
                host.StartComponent();
            }
        }

		/// <summary>
		/// Ensures that the specified <see cref="ContainerPage"/> is stopped, regardless of whether or not it is visible.
		/// </summary>
		/// <param name="page"></param>
		protected void EnsureStopped(ContainerPage page)
		{
			PageHost host = _mapPageToHost[page];
			if (host.IsStarted)
			{
				host.StopComponent();
				page.Component.ModifiedChanged -= Component_ModifiedChanged;
			}
		}

        /// <summary>
        /// Calls <see cref="IApplicationComponent.Stop"/> on all child components.
        /// </summary>
        private void StopAll()
        {
            foreach (TPage page in _pages)
            {
				EnsureStopped(page);
            }
        }


        /// <summary>
        /// True if <see cref="IApplicationComponent.Modified"/> returns true for any child component.
        /// </summary>
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

		/// <summary>
		/// Does nothing unless overridden.
		/// </summary>
		/// <remarks>
		/// This method is called each time a child component's <see cref="IApplicationComponent.ModifiedChanged"/>
		/// event has fired.  Override this method when custom handling is required for the container.
		/// </remarks>
		/// <param name="component">The component whose <see cref="IApplicationComponent.ModifiedChanged"/> event has fired.</param>
        protected virtual void OnComponentModifiedChanged(IApplicationComponent component)
        {
        }

        #endregion
    }
}
