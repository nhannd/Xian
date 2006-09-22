using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="AEServerEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(AEServerEditorComponentViewExtensionPoint))]
    public class AEServerEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private AEServerEditorComponent _component;
        private AEServerEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AEServerEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new AEServerEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
