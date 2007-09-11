using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PresetVoiLutConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PresetVoiLutConfigurationComponentViewExtensionPoint))]
    public class PresetVoiLutConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private PresetVoiLutConfigurationComponent _component;
        private PresetVoiLutConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PresetVoiLutConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PresetVoiLutConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
