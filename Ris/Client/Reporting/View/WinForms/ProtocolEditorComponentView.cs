using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ProtocolEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ProtocolEditorComponentViewExtensionPoint))]
    public class ProtocolEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ProtocolEditorComponent _component;
        private ProtocolEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ProtocolEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ProtocolEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
