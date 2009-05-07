#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// This implementation does not support overlays embedded in the unused bits of the
	/// <see cref="DicomTags.PixelData"/> attribute, a retired usage from previous versions
	/// of the DICOM Standard. 
	/// </remarks>
	[Cloneable(true)]
	public class OverlayPlaneGraphic : CompositeGraphic, IShutterGraphic
	{
		private GrayscaleImageGraphic _overlayGraphic;

		private readonly int _index;
		private readonly int _frameNumber;
		private readonly string _label;
		private readonly string _description;
		private readonly string _subtype;
		private readonly OverlayType _type;
		private readonly OverlayPlaneSource _source;
		private ushort _grayPresentationValue = 0;
		private Color? _color;

		internal OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, byte[] overlayPixelData, OverlayPlaneSource source) : this(overlayPlaneIod, overlayPixelData, 0, source) {}

		internal OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, byte[] overlayPixelData, int frameNumber, OverlayPlaneSource source)
		{
			_frameNumber = frameNumber;
			_index = overlayPlaneIod.Index;
			_label = overlayPlaneIod.OverlayLabel;
			_description = overlayPlaneIod.OverlayDescription;
			_type = overlayPlaneIod.OverlayType;
			_source = source;

			GrayscaleImageGraphic overlayImageGraphic = CreateOverlayImageGraphic(overlayPlaneIod, overlayPixelData);
			if (overlayImageGraphic != null)
			{
				_overlayGraphic = overlayImageGraphic;
				Color = System.Drawing.Color.PeachPuff;
				base.Graphics.Add(overlayImageGraphic);
			}

			if (string.IsNullOrEmpty(overlayPlaneIod.OverlayLabel))
			{
				if (overlayPlaneIod.IsMultiFrame)
					base.Name = string.Format("Overlay Plane ({2} #{0}, Fr #{1})", _index, frameNumber, _source);
				else
					base.Name = string.Format("Overlay Plane ({2} #{0})", _index, -1, _source);
			}
			else
			{
				base.Name = overlayPlaneIod.OverlayLabel;
			}
		}

		//for cloning; all the underlying graphics are already cloneable.
		private OverlayPlaneGraphic() {}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_overlayGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                              delegate(IGraphic graphic) { return graphic is GrayscaleImageGraphic; }) as GrayscaleImageGraphic;
		}

		private static GrayscaleImageGraphic CreateOverlayImageGraphic(OverlayPlane overlayPlaneIod, byte[] overlayData)
		{
			Point origin = (overlayPlaneIod.OverlayOrigin ?? new Point(1, 1)) - new Size(1, 1);
			int rows = overlayPlaneIod.OverlayRows;
			int cols = overlayPlaneIod.OverlayColumns;

			if (overlayData == null || overlayData.Length == 0)
				throw new Exception("Overlay plane data is invalid.");

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(
				rows, cols, // the reported overlay dimensions
				8, // bits allocated is always 8
				8, // overlays always have bit depth of 1, but we upconverted the data
				7, // the high bit is now 7 after upconverting
				false, false, // overlays aren't signed and don't get inverted
				1, 0, // overlays have no rescale
				overlayData); // the upconverted overlay data

			imageGraphic.SpatialTransform.TranslationX = origin.X;
			imageGraphic.SpatialTransform.TranslationY = origin.Y;

			return imageGraphic;
		}

		private void UpdateLuts()
		{
			// NOTE: this determination is actually supposed to be based on the client display device
			if (_color == null || _color.Value.IsEmpty)
			{
				_overlayGraphic.VoiLutManager.InstallLut(new OverlayVoiLut(_grayPresentationValue, 65535));
				//Install a color map with the first value being transparent.
				_overlayGraphic.ColorMapManager.InstallColorMap(new GrayscaleColorMap(_grayPresentationValue));
			}
			else
			{
				//The color makes the gray p-value irrelevant, so do this to save space.
				_overlayGraphic.VoiLutManager.InstallLut(new OverlayVoiLut(255, 255));
				_overlayGraphic.ColorMapManager.InstallColorMap(new OverlayColorMap(_color.Value));
			}
		}

		protected GrayscaleImageGraphic OverlayImageGraphic
		{
			get { return _overlayGraphic; }
		}

		public int Index
		{
			get { return _index; }
		}

		public OverlayPlaneSource Source
		{
			get { return _source; }
		}

		public string Label
		{
			get { return _label; }
		}

		public string Description
		{
			get { return _description; }
		}

		public OverlayType Type
		{
			get { return _type; }
		}

		public string SubType
		{
			get { return _subtype; }
		}

		public PointF Origin
		{
			get { return new PointF(_overlayGraphic.SpatialTransform.TranslationX, _overlayGraphic.SpatialTransform.TranslationY); }
		}

		public int Rows
		{
			get { return _overlayGraphic.Rows; }
		}

		public int Columns
		{
			get { return _overlayGraphic.Columns; }
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

		public OverlayData CreateOverlayData(bool bigEndianWords)
		{
			return OverlayData.FromPixelData(bigEndianWords, _overlayGraphic.PixelData);
		}

		#region IShutterGraphic Members

		ushort IShutterGraphic.PresentationValue
		{
			get { return this.GrayPresentationValue; }
			set { this.GrayPresentationValue = value; }
		}

		Color IShutterGraphic.PresentationColor
		{
			get { return this.Color ?? System.Drawing.Color.Empty; }
			set { this.Color = value; }
		}

		#endregion

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
			private OverlayVoiLut() {}

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
				protected set { }
			}

			public override int MaxOutputValue
			{
				get { return _maxOutputValue; }
				protected set { }
			}

			public override int this[int index]
			{
				get
				{
					//if (index <= 0)
					//    return 0;
					//return _presentationValue;
					return (int) ((index/(float) _maxInputValue)*_presentationValue);
				}
				protected set { throw new InvalidOperationException("This lut is not editable."); }
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
			private readonly ushort _presentationValue;

			public GrayscaleColorMap(ushort presentationValue)
			{
				_presentationValue = presentationValue;
			}

			/// <summary>
			/// Cloning constructor
			/// </summary>
			private GrayscaleColorMap() {}

			protected override void Create()
			{
				Color color;

				int j = 0;
				float maxGrayLevel = this.Length - 1;
				float alphaRange = _presentationValue - this.MinInputValue;

				for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
				{
					float scale = j/maxGrayLevel;
					float alphaScale = Math.Min(1f, j/alphaRange);
					j++;

					int value = (int) (byte.MaxValue*scale);
					int alpha = (int) (byte.MaxValue*alphaScale);
					color = System.Drawing.Color.FromArgb(alpha, value, value, value);
					this[i] = color.ToArgb();
				}
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
				for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
					this[i] = Color.FromArgb(i, _color).ToArgb();
			}

			public override string GetDescription()
			{
				return string.Format("OverlayColorMap_{0}_{1}_{2}_{3}", this.Color.A, this.Color.R, this.Color.G, this.Color.B);
			}
		}

		#endregion
	}

	/// <summary>
	/// Enumeration to indicate the source of an <see cref="OverlayPlaneGraphic"/>.
	/// </summary>
	public enum OverlayPlaneSource
	{
		/// <summary>
		/// Indicates that the associated <see cref="OverlayPlaneGraphic"/> was defined in the image SOP or the image SOP referenced by the presentation state SOP.
		/// </summary>
		Image,

		/// <summary>
		/// Indicates that the associated <see cref="OverlayPlaneGraphic"/> was defined in the presentation state SOP.
		/// </summary>
		PresentationState,

		/// <summary>
		/// Indicates that the associated <see cref="OverlayPlaneGraphic"/> was user-created.
		/// </summary>
		User
	}
}