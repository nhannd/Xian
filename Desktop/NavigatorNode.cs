using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class NavigatorNode : IApplicationComponentHost
    {
        private IApplicationComponent _component;
        private IApplicationComponentView _view;
        private Path _path;
        private bool _started;

        public NavigatorNode(string path, IApplicationComponent component)
        {
            _path = ClearCanvas.Desktop.Path.ParseAndLocalize(path, new ResourceResolver(new Assembly[] { component.GetType().Assembly }));
            _component = component;
            _started = false;
        }

        public Path NodePath
        {
            get { return _path; }
        }

        internal bool Started
        {
            get { return _started; }
        }

        internal void Start()
        {
            if (!_started)
            {
                _component.SetHost(this);
                _component.Start();
            }
        }

        internal void Stop()
        {
            if (_started)
                _component.Stop();
        }

        public IApplicationComponentView ComponentView
        {
            get
            {
                if (_view == null)
                {
                    IExtensionPoint viewExtensionPoint = ApplicationComponent.GetViewExtensionPoint(_component.GetType());
                    _view = (IApplicationComponentView)ViewFactory.CreateView(viewExtensionPoint);
                    _view.SetComponent(_component);
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

        #endregion

    }
}
