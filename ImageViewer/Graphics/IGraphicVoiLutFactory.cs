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
	/// <summary>
	/// Defines a factory for getting a VOI LUT appropriate for an <see cref="ImageGraphic"/>.
	/// </summary>
	public interface IGraphicVoiLutFactory
	{
		/// <summary>
		/// Creates a Voi LUT suitable for the given <paramref name="imageGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		IComposableLut CreateVoiLut(ImageGraphic imageGraphic);
	}

	/// <summary>
	/// A base class defines a factory for getting a VOI LUT appropriate for an <see cref="ImageGraphic"/>.
	/// </summary>
	public abstract class GraphicVoiLutFactory : IGraphicVoiLutFactory
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected GraphicVoiLutFactory() {}

		/// <summary>
		/// Creates a Voi LUT suitable for the given <paramref name="imageGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public abstract IComposableLut CreateVoiLut(ImageGraphic imageGraphic);

		/// <summary>
		/// Defines the method for creating a Voi LUT suitable for the given <paramref name="imageGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public delegate IComposableLut CreateVoiLutDelegate(ImageGraphic imageGraphic);

		/// <summary>
		/// Creates a new factory that wraps the given delegate.
		/// </summary>
		/// <param name="createVoiLutDelegate">A <see cref="CreateVoiLutDelegate"/> delegate to
		/// get a VOI LUT appropriate for the given <see cref="ImageGraphic"/>.
		/// This method should generally be static, as the factory may only be reference-copied when the parent graphic is cloned.</param>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public static GraphicVoiLutFactory Create(CreateVoiLutDelegate createVoiLutDelegate)
		{
			return new DelegateGraphicVoiLutFactory(createVoiLutDelegate);
		}

		private class DelegateGraphicVoiLutFactory : GraphicVoiLutFactory
		{
			private readonly CreateVoiLutDelegate _createVoiLutDelegate;

			public DelegateGraphicVoiLutFactory(CreateVoiLutDelegate createVoiLutDelegate)
			{
				Platform.CheckForNullReference(createVoiLutDelegate, "createVoiLutDelegate");
				_createVoiLutDelegate = createVoiLutDelegate;
			}

			public override IComposableLut CreateVoiLut(ImageGraphic imageGraphic)
			{
				return _createVoiLutDelegate(imageGraphic);
			}
		}
	}
}