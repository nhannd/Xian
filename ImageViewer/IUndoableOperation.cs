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
	//TODO: redo documentation

	/// <summary>
	/// Models an undoable operation applied to an item of type <typeparamref name="T"/>, to
	/// be used in conjuction with the <see cref="UndoableOperationApplicator{T}"/>.
	/// </summary>
	/// <remarks>
	/// <para>This interface may be deprecated in a future release. Consider using the <see cref="CompositeUndoableCommand"/> instead.</para>
	/// <para>
	/// The item type <typeparam name="T"/> need not implement <see cref="IMemorable"/> itself,
	/// but must be able to provide the originator object (from the <see cref="GetOriginator"/> method) for
	/// the operation being performed.</para>
	/// </remarks>
	public interface IUndoableOperation<T> where T : class
	{
		/// <summary>
		/// Gets the object whose state will be captured and or restored before and/or after
		/// the operation is applied (via <see cref="Apply"/>).
		/// </summary>
		IMemorable GetOriginator(T item);

		/// <summary>
		/// Gets whether or not this operation applies to the given <paramref name="item"/>.
		/// </summary>
		bool AppliesTo(T item);

		/// <summary>
		/// Applies the operation to the given <paramref name="item"/>.
		/// </summary>
		void Apply(T item);
	}
}