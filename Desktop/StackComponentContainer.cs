using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="StackComponentContainer"/>
    /// </summary>
    [ExtensionPoint]
    public class StackComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StackComponentContainer class
    /// </summary>
    [AssociateView(typeof(StackComponentContainerViewExtensionPoint))]
    public class StackComponentContainer : ApplicationComponentContainer
    {
        public class ComponentHost : ApplicationComponentHost
        {
            private StackComponentContainer _container;
            private bool _exitRequestedByComponent;

            public ComponentHost(StackComponentContainer container, IApplicationComponent component)
                :base(component)
	        {
                _container = container;
	        }

            public override void Exit()
            {
                if (this.Component != _container._hostStack.Peek().Component)
                    throw new InvalidOperationException("Component cannot exit because it is not at the top of the component stack.");

                _exitRequestedByComponent = true;
                _container.Pop();
            }

            public override IDesktopWindow  DesktopWindow
            {
	            get { return _container.Host.DesktopWindow; }
            }

            internal bool ExitRequestedByComponent
            {
                get { return _exitRequestedByComponent; }
            }
        }

        private Stack<ComponentHost> _hostStack;
        private event EventHandler _topmostChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public StackComponentContainer()
        {
            _hostStack = new Stack<ComponentHost>();
        }

        public void Push(IApplicationComponent component)
        {
            ComponentHost host = new ComponentHost(this, component);
            _hostStack.Push(host);

            if (this.IsStarted)
            {
                host.StartComponent();

                // notify view
                EventsHelper.Fire(_topmostChanged, this, EventArgs.Empty);
            }
        }

        public IApplicationComponent Pop()
        {
            if(_hostStack.Count == 0)
                throw new InvalidOperationException("Component stack is empty");

            if (this.IsStarted)
            {
                ComponentHost top = _hostStack.Peek();
                if (top.ExitRequestedByComponent || top.Component.CanExit())
                {
                    _hostStack.Pop();
                    top.StopComponent();

                    // the next component may not have been started, in which case we need to start it now
                    ComponentHost next = _hostStack.Peek();
                    if (!next.Component.IsStarted)
                        next.StartComponent();

                    // notify view
                    EventsHelper.Fire(_topmostChanged, this, EventArgs.Empty);

                    return top.Component;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return _hostStack.Pop().Component;
            }
        }

        public override void Start()
        {
            // start the topmost component on the stack
            _hostStack.Peek().StartComponent();

            base.Start();
        }

        public override void Stop()
        {
            foreach (ComponentHost host in _hostStack)
            {
                if (host.Component.IsStarted)
                    host.StopComponent();
            }

            base.Stop();
        }

        #region Presentation Model

        public ComponentHost Topmost
        {
            get { return _hostStack.Count == 0 ? null : _hostStack.Peek(); }
        }

        public event EventHandler TopmostChanged
        {
            add { _topmostChanged += value; }
            remove { _topmostChanged -= value; }
        }

        #endregion
    }
}
