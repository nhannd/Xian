using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DiskspaceManagerConfigurationComponentViewExtensionPoint))]
    public class DiskspaceManagerConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private DiskspaceManagerConfigurationComponent _component;
        private DiskspaceManagerConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DiskspaceManagerConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DiskspaceManagerConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
