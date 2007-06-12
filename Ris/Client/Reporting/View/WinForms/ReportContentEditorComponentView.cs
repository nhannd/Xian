using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ReportContentEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ReportContentEditorComponentViewExtensionPoint))]
    public class ReportContentEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ReportContentEditorComponent _component;
        private ReportContentEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ReportContentEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ReportContentEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
