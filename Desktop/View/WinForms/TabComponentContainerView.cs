using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(TabComponentContainerViewExtensionPoint))]
    public class TabComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private TabComponentContainer _component;
        private TabComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
			Platform.CheckForNullReference(component, "component");
            _component = (TabComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new TabComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
