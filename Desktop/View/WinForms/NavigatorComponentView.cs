using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(NavigatorComponentViewExtensionPoint))]
    public class NavigatorComponentView : WinFormsView, IApplicationComponentView
    {
        private NavigatorComponent _component;
        private NavigatorComponentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (NavigatorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new NavigatorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
