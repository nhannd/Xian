using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ReportEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ReportEditorComponentViewExtensionPoint))]
    public class ReportEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ReportEditorComponent _component;
        private ReportEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ReportEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ReportEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
