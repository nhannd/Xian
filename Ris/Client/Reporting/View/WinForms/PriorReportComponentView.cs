using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PriorReportComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PriorReportComponentViewExtensionPoint))]
    public class PriorReportComponentView : WinFormsView, IApplicationComponentView
    {
        private PriorReportComponent _component;
        private PriorReportComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PriorReportComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PriorReportComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
