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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are ARGB.
	/// </summary>
	/// <remarks>
	/// A typical usage of this class is overlaying
	/// colour regions on a grayscale image to highlight areas of interest.
	/// Note that you can control not just the colour, but also the 
	/// opacity (i.e. alpha) of each pixel.
	/// </remarks>
	[Cloneable]
	public class ColorImageGraphic
		: ImageGraphic,
		IVoiLutProvider
	{
		/// <summary>
		/// We only support VOI LUTs right now, but theoretically we
		/// can implement ICC Profiles with an "ICC Transform LUT"
		/// after the VOI.
		/// </summary>
		private enum Luts
		{
			/// <summary>
			/// This LUT is initially a <see cref="NeutralColorLinearLut"/>,
			/// but may be replaced by W/L tools with something else, such as
			/// <see cref="BasicVoiLutLinear"/>
			/// </summary>
			Voi = 0
		}

		#region Private Fields

		private bool _invert;

		private LutComposer _lutComposer;
		private IVoiLutManager _voiLutManager;

		#endregion

		#region Public constructors

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// Creates an empty colour image of a specific size.
		/// By default, all pixels are set to ARGB=(0,0,0,0) (i.e.,
		/// transparent). 
		/// </remarks>
		public ColorImageGraphic(int rows, int columns)
			: base(rows, columns, 32)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData">The pixel data in ARGB format.</param>
		/// <remarks>
		/// Creates a colour image using existing pixel data.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ColorImageGraphic(ColorImageGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);

			if (source.LutComposer.LutCollection.Count > (int) Luts.Voi) //clone the voi lut.
				this.InstallVoiLut(source.VoiLut.Clone());
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering this graphic.
		/// </summary>
		public bool VoiLutsEnabled
		{
			get { return this.VoiLutManager.Enabled; }
			set { this.VoiLutManager.Enabled = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the image should be inverted.
		/// </summary>
		/// <remarks>
		/// Inversion is equivalent to polarity.
		/// </remarks>
		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new ColorPixelData PixelData
		{
			get
			{
				return base.PixelData as ColorPixelData;
			}
		}

		#region IVoiLutProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get
			{
				if (_voiLutManager == null)
					_voiLutManager = new ColorVoiLutManager(this);

				return _voiLutManager;
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets the LUT used to select and rescale values of interest in the color pixel data <b>on a per-channel basis</b>.
		/// </summary>
		public IComposableLut VoiLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer.LutCollection[(int) Luts.Voi];
			}
		}

		/// <summary>
		/// The output lut composed of all LUTs in the pipeline.
		/// </summary>
		public IComposedLut OutputLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer;
			}
		}

		#endregion

		#region Private properties

		private LutComposer LutComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LutComposer(8, false);

				return _lutComposer;
			}
		}

		#endregion

		#region Disposal

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lutComposer != null)
					_lutComposer.Dispose();
			}
		}

		#endregion

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataRaw);
			else
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataGetter);
		}

		#region Private methods

		private void InitializeNecessaryLuts(Luts luts)
		{
			if (luts >= Luts.Voi && LutComposer.LutCollection.Count == (int) Luts.Voi)
			{
				IComposableLut lut = InitialVoiLutProvider.Instance.GetLut(this.ParentPresentationImage);

				if (lut == null)
					lut = new NeutralColorLinearLut();

				InstallVoiLut(lut);
			}
		}

		#endregion

		#region Internal Properties / Methods

		internal void InstallVoiLut(IComposableLut voiLut)
		{
			Platform.CheckForNullReference(voiLut, "voiLut");

			if (this.LutComposer.LutCollection.Count == (int) Luts.Voi)
			{
				this.LutComposer.LutCollection.Add(voiLut);
			}
			else
			{
				this.LutComposer.LutCollection[(int) Luts.Voi] = voiLut;
			}
		}

		#endregion
	}
}
