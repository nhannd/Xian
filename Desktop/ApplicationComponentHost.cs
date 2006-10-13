using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public abstract class ApplicationComponentHost : IApplicationComponentHost
    {
        private IApplicationComponent _component;
        private IApplicationComponentView _componentView;

        public ApplicationComponentHost(IApplicationComponent component)
        {
            _component = component;
            _component.SetHost(this);
        }

        public virtual void StartComponent()
        {
            _component.Start();
        }

        public virtual void StopComponent()
        {
            _component.Stop();
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

        /// <summary>
        /// Not supported.  Override this method to add support.
        /// </summary>
        public virtual void Exit()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported. Override this method to add support.
        /// </summary>
        public virtual CommandHistory CommandHistory
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Shows a message box in the associated desktop window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            return Platform.ShowMessageBox(message, buttons);
        }

        /// <summary>
        /// Returns the associated desktop window
        /// </summary>
        public abstract IDesktopWindow DesktopWindow { get; }

        #endregion
    }
}
