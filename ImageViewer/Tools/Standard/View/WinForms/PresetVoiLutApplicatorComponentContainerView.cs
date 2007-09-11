using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(PresetVoiLutApplicatorComponentContainerViewExtensionPoint))]
	public class PresetVoiLutApplicatorComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private PresetVoiLutApplicatorComponentContainer _component;
        private PresetVoiLutApplicatorComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PresetVoiLutApplicatorComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PresetVoiLutApplicatorComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
