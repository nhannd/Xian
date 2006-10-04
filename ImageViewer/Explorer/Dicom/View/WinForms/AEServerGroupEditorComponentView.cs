using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="AEServerGroupEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(AEServerGroupEditorComponentViewExtensionPoint))]
    public class AEServerGroupEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private AEServerGroupEditorComponent _component;
        private AEServerGroupEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AEServerGroupEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new AEServerGroupEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
