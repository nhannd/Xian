using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DowntimePrintFormsComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DowntimePrintFormsComponentViewExtensionPoint))]
    public class DowntimePrintFormsComponentView : WinFormsView, IApplicationComponentView
    {
        private DowntimePrintFormsComponent _component;
        private DowntimePrintFormsComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DowntimePrintFormsComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DowntimePrintFormsComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
