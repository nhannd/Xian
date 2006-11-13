using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using System.Drawing;

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
		private Rectangle _parentRectangle;

		public ImageBox ImageBox
		{
			get { return _imageBox; }
			set { _imageBox = value; }
		}

		public Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set { _parentRectangle = value; }
		}

        public override object GuiElement
        {
            get
            {
                if (_imageBoxControl == null)
                {
					_imageBoxControl = new ImageBoxControl(this.ImageBox, this.ParentRectangle);
                }
                return _imageBoxControl;
            }
        }
    }
}
