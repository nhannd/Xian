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
    public class ReconciliationConfirmComponentView : WinFormsView, IApplicationComponentView
    {
        private ReconciliationConfirmComponent _component;
        private ReconciliationConfirmComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ReconciliationConfirmComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ReconciliationConfirmComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
