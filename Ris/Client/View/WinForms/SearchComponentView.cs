using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    [ExtensionOf(typeof(SearchComponentViewExtensionPoint))]
    public class SearchComponentView : WinFormsView, IApplicationComponentView
    {
        private SearchComponent _component;
        private SearchComponentControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SearchComponentControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SearchComponent)component;
        }

        #endregion

    }
}
