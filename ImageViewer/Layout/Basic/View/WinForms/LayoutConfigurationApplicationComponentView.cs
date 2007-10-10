using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="LayoutSettingsApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(LayoutConfigurationApplicationComponentViewExtensionPoint))]
    public class LayoutConfigurationApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private LayoutConfigurationApplicationComponent _component;
        private LayoutConfigurationApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LayoutConfigurationApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LayoutConfigurationApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
