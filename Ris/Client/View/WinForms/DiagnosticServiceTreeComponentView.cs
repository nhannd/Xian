using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DiagnosticServiceTreeComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DiagnosticServiceTreeComponentViewExtensionPoint))]
    public class DiagnosticServiceTreeComponentView : WinFormsView, IApplicationComponentView
    {
        private DiagnosticServiceTreeComponent _component;
        private DiagnosticServiceTreeComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DiagnosticServiceTreeComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DiagnosticServiceTreeComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
