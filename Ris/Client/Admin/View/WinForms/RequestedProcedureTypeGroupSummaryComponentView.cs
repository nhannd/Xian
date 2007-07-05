using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RequestedProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RequestedProcedureTypeGroupSummaryComponentViewExtensionPoint))]
    public class RequestedProcedureTypeGroupSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private RequestedProcedureTypeGroupSummaryComponent _component;
        private RequestedProcedureTypeGroupSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RequestedProcedureTypeGroupSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RequestedProcedureTypeGroupSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
