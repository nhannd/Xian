using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(ApplicationComponentHostShelfViewExtensionPoint))]
    public class ApplicationComponentHostShelfView : WinFormsView, IShelfView
    {
        private ApplicationComponentHostShelf _shelf;
        private IApplicationComponentView _componentView;

        public ApplicationComponentHostShelfView()
        {
        }


        public override object GuiElement
        {
            get { return this.ComponentView.GuiElement; }
        }

        protected IApplicationComponentView ComponentView
        {
            get
            {
                if (_componentView == null)
                {
                    _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_shelf.Component.GetType());
                    _componentView.SetComponent(_shelf.Component);
                }
                return _componentView;
            }
        }
    
        #region IShelfView Members

        public void  SetShelf(IShelf shelf)
        {
            _shelf = (ApplicationComponentHostShelf)shelf;
        }

        #endregion
    }
}
