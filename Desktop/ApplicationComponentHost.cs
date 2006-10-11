using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class ApplicationComponentHost : IApplicationComponentHost
    {
        private IApplicationComponent _component;
        private IApplicationComponentView _componentView;
        private IDesktopWindow _window;

        public ApplicationComponentHost(IApplicationComponent component, IDesktopWindow window)
        {
            _window = window;
            _component = component;
            _component.SetHost(this);
        }

        public IApplicationComponent Component
        {
            get { return _component; }
        }

        public IApplicationComponentView ComponentView
        {
            get
            {
                if (_componentView == null)
                {
                    _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_component.GetType());
                    _componentView.SetComponent(_component);
                }
                return _componentView;
            }
        }
	
	

        #region IApplicationComponentHost Members

        public virtual void Exit()
        {
            throw new NotSupportedException();
        }

        public virtual CommandHistory CommandHistory
        {
            get { throw new NotSupportedException(); }
        }

        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            return Platform.ShowMessageBox(message, buttons);
        }

        public virtual IDesktopWindow DesktopWindow
        {
            get { return _window; }
        }

        #endregion
    }
}
