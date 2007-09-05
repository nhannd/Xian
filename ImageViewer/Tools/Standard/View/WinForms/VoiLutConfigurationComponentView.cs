using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="VoiLutConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(VoiLutConfigurationComponentViewExtensionPoint))]
    public class VoiLutConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private VoiLutConfigurationComponent _component;
        private VoiLutConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (VoiLutConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new VoiLutConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
