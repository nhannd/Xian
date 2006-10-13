using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="StackComponentContainer"/>
    /// </summary>
    [ExtensionOf(typeof(StackComponentContainerViewExtensionPoint))]
    public class StackComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private StackComponentContainer _component;
        private StackComponentContainerControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StackComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new StackComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
