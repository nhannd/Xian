using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ModalityEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ModalityEditorComponentViewExtensionPoint))]
    public class ModalityEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ModalityEditorComponent _component;
        private ModalityEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ModalityEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ModalityEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
