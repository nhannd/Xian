using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(NavigatorComponentContainerViewExtensionPoint))]
    public class NavigatorComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private NavigatorComponentContainer _component;
        private NavigatorComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (NavigatorComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new NavigatorComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
