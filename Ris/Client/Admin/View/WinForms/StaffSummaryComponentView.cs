using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="StaffSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(StaffSummaryComponentViewExtensionPoint))]
    public class StaffSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private StaffSummaryComponent _component;
        private StaffSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StaffSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new StaffSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
