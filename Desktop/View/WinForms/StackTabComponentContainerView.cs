using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="StackTabComponentContainer"/>
    /// </summary>
    [ExtensionOf(typeof(StackTabComponentContainerViewExtensionPoint))]
    public class StackTabComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private StackTabComponentContainer _component;
        private StackTabComponentContainerControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StackTabComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new StackTabComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
