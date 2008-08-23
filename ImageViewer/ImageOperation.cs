#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Abstract base class for an undoable operation performed on an <see cref="IPresentationImage"/>.
	/// </summary>
	public abstract class ImageOperation : IUndoableOperation<IPresentationImage>
	{
		/// <summary>
		/// Default protected constructor.
		/// </summary>
		protected ImageOperation()
		{
		}

		#region IUndoableOperation{T} Members

		/// <summary>
		/// Gets the object whose state is to be captured or restored.
		/// </summary>
		/// <param name="image">An <see cref="IPresentationImage"/> that contains
		/// the object whose state is to be captured or restored.</param>
		/// <remarks>
		/// <para>
		/// Typically, operations are applied to some aspect of the presentation image,
		/// such as zoom, pan, window/level, etc. That aspect will usually be 
		/// encapsulated as an object that is owned by the
		/// by <see cref="IPresentationImage"/>.  <see cref="IUndoableOperation{T}.GetOriginator"/> allows
		/// the plugin developer to define what that object is.
		/// </para>
		/// <para>
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> should not return true if <see cref="IUndoableOperation{T}.GetOriginator"/> has returned null.
		/// However, it is valid for <see cref="IUndoableOperation{T}.GetOriginator"/> to return a non-null value and <see cref="IUndoableOperation{T}.AppliesTo"/>
		/// to return false.
		/// </para>
		/// </remarks>
		/// <returns>
		/// The appropriate originator for the input <see cref="IPresentationImage"/>, or null if one doesn't exist.
		/// </returns>
		public abstract IMemorable GetOriginator(IPresentationImage image);

		/// <summary>
		/// Gets whether or not the operation is applicable for the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> should never return true if <see cref="IUndoableOperation{T}.GetOriginator"/> has returned null.
		/// </remarks>
		/// <returns>
		/// Unless overridden, returns true if <see cref="GetOriginator"/> returns a non-null value, otherwise false.
		/// </returns>
		public virtual bool AppliesTo(IPresentationImage image)
		{
			return GetOriginator(image) != null;
		}

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		public abstract void Apply(IPresentationImage image);

		#endregion
	}
}
