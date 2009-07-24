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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A colour <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class ColorPresentationImage
		: BasicPresentationImage,
		IVoiLutProvider
	{
		#region Private fields

		// We only bother having these private fields so
		// we can easily implement CreateFreshCopy()
		private int _rows;
		private int _columns;
		private double _pixelSpacingX;
		private double _pixelSpacingY;
		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;

		[CloneCopyReference]
		private PixelDataGetter _pixelDataGetter;

		private int _constructor;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ColorPresentationImage"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// This simple constructor will automatically create RGB pixel data with the specified
		/// number of rows and columns.
		/// </remarks>
		public ColorPresentationImage(int rows, int columns) 
			: base(new ColorImageGraphic(rows, columns))
		{
			_constructor = 0;
			_rows = rows;
			_columns = columns;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorPresentationImage"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// This more flexible constructor allows for the pixel data
		/// to be retrieved from and external source via a <see cref="PixelDataGetter"/>.
		/// </remarks>
		public ColorPresentationImage(
			int rows,
			int columns,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY,
			PixelDataGetter pixelDataGetter)
			: base(new ColorImageGraphic(rows, columns, pixelDataGetter),
			       pixelSpacingX,
			       pixelSpacingY,
			       pixelAspectRatioX,
			       pixelAspectRatioY)
		{
			_constructor = 1;
			_rows = rows;
			_columns = columns;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
			_pixelDataGetter = pixelDataGetter;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ColorPresentationImage(ColorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="ColorPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="ColorPresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			if (_constructor == 0)
				return new ColorPresentationImage(_rows, _columns);
			else
				return new ColorPresentationImage(
					_rows,
					_columns,
					_pixelSpacingX,
					_pixelSpacingY,
					_pixelAspectRatioX,
					_pixelAspectRatioY,
					_pixelDataGetter);
		}

		/// <summary>
		/// Gets this image's <see cref="ColorImageGraphic"/>.
		/// </summary>
		public new ColorImageGraphic ImageGraphic
		{
			get { return (ColorImageGraphic)base.ImageGraphic;  }	
		}

		/// <summary>
		/// Gets or sets a value indicating whether VOI LUTs should be used in rendering this image.
		/// </summary>
		public bool VoiLutsEnabled
		{
			get { return this.VoiLutManager.Enabled; }
			set { this.VoiLutManager.Enabled = value; }
		}

		#region IVoiLutProvider Members

		/// <summary>
		/// Gets this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get { return this.ImageGraphic.VoiLutManager; }
		}

		#endregion
	}
}
