using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomExplorerConfigurationApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomExplorerConfigurationApplicationComponentViewExtensionPoint))]
    public class DicomExplorerConfigurationApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomExplorerConfigurationApplicationComponent _component;
        private DicomExplorerConfigurationApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomExplorerConfigurationApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomExplorerConfigurationApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
