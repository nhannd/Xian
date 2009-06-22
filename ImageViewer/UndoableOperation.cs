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

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Abstract base class for <see cref="IUndoableOperation{T}"/>.
	/// </summary>
	public abstract class UndoableOperation<T> : IUndoableOperation<T> where T : class
	{
		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected UndoableOperation()
		{
		}

		#region IUndoableOperation<T> Members

		/// <summary>
		/// In the memento pattern, the 'originator' is the object whose state is being
		/// captured and restored via a memento.
		/// </summary>
		/// <remarks>
		/// In this interface definition, the originator is purposely not of <typeparamref name="T">type T</typeparamref>
		/// because you may actually want to perform the operation on an object that is not itself
		/// <see cref="IMemorable">memorable</see>, but rather on some <see cref="IMemorable">memorable</see> property.
		/// </remarks>
		public abstract IMemorable GetOriginator(T item);

		/// <summary>
		/// Gets whether or not this operation applies to the given item.
		/// </summary>
		/// <remarks>
		/// By default, simply returns whether or not <see cref="GetOriginator"/> returns for the given item.
		/// Subclasses can override this method to customize the behaviour.
		/// </remarks>
		public virtual bool AppliesTo(T item)
		{
			return GetOriginator(item) != null;
		}

		/// <summary>
		/// Applies the operation to the given item.
		/// </summary>
		public abstract void Apply(T item);

		#endregion
	}
}
