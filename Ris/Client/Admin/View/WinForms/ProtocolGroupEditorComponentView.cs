using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ProtocolGroupEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ProtocolGroupEditorComponentViewExtensionPoint))]
    public class ProtocolGroupEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ProtocolGroupEditorComponent _component;
        private ProtocolGroupEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ProtocolGroupEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ProtocolGroupEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
