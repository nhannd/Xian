using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    [ExtensionOf(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponentView : WinFormsView, IApplicationComponentView
    {

        private ImageViewerControl _control;
        private ImageViewerComponent _component;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ImageViewerComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ImageViewerControl(_component);
                }
                return _control;
            }
        }
    }
}
