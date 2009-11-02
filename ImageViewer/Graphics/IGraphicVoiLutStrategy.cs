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

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	//TODO (cr Oct 2009): factory - the old provider name wasn't right either (rename?)
	//TODO (cr Oct 2009): Should take an ImageGraphic or IImageGraphic
	/// <summary>
	/// Defines a strategy for selecing a VOI LUT appropriate for an <see cref="IGraphic"/>.
	/// </summary>
	public interface IGraphicVoiLutStrategy
	{
		/// <summary>
		/// Gets the initial VOI LUT appropriate for the <paramref name="graphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		IComposableLut GetInitialVoiLut(IGraphic graphic);
	}

	/// <summary>
	/// A base class defines a strategy for selecing a VOI LUT appropriate for an <see cref="IGraphic"/>.
	/// </summary>
	public abstract class GraphicVoiLutStrategy : IGraphicVoiLutStrategy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected GraphicVoiLutStrategy() {}

		/// <summary>
		/// Gets the initial VOI LUT appropriate for the <paramref name="graphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public abstract IComposableLut GetInitialVoiLut(IGraphic graphic);

		/// <summary>
		/// Defines the method for selecing a VOI LUT appropriate for an <see cref="IGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public delegate IComposableLut GetInitialVoiLutDelegate(IGraphic graphic);

		/// <summary>
		/// Creates a new strategy that wraps the given delegate.
		/// </summary>
		/// <param name="initialVoiLutDelegate">A <see cref="GetInitialVoiLutDelegate"/> delegate to select a VOI LUT appropriate for the given <see cref="IGraphic"/>. This method should generally be static, as the strategy may only be reference-copied when the parent graphic is cloned.</param>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public static GraphicVoiLutStrategy CreateStrategy(GetInitialVoiLutDelegate initialVoiLutDelegate)
		{
			return new DelegateGraphicVoiLutStrategy(initialVoiLutDelegate);
		}

		private class DelegateGraphicVoiLutStrategy : GraphicVoiLutStrategy
		{
			private readonly GetInitialVoiLutDelegate _initialVoiLutDelegate;

			public DelegateGraphicVoiLutStrategy(GetInitialVoiLutDelegate initialVoiLutDelegate)
			{
				Platform.CheckForNullReference(initialVoiLutDelegate, "initialVoiLutDelegate");
				_initialVoiLutDelegate = initialVoiLutDelegate;
			}

			public override IComposableLut GetInitialVoiLut(IGraphic graphic)
			{
				return _initialVoiLutDelegate(graphic);
			}
		}
	}
}