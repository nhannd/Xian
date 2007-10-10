using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="CloseHelperComponent"/>
    /// </summary>
    [ExtensionOf(typeof(CloseHelperComponentViewExtensionPoint))]
    public class CloseHelperComponentView : WinFormsView, IApplicationComponentView
    {
        private CloseHelperComponent _component;
        private CloseHelperComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CloseHelperComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CloseHelperComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
