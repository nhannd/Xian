using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.Standard;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomConfigurationApplicationComponent"/>
    /// </summary>
	[ExtensionOf(typeof(DateFormatApplicationComponentViewExtensionPoint))]
    public class DateFormatApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private DateFormatApplicationComponent _component;
        private DateFormatApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DateFormatApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DateFormatApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
