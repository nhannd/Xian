using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomServerTestComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomServerTestComponentViewExtensionPoint))]
    public class DicomServerTestComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomServerTestComponent _component;
        private DicomServerTestComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomServerTestComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomServerTestComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
