using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(TabbedGroupsComponentContainerViewExtensionPoint))]
    public class TabGroupComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private TabGroupComponentContainer _component;
        private TabGroupComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
			Platform.CheckForNullReference(component, "component");
            _component = (TabGroupComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new TabGroupComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
