using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Utilities.DicomEditor;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    [ExtensionOf(typeof(DicomEditorComponentViewExtensionPoint))]
    public class DicomEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomEditorComponent _component;
        private DicomEditorControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomEditorControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomEditorComponent)component;
        }

        #endregion
    }
}
