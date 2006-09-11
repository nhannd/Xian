using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ConfirmReconciliationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ConfirmReconciliationComponentViewExtensionPoint))]
    public class ConfirmReconciliationComponentView : WinFormsView, IApplicationComponentView
    {
        private ConfirmReconciliationComponent _component;
        private ConfirmReconciliationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ConfirmReconciliationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ConfirmReconciliationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
