using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TileComponent"/>
    /// </summary>
	[ExtensionOf(typeof(ImageBoxViewExtensionPoint))]
    public class ImageBoxView : WinFormsView, IView
    {
		private ImageBox _imageBox;
		private ImageBoxControl _imageBoxControl;

		public ImageBox ImageBox
		{
			get { return _imageBox; }
			set { _imageBox = value; }
		}

        public override object GuiElement
        {
            get
            {
				// TODO: Should a reference be held?
                if (_imageBoxControl == null)
                {
					_imageBoxControl = new ImageBoxControl(this.ImageBox);
                }
                return _imageBoxControl;
            }
        }
    }
}
