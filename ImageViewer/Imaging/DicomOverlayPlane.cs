using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using OverlayPlaneIod=ClearCanvas.Dicom.Iod.Modules.OverlayPlane;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class DicomOverlayPlane
	{
		private readonly OverlayPlaneIod _dicomIod;
		private readonly string _name;

		private Color _color = Color.Gray;
		private OverlayColorMap _colorMap;
		private GrayscaleImageGraphic _overlay;

		internal DicomOverlayPlane(OverlayPlaneIod dicomIod)
		{
			_dicomIod = dicomIod;

			int index = (int)_dicomIod.TagOffset / 0x10000;

			_color = GetDefaultColor(index);

			if (string.IsNullOrEmpty(_dicomIod.OverlayLabel))
				_name = string.Format("Overlay Plane #{0}", index + 1);
			else
				_name = _dicomIod.OverlayLabel;
		}

		private static Color GetDefaultColor(int index)
		{
			// TODO perhaps implement some mechanism for configurable colours
			return Color.PeachPuff;
		}

		public string Name
		{
			get { return _name; }
		}

		public string Description
		{
			get { return _dicomIod.OverlayDescription ?? string.Empty; }
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					this.OnColorChanged();
				}
			}
		}

		public bool Visible
		{
			get
			{
				if (_overlay == null)
					return false;
				return _overlay.Visible;
			}
			set
			{
				if (_overlay != null && _overlay.Visible != value)
					_overlay.Visible = value;
			}
		}

		public GrayscaleImageGraphic Graphic
		{
			get
			{
				if (_overlay == null)
				{
					Point origin = (_dicomIod.OverlayOrigin ?? new Point(1, 1)) - new Size(1, 1);
					byte[] overlayData = _dicomIod.OverlayData;
					int rows = _dicomIod.OverlayRows;
					int cols = _dicomIod.OverlayColumns;
					int bitsAllocated = _dicomIod.OverlayBitsAllocated;
					int highBit = _dicomIod.OverlayBitPosition;

					if (overlayData == null || overlayData.Length == 0)
					{
						// data is stored in unused bits of pixel data
						overlayData = _dicomIod.DicomAttributeProvider[DicomTags.PixelData].Values as byte[];
					}
					else
					{
						// data is stored in overlay data
						overlayData = new OverlayData(rows, cols, _dicomIod.IsBigEndianOW, overlayData).ToPixelData().Raw;
						bitsAllocated = 8;
						highBit = 1;
					}

					_overlay = new GrayscaleImageGraphic(
						rows, cols, // the reported overlay dimensions
						bitsAllocated, // the reported bits allocated
						1, // overlays always have bit depth of 1
						highBit, // the reported bit position
						false, false, // overlays aren't signed and don't get inverted
						1, 0, // overlays have no rescale
						overlayData); // the overlay data, or the pixel data if overlay data doesn't exist

					_overlay.VoiLutManager.InstallLut(new MinMaxPixelCalculatedLinearLut(_overlay.PixelData));
					_overlay.ColorMapManager.InstallColorMap(_colorMap = new OverlayColorMap(_color));
					_overlay.Name = this.Name;

					_overlay.SpatialTransform.TranslationX = origin.X;
					_overlay.SpatialTransform.TranslationY = origin.Y;
				}
				return _overlay;
			}
		}

		public void DrawGraphic()
		{
			if (_overlay == null)
				return;
			_overlay.Draw();
		}

		protected virtual void OnColorChanged()
		{
			if (_colorMap != null)
			{
				_colorMap.Color = this.Color;
			}
		}

		#region Color Map

		/// <summary>
		/// Simple 2-value color map for DICOM overlays.
		/// </summary>
		private class OverlayColorMap : ColorMap
		{
			private Color _color = Color.Gray;

			public OverlayColorMap() {}

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
				this.MinInputValue = 0;
				this.MaxInputValue = 1;
				this[0] = Color.FromArgb(0, _color).ToArgb();
				this[1] = Color.FromArgb(255, _color).ToArgb();
			}

			public override string GetDescription()
			{
				return "Overlay";
			}
		}

		#endregion
	}
}