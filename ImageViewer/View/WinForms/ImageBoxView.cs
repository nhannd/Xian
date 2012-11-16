#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Linq;
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

                    var decorators = new ImageBoxControlDecoratorExtensionPoint().CreateExtensions().Cast < IImageBoxControlDecorator>();
                    foreach(var decorator in decorators)
                    {
                        _imageBoxControl = decorator.Apply(_imageBoxControl, this.ImageBox);
                    }
                }
                return _imageBoxControl;
            }
        }

        
    }

    [ExtensionPoint]
    public class ImageBoxControlDecoratorExtensionPoint:ExtensionPoint<IImageBoxControlDecorator>{}

    public interface IImageBoxControlDecorator
    {
        ImageBoxControl Apply(ImageBoxControl control, ImageBox box);
    }

}
