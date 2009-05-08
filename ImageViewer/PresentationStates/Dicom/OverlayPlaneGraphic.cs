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
		[CloneIgnore]
		private GrayscaleImageGraphic _overlayGraphic;

		private readonly int _index;
		private readonly int _frameIndex;
		private readonly OverlayPlaneSource _source;
		private string _label;
		private string _description;
		private OverlaySubtype _subtype;
		private OverlayType _type;
		private ushort _grayPresentationValue = 0;
		private Color? _color;

		/// <summary>
		/// Constructs an <see cref="OverlayPlaneGraphic"/> for a single-frame overlay plane using a pre-processed overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload should only be used for single-frame overlay planes. Multi-frame overlay planes should process the overlay data
		/// into separate buffers and then construct individual graphics using <see cref="OverlayPlaneGraphic(OverlayPlane, byte[], int, OverlayPlaneSource)"/>.
		/// </para>
		/// <para>
		/// The <paramref name="overlayPixelData"/> parameter allows for the specification of an alternate source of overlay pixel data, such
		/// as the unpacked contents of <see cref="DicomTags.OverlayData"/> or the extracted, inflated overlay pixels of <see cref="DicomTags.PixelData"/>.
		/// Although the format should be 8-bits per pixel, every pixel should either be 0 or 255. This will allow pixel interpolation algorithms
		/// sufficient range to produce a pleasant image. (If the data was either 0 or 1, regardless of the bit-depth, most interpolation algorithms
		/// will interpolate 0s for everything in between!)
		/// </para>
		/// </remarks>
		/// <param name="overlayPlaneIod">The IOD object containing properties of the overlay plane.</param>
		/// <param name="overlayPixelData">The overlay pixel data in 8-bits per pixel format, with each pixel being either 0 or 255.</param>
		/// <param name="source">A value identifying the source of the overlay plane.</param>
		public OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, byte[] overlayPixelData, OverlayPlaneSource source) : this(overlayPlaneIod, overlayPixelData, 0, source) {}

		/// <summary>
		/// Constructs an <see cref="OverlayPlaneGraphic"/> for a single or multi-frame overlay plane using a pre-processed overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <paramref name="overlayPixelData"/> parameter allows for the specification of an alternate source of overlay pixel data, such
		/// as the unpacked contents of <see cref="DicomTags.OverlayData"/> or the extracted, inflated overlay pixels of <see cref="DicomTags.PixelData"/>.
		/// Although the format should be 8-bits per pixel, every pixel should either be 0 or 255. This will allow pixel interpolation algorithms
		/// sufficient range to produce a pleasant image. (If the data was either 0 or 1, regardless of the bit-depth, most interpolation algorithms
		/// will interpolate 0s for everything in between!)
		/// </para>
		/// </remarks>
		/// <param name="overlayPlaneIod">The IOD object containing properties of the overlay plane.</param>
		/// <param name="overlayPixelData">The overlay pixel data in 8-bits per pixel format, with each pixel being either 0 or 255.</param>
		/// <param name="frameIndex">The overlay frame index (0-based). Single-frame overlays should specify 0.</param>
		/// <param name="source">A value identifying the source of the overlay plane.</param>
		public OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, byte[] overlayPixelData, int frameIndex, OverlayPlaneSource source)
		{
			Platform.CheckNonNegative(frameIndex, "frameIndex");
			_frameIndex = frameIndex;
			_index = overlayPlaneIod.Index;
			_label = overlayPlaneIod.OverlayLabel;
			_description = overlayPlaneIod.OverlayDescription;
			_type = overlayPlaneIod.OverlayType;
			_subtype = overlayPlaneIod.OverlaySubtype;
			_source = source;

			GrayscaleImageGraphic overlayImageGraphic = CreateOverlayImageGraphic(overlayPlaneIod, overlayPixelData);
			if (overlayImageGraphic != null)
			{
				_overlayGraphic = overlayImageGraphic;
				this.Color = System.Drawing.Color.PeachPuff;
				base.Graphics.Add(overlayImageGraphic);
			}

			if (string.IsNullOrEmpty(overlayPlaneIod.OverlayLabel))
			{
				if (overlayPlaneIod.IsMultiFrame)
					base.Name = string.Format(SR.FormatDefaultMultiFrameOverlayGraphicName, _source, _index, frameIndex);
				else
					base.Name = string.Format(SR.FormatDefaultSingleFrameOverlayGraphicName, _source, _index, frameIndex);
			}
			else
			{
				base.Name = overlayPlaneIod.OverlayLabel;
			}
		}

		/// <summary>
		/// Constructs a new user-created <see cref="OverlayPlaneGraphic"/> with the specified dimensions.
		/// </summary>
		/// <param name="rows">The number of rows in the overlay.</param>
		/// <param name="columns">The number of columns in the overlay.</param>
		protected OverlayPlaneGraphic(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			_index = -1;
			_frameIndex = 0;
			_label = string.Empty;
			_description = string.Empty;
			_type = OverlayType.G;
			_subtype = null;
			_source = OverlayPlaneSource.User;
			_overlayGraphic = new GrayscaleImageGraphic(
				rows, columns, // the reported overlay dimensions
				8, // bits allocated is always 8
				8, // overlays always have bit depth of 1, but we upconverted the data
				7, // the high bit is now 7 after upconverting
				false, false, // overlays aren't signed and don't get inverted
				1, 0, // overlays have no rescale
				new byte[rows * columns]); // new empty pixel buffer

			this.Color = System.Drawing.Color.PeachPuff;
			base.Graphics.Add(_overlayGraphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
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

		protected byte[] OverlayPixelData
		{
			get { return _overlayGraphic.PixelData.Raw; }
		}

		private GrayscaleImageGraphic OverlayImageGraphic
		{
			get { return _overlayGraphic; }
		}

		public int Index
		{
			get { return _index; }
		}

		public int FrameIndex
		{
			get { return _frameIndex; }
		}

		public OverlayPlaneSource Source
		{
			get { return _source; }
		}

		public string Label
		{
			get { return _label; }
			protected set { _label = value; }
		}

		public string Description
		{
			get { return _description; }
			protected set { _description = value; }
		}

		public OverlayType Type
		{
			get { return _type; }
			protected set { _type = value; }
		}

		public OverlaySubtype Subtype
		{
			get { return _subtype; }
			protected set { _subtype = value; }
		}

		public PointF Origin
		{
			get { return new PointF(_overlayGraphic.SpatialTransform.TranslationX + 1, _overlayGraphic.SpatialTransform.TranslationY + 1); }
			protected set
			{
				_overlayGraphic.SpatialTransform.TranslationX = value.X - 1;
				_overlayGraphic.SpatialTransform.TranslationY = value.Y - 1;
			}
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