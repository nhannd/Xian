using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="WorklistSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(WorklistSummaryComponentViewExtensionPoint))]
    public class WorklistSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private WorklistSummaryComponent _component;
        private WorklistSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WorklistSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new WorklistSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
