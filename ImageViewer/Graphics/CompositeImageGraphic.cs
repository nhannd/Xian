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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="CompositeGraphic"/> whose <see cref="SpatialTransform"/>
	/// is tailored for images.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Use <see cref="CompositeImageGraphic"/> when you want to anchor graphics to
	/// an image.  Create an instance of <see cref="CompositeImageGraphic"/> using
	/// parameters (rows, columns, pixel spacing, etc.) from the <see cref="ImageGraphic"/>
	/// which you want to anchor other graphics to.  Then add the <see cref="ImageGraphic"/>
	/// to the <see cref="CompositeImageGraphic"/> along with any other graphics you want
	/// "attached" to the <see cref="ImageGraphic"/>.
	/// </para>
	/// </remarks>
	[Cloneable]
	public class CompositeImageGraphic : CompositeGraphic
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private double _pixelSpacingX;
		private double _pixelSpacingY;
		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeImageGraphic"/> with
		/// the specified rows and columns.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		public CompositeImageGraphic(
			int rows, 
			int columns) : this(rows, columns, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeImageGraphic"/> with
		/// the specified rows, columns and pixel spacing.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		public CompositeImageGraphic(
			int rows,
			int columns,
			double pixelSpacingX,
			double pixelSpacingY) : this(rows, columns, pixelSpacingX, pixelSpacingY, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeImageGraphic"/> with
		/// the specified rows, columns, pixel spacing and pixel aspect ratio.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		public CompositeImageGraphic(
				int rows,
				int columns,
				double pixelSpacingX,
				double pixelSpacingY,
				double pixelAspectRatioX,
				double pixelAspectRatioY)
		{
			_rows = rows;
			_columns = columns;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CompositeImageGraphic(CompositeImageGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// This member overrides <see cref="Graphic.CreateSpatialTransform"/>.
		/// </summary>
		/// <returns></returns>
		protected override SpatialTransform CreateSpatialTransform()
		{
			return new ImageSpatialTransform(
				this,
				_rows,
				_columns,
				_pixelSpacingX,
				_pixelSpacingY,
				_pixelAspectRatioX,
				_pixelAspectRatioY);
		}
	}
}
