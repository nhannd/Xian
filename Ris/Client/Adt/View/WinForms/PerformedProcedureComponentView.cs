using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PerformedProcedureComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PerformedProcedureComponentViewExtensionPoint))]
    public class PerformedProcedureComponentView : WinFormsView, IApplicationComponentView
    {
        private PerformedProcedureComponent _component;
        private PerformedProcedureComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PerformedProcedureComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PerformedProcedureComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
