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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressGraphic
	{
		[Cloneable]
		internal class ProgressCompositeGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private readonly Size _size = new Size(240, 80);

			[CloneIgnore]
			private ITextGraphic _progressTextGraphic;

			[CloneIgnore]
			private ProgressBarGraphic _progressBarGraphic;

			[CloneIgnore]
			private ColorImageGraphic _background;

			[CloneIgnore]
			private readonly ProgressBarGraphicStyle _style;

			public ProgressCompositeGraphic(ProgressBarGraphicStyle style)
			{
				_style = style;

				Initialize();
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ProgressCompositeGraphic(ProgressCompositeGraphic source, ICloningContext context)
			{
				context.CloneFields(source, this);

				_style = source._style;
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_progressTextGraphic = (ITextGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ITextGraphic);
				_progressBarGraphic = (ProgressBarGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ProgressBarGraphic);
				_background = (ColorImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ColorImageGraphic);

				Initialize();
			}

			private void Initialize()
			{
				if (_background == null)
				{
					byte[] pixelData = new byte[4*_size.Width*_size.Height];
					for (int n = 0; n < pixelData.Length; n += 4)
					{
						byte[] pixel = BitConverter.GetBytes(Color.FromArgb(196, 85, 85, 85).ToArgb());
						pixelData[n + 0] = pixel[0];
						pixelData[n + 1] = pixel[1];
						pixelData[n + 2] = pixel[2];
						pixelData[n + 3] = pixel[3];
					}
					base.Graphics.Add(_background = new ColorImageGraphic(_size.Height, _size.Width, pixelData));
				}

				if (_progressBarGraphic == null)
				{
					base.Graphics.Add(_progressBarGraphic = ProgressBarGraphic.Create(_style));
					var offset = Center(_size, _progressBarGraphic.Size) + new Size(0, 10);
					_progressBarGraphic.SpatialTransform.TranslationX = offset.X;
					_progressBarGraphic.SpatialTransform.TranslationY = offset.Y;
				}

				if (_progressTextGraphic == null)
				{
					base.Graphics.Add(_progressTextGraphic = new InvariantTextPrimitive());
					var offset = Center(_size, new Size(1, 1)) - new Size(0, 15);
					_progressTextGraphic.SpatialTransform.TranslationX = offset.X;
					_progressTextGraphic.SpatialTransform.TranslationY = offset.Y;
				}
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					SpatialTransform transform = base.SpatialTransform;
					transform.TranslationX = (base.ParentPresentationImage.ClientRectangle.Width - this.Width)/2f;
					transform.TranslationY = (base.ParentPresentationImage.ClientRectangle.Height - this.Height)/2f;
				}
				base.OnDrawing();
			}

			protected override SpatialTransform CreateSpatialTransform()
			{
				return new InvariantSpatialTransform(this);
			}

			public string Text
			{
				get { return _progressTextGraphic.Text; }
				set { _progressTextGraphic.Text = value; }
			}

			public float Progress
			{
				get { return _progressBarGraphic.Progress; }
				set { _progressBarGraphic.Progress = value; }
			}

			public ProgressBarGraphicStyle Style
			{
				get { return _style; }
			}

			public Size Size
			{
				get { return _size; }
			}

			public int Width
			{
				get { return _size.Width; }
			}

			public int Height
			{
				get { return _size.Height; }
			}

			private static Point Center(Size bounds, Size size)
			{
				return new Point((bounds.Width - size.Width)/2, (bounds.Height - size.Height)/2);
			}
		}
	}
}