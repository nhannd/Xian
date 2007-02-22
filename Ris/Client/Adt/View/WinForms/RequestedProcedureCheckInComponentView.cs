using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RequestedProcedureCheckInComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RequestedProcedureCheckInComponentViewExtensionPoint))]
    public class RequestedProcedureCheckInComponentView : WinFormsView, IApplicationComponentView
    {
        private RequestedProcedureCheckInComponent _component;
        private RequestedProcedureCheckInComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RequestedProcedureCheckInComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RequestedProcedureCheckInComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
