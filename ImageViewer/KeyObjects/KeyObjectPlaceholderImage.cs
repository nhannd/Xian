#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	[Cloneable]
	public class KeyObjectPlaceholderImage : GrayscalePresentationImage
	{
		private readonly string _reason;

		public KeyObjectPlaceholderImage(string reason) : base(5, 5)
		{
			InvariantTextPrimitive textGraphic = new InvariantTextPrimitive(_reason = reason);
			textGraphic.Color = Color.WhiteSmoke;
			base.ApplicationGraphics.Add(textGraphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected KeyObjectPlaceholderImage(KeyObjectPlaceholderImage source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override void OnDrawing()
		{
			// upon drawing, re-centre the text
			RectangleF bounds = base.ClientRectangle;
			PointF anchor = new PointF(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);
			InvariantTextPrimitive textGraphic = (InvariantTextPrimitive) CollectionUtils.SelectFirst(base.ApplicationGraphics, IsType<InvariantTextPrimitive>);
			textGraphic.CoordinateSystem = CoordinateSystem.Destination;
			textGraphic.Location = anchor;
			textGraphic.ResetCoordinateSystem();
			base.OnDrawing();
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new KeyObjectPlaceholderImage(_reason);
		}

		private static bool IsType<T>(object test)
		{
			return test is T;
		}
	}
}