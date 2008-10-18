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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Allows object state to be captured and restored.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="IMemorable"/> can be implemented by classes that require support
	/// for the <i>Memento</i> design pattern--<see cref="IMemorable"/> acts as the
	/// <i>Originator</i>.  Typically, the <see cref="IMemorable"/>
	/// interface is used in conjunction with <see cref="MemorableUndoableCommand"/> 
	/// to provide undo/redo support.
	/// </para>
	/// <para>
	///	It is common in the framework to use a 'begin state' and 'end state' memento
	/// that comprise an <see cref="MemorableUndoableCommand"/>.  It is also common to check
	/// the equality of the two mementos in order to decide whether or not an
	/// <see cref="MemorableUndoableCommand"/> should be added to a <see cref="CommandHistory"/>
	/// object.  Therefore, it is good practice to override and implement the
	/// <see cref="object.Equals(object)"/> method on memento classes.
	/// </para>
	/// </remarks>
	/// <seealso cref="MemorableUndoableCommand"/>
	/// <seealso cref="CommandHistory"/>
	public interface IMemorable
	{
		/// <summary>
		/// Captures the state of an object.
		/// </summary>
		/// <remarks>
		/// The implementation of <see cref="CreateMemento"/> should return an
		/// object containing enough state information so that
		/// when <see cref="SetMemento"/> is called, the object can be restored
		/// to the original state.
		/// </remarks>
		object CreateMemento();

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The object that was
		/// originally created with <see cref="CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="SetMemento"/> should return the 
		/// object to the original state captured by <see cref="CreateMemento"/>.
		/// </remarks>
		void SetMemento(object memento);
	}
}
