using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SearchPanelComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SearchPanelComponentViewExtensionPoint))]
    public class SearchPanelComponentView : WinFormsView, IApplicationComponentView
    {
        private SearchPanelComponent _component;
        private SearchPanelComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SearchPanelComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SearchPanelComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
