#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
