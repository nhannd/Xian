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

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Specifies the line style to use when drawing the vector.
	/// </summary>
	public enum LineStyle
	{
		/// <summary>
		/// A solid line.
		/// </summary>
		Solid = 0,
		/// <summary>
		/// A dashed line.
		/// </summary>
		Dash = 1,
		/// <summary>
		/// A dotted line.
		/// </summary>
		Dot = 2
	}

	/// <summary>
	/// An vector <see cref="Graphic"/>.
	/// </summary>
	[Cloneable(true)]
	public abstract class VectorGraphic : Graphic, IVectorGraphic
	{
		/// <summary>
		/// The hit test distance in destination pixels.
		/// </summary>
		public static readonly int HitTestDistance = 10;
		private Color _color = Color.Yellow;
		private LineStyle _lineStyle = LineStyle.Solid;

		/// <summary>
		/// Initializes a new instance of <see cref="VectorGraphic"/>.
		/// </summary>
		protected VectorGraphic()
		{
		}

		/// <summary>
		/// Gets or sets the colour.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _lineStyle; }
			set { _lineStyle = value; }
		}
	}
}
