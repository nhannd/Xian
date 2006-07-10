using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.View.WinForms
{
    public abstract class ApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private IApplicationComponent _component;

        protected IApplicationComponent Component
        {
            get { return _component; }
        }

        #region IApplicationComponentView Members

        public virtual void SetComponent(IApplicationComponent component)
        {
            _component = component;
        }

        #endregion
    }
}
