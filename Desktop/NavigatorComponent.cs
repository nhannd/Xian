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

        public NavigatorComponent()
        {
            _nodes = new List<NavigatorNode>();
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
            get { return _current < _nodes.Count - 1; }
        }

        public void Back()
        {
            MoveTo(_current - 1);
        }

        public bool BackEnabled
        {
            get { return _current > 0; }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
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
                    node.Start();

                EventsHelper.Fire(_currentNodeChanged, this, new EventArgs());

                NotifyPropertyChanged("ForwardEnabled");
                NotifyPropertyChanged("BackEnabled");
            }
        }

        private void StopAll()
        {
            foreach (NavigatorNode node in _nodes)
            {
                if (node.Started)
                    node.Stop();
            }
        }
    }
}
