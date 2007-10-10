using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomServerEditComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomServerEditComponentViewExtensionPoint))]
    public class DicomServerEditComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomServerEditComponent _component;
        private DicomServerEditComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomServerEditComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomServerEditComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
