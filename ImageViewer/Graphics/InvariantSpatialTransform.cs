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

using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Implements a <see cref="SpatialTransform"/> which is invariant in the destination coordinate system with respect to scale, flip and rotation.
	/// </summary>
	[Cloneable]
	public sealed class InvariantSpatialTransform : SpatialTransform
	{
		/// <summary>
		/// Initializes a new <see cref="InvariantSpatialTransform"/>.
		/// </summary>
		/// <param name="ownerGraphic">The graphic for which this <see cref="InvariantSpatialTransform"/> is being constructed.</param>
		public InvariantSpatialTransform(IGraphic ownerGraphic) : base(ownerGraphic) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private InvariantSpatialTransform(InvariantSpatialTransform source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override void CalculatePostTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Reset();
			cumulativeTransform.Translate(this.TranslationX, this.TranslationY);
		}

		protected override void CalculatePreTransform(Matrix cumulativeTransform) {}
	}
}