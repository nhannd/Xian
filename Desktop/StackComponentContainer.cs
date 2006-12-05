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
    /// ******************************************************************
    /// NB. This class is severely broken!!! Do not use this container
    /// *******************************************************************
    /// </summary>
    [AssociateView(typeof(StackComponentContainerViewExtensionPoint))]
    public class StackComponentContainer : ApplicationComponentContainer
    {
        public class ComponentHost : ApplicationComponentHost
        {
            private StackComponentContainer _container;
            private bool _exitRequestedByComponent;

            public ComponentHost(StackComponentContainer container, IApplicationComponent component, ApplicationComponentExitDelegate exitCallback)
                :base(component, exitCallback)
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
            Push(component, null);
        }

        public void Push(IApplicationComponent component, ApplicationComponentExitDelegate exitCallback)
        {
            ComponentHost host = new ComponentHost(this, component, exitCallback);
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

                    // the last Pop may have caused this entire component to be stopped, so we need to check again
                    // that we are still "started", and only in this case do we start the next component down
                    if (this.IsStarted)
                    {
                        // the next component may not have been started, in which case we need to start it now
                        ComponentHost next = _hostStack.Peek();
                        if (!next.Component.IsStarted)
                            next.StartComponent();

                        // notify view
                        EventsHelper.Fire(_topmostChanged, this, EventArgs.Empty);
                    }

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

        public IApplicationComponent Peek()
        {
            return this.Topmost.Component;
        }

        #region ApplicationComponent overrides

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

        public override bool Modified
        {
            get
            {
                foreach (ComponentHost host in _hostStack)
                {
                    if (host.Component.Modified)
                        return true;
                }
                return false;
            }
            protected set
            {
            }
        }

        #endregion

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

        protected override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
