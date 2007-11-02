using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RequestedProcedureEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RequestedProcedureEditorComponentViewExtensionPoint))]
    public class RequestedProcedureEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private RequestedProcedureEditorComponent _component;
        private RequestedProcedureEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RequestedProcedureEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RequestedProcedureEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
