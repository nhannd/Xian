using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class NavigatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(NavigatorComponentViewExtensionPoint))]
    public class NavigatorComponent : ApplicationComponent
    {
        private List<NavigatorNode> _nodes;
        private int _current;
        private event EventHandler _currentNodeChanged;

        private bool _forwardEnabled;
        private event EventHandler _forwardEnabledChanged;

        private bool _backEnabled;
        private event EventHandler _backEnabledChanged;

        private bool _acceptEnabled;
        private event EventHandler _acceptEnabledChanged;

        public NavigatorComponent()
        {
            _nodes = new List<NavigatorNode>();
            _current = -1;
        }

        public override void Start()
        {
            base.Start();

            foreach (NavigatorNode node in _nodes)
            {
            }

            MoveTo(0);
        }

        public override void Stop()
        {
            StopAll();
            base.Stop();
        }

        private void Component_ModifiedChanged(object sender, EventArgs e)
        {
            this.AcceptEnabled = this.Modified = AnyNodeModified();
        }

        public NavigatorNode CurrentNode
        {
            get { return _nodes[_current]; }
            set
            {
                int i = _nodes.IndexOf(value);
                if (i > -1 && i != _current)
                {
                    MoveTo(i);
                }
            }
        }

        public event EventHandler CurrentNodeChanged
        {
            add { _currentNodeChanged += value; }
            remove { _currentNodeChanged -= value; }
        }

        public IList<NavigatorNode> Nodes
        {
            get { return _nodes; }
        }

        public void Forward()
        {
            MoveTo(_current + 1);
        }

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

        public event EventHandler ForwardEnabledChanged
        {
            add { _forwardEnabledChanged += value; }
            remove { _forwardEnabledChanged -= value; }
        }

        public void Back()
        {
            MoveTo(_current - 1);
        }

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

        public event EventHandler BackEnabledChanged
        {
            add { _backEnabledChanged += value; }
            remove { _backEnabledChanged -= value; }
        }
        
        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

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

        public event EventHandler AcceptEnabledChanged
        {
            add { _acceptEnabledChanged += value; }
            remove { _acceptEnabledChanged -= value; }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        private void MoveTo(int index)
        {
            if (index > -1 && index < _nodes.Count)
            {
                _current = index;
                NavigatorNode node = _nodes[_current];
                if (!node.Started)
                {
                    node.Start();
                    node.Component.ModifiedChanged += Component_ModifiedChanged;
                }

                EventsHelper.Fire(_currentNodeChanged, this, new EventArgs());

                this.ForwardEnabled = (_current < _nodes.Count - 1);
                this.BackEnabled = (_current > 0);
            }
        }

        private void StopAll()
        {
            foreach (NavigatorNode node in _nodes)
            {
                if (node.Started)
                {
                    node.Stop();
                    node.Component.ModifiedChanged -= Component_ModifiedChanged;
                }
            }
        }

        private bool AnyNodeModified()
        {
            foreach (NavigatorNode node in _nodes)
            {
                if (node.Component.Modified)
                    return true;
            }
            return false;
        }
    }
}
