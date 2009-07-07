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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// Defines a property to get or set the <see cref="PresentationStates.PresentationState"/> of an <see cref="IPresentationImage"/>.
	/// </summary>
	public interface IPresentationStateProvider
	{
		/// <summary>
		/// Gets or sets the <see cref="PresentationStates.PresentationState"/> of the image.
		/// </summary>
		PresentationState PresentationState { get; set; }
	}

	/// <summary>
	/// The base presentation state class from which all specific presentation state implementations derive.
	/// </summary>
	/// <remarks>
	/// The objects that constitute the presentation state of a given <see cref="IPresentationImage"/> are defined by the specific implementations.
	/// </remarks>
	[Cloneable(true)]
	public abstract class PresentationState
	{
		/// <summary>
		/// Constructs a new presentation state.
		/// </summary>
		protected PresentationState() {}

		/// <summary>
		/// Serializes the presentation state of the image to the current state object.
		/// </summary>
		/// <param name="image">The image whose presentation state is to be serialized.</param>
		public virtual void Serialize(IPresentationImage image)
		{
			this.Serialize(ToEnumerable(image));
		}

		/// <summary>
		/// Serializes the presentation state of the given images to the current state object.
		/// </summary>
		/// <param name="images">The images whose presentation states are to be serialized.</param>
		public abstract void Serialize(IEnumerable<IPresentationImage> images);

		/// <summary>
		/// Deserializes the presentation state from the current state object into the given image.
		/// </summary>
		/// <param name="image">The image to which the presentation state is to be deserialized.</param>
		public virtual void Deserialize(IPresentationImage image)
		{
			this.Deserialize(ToEnumerable(image));
		}

		/// <summary>
		/// Deserializes the presentation state from the current state object into the given images.
		/// </summary>
		/// <param name="images">The images to which the presentation state is to be deserialized.</param>
		public abstract void Deserialize(IEnumerable<IPresentationImage> images);

		/// <summary>
		/// Clears the presentation state of the given image.
		/// </summary>
		/// <remarks>
		/// Whether all presentation state concepts defined by the implementation are cleared, or only the
		/// objects actually defined by this particular state object are cleared, is up to the implementation.
		/// </remarks>
		/// <param name="image">The image whose presentation state is to be cleared.</param>
		public virtual void Clear(IPresentationImage image)
		{
			this.Clear(ToEnumerable(image));
		}

		/// <summary>
		/// Clears the presentation states of the given images.
		/// </summary>
		/// <remarks>
		/// Whether all presentation state concepts defined by the implementation are cleared, or only the
		/// objects actually defined by this particular state object are cleared, is up to the implementation.
		/// </remarks>
		/// <param name="image">The images whose presentation states are to be cleared.</param>
		public abstract void Clear(IEnumerable<IPresentationImage> image);

		private static IEnumerable<IPresentationImage> ToEnumerable(IPresentationImage image)
		{
			yield return image;
		}
	}
}