using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;
using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DicomGraphics
{
	[Cloneable(true)]
	public class OverlayPlaneGraphic : CompositeGraphic
	{
		private GrayscaleImageGraphic _overlayGraphic;

		private readonly int _index;
		private readonly bool _isBitmapShutter;
		private readonly string _description;
		private ushort _grayPresentationValue = 0;
		private Color? _color;

		internal OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, bool isBitmapShutter)
		{
			_isBitmapShutter = isBitmapShutter;
			_index = overlayPlaneIod.Index;
			_description = overlayPlaneIod.OverlayDescription;

			GrayscaleImageGraphic overlayImageGraphic = CreateOverlayImageGraphic(overlayPlaneIod);
			if (overlayImageGraphic != null)
			{
				_overlayGraphic = overlayImageGraphic;
				Color = System.Drawing.Color.PeachPuff;
				base.Graphics.Add(overlayImageGraphic);
			}

			if (string.IsNullOrEmpty(overlayPlaneIod.OverlayLabel))
			{
				if (isBitmapShutter)
					base.Name = "Bitmap Shutter";
				else
					base.Name = string.Format("Overlay Plane #{0}", _index);
			}
			else
			{
				base.Name = overlayPlaneIod.OverlayLabel;
			}
		}

		//for cloning; all the underlying graphics are already cloneable.
		private OverlayPlaneGraphic()
		{
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_overlayGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                            delegate(IGraphic graphic)
			                                            	{
			                                            		return graphic is GrayscaleImageGraphic;
			                                            	}) as GrayscaleImageGraphic;
		}

		private static GrayscaleImageGraphic CreateOverlayImageGraphic(OverlayPlane overlayPlaneIod)
		{
			Point origin = (overlayPlaneIod.OverlayOrigin ?? new Point(1, 1)) - new Size(1, 1);
			byte[] overlayData = overlayPlaneIod.OverlayData;
			int rows = overlayPlaneIod.OverlayRows;
			int cols = overlayPlaneIod.OverlayColumns;
			int bitsAllocated = overlayPlaneIod.OverlayBitsAllocated;
			int highBit = overlayPlaneIod.OverlayBitPosition;

			if (overlayData == null || overlayData.Length == 0)
			{
				// data is stored in unused bits of pixel data
				overlayData = overlayPlaneIod.DicomAttributeProvider[DicomTags.PixelData].Values as byte[];
			}
			else
			{
				// data is stored in overlay data
				overlayData = new OverlayData(rows, cols, overlayPlaneIod.IsBigEndianOW, overlayData).ToPixelData().Raw;
				bitsAllocated = 8;
				highBit = 1;
			}

			if (overlayData == null)
				throw new Exception("Overlay plane data is invalid.");

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(
				rows, cols, // the reported overlay dimensions
				bitsAllocated, // the reported bits allocated
				1, // overlays always have bit depth of 1
				highBit, // the reported bit position
				false, false, // overlays aren't signed and don't get inverted
				1, 0, // overlays have no rescale
				overlayData); // the overlay data, or the pixel data if overlay data doesn't exist

			imageGraphic.SpatialTransform.TranslationX = origin.X;
			imageGraphic.SpatialTransform.TranslationY = origin.Y;

			return imageGraphic;
		}

		private void UpdateLuts()
		{
			if (_color == null)
			{
				_overlayGraphic.VoiLutManager.InstallLut(new OverlayVoiLut(_grayPresentationValue, 65535));
				//Install a color map with the first value being transparent.
				_overlayGraphic.ColorMapManager.InstallColorMap(new GrayscaleColorMap());
			}
			else
			{
				//The color makes the gray p-value irrelevant, so do this to save space.
				_overlayGraphic.VoiLutManager.InstallLut(new OverlayVoiLut(1, 1));
				_overlayGraphic.ColorMapManager.InstallColorMap(new OverlayColorMap(_color.Value));
			}
		}

		protected GrayscaleImageGraphic OverlayImageGraphic
		{
			get { return _overlayGraphic; }
		}

		public bool IsBitmapShutter
		{
			get { return _isBitmapShutter; }
		}

		public string Description
		{
			get { return _description; }
		}

		public ushort GrayPresentationValue
		{
			get { return _grayPresentationValue; }
			set
			{
				if (_grayPresentationValue == value)
					return;

				Platform.CheckArgumentRange(value, 0, 65535, "_grayPresentationValue");

				_grayPresentationValue = value;
				UpdateLuts();
			}
		}

		public Color? Color
		{
			get { return _color; }
			set
			{
				if (_color == value)
					return;

				_color = value;
				UpdateLuts();
			}
		}

		#region Voi Lut

		[Cloneable(true)]
		private class OverlayVoiLut : ComposableLut
		{
			private readonly int _presentationValue;
			private int _minInputValue = 0;
			private int _maxInputValue = 0;
			private readonly int _maxOutputValue;

			public OverlayVoiLut(int presentationValue, int maxOutputValue)
			{
				_presentationValue = presentationValue;
				_maxOutputValue = maxOutputValue;
			}

			//for cloning.
			private OverlayVoiLut()
			{
			}

			public override int MinInputValue
			{
				get { return _minInputValue; }
				set { _minInputValue = value; }
			}

			public override int MaxInputValue
			{
				get { return _maxInputValue; }
				set { _maxInputValue = value; }
			}

			public override int MinOutputValue
			{
				get { return 0; }
				protected set
				{
				}
			}

			public override int MaxOutputValue
			{
				get { return _maxOutputValue; }
				protected set
				{
				}
			}

			public override int this[int index]
			{
				get
				{
					if (index <= 0)
						return 0;

					return _presentationValue;
				}
				protected set
				{
					throw new InvalidOperationException("This lut is not editable.");
				}
			}

			public override string GetKey()
			{
				return String.Format("OverlayVoi_{0}_{1}_{2}", this.MinInputValue, this.MaxInputValue, _presentationValue);
			}

			public override string GetDescription()
			{
				return "Overlay Voi";
			}
		}

		#endregion

		#region Color Map

		[Cloneable(true)]
		private class GrayscaleColorMap : Imaging.GrayscaleColorMap
		{
			public GrayscaleColorMap()
			{
			}

			protected override void Create()
			{
				base.Create();
				//zero values are transparent, all others are opaque.
				this[MinInputValue] = 0x0;
			}

			public override string GetKey()
			{
				return String.Format("OverlayGrayColorMap_{0}_{1}", this.MinInputValue, this.MaxInputValue);
			}
		}

		[Cloneable(true)]
		private class OverlayColorMap : ColorMap
		{
			private Color _color = Color.Gray;

			public OverlayColorMap()
			{
			}

			public OverlayColorMap(Color color)
			{
				_color = color;
			}

			public Color Color
			{
				get { return _color; }
				set
				{
					if (_color != value)
					{
						_color = value;
						this.Clear();
					}
				}
			}

			protected override void Create()
			{
				this[MinInputValue] = Color.FromArgb(0, _color).ToArgb();

				for (int i = this.MinInputValue + 1; i <= this.MaxInputValue; i++)
					this[i] = Color.FromArgb(255, _color).ToArgb();
			}

			public override string GetDescription()
			{
				return string.Format("OverlayColorMap_{0}_{1}_{2}_{3}", this.Color.A, this.Color.R, this.Color.G, this.Color.B);
			}
		}

		#endregion
	}
}