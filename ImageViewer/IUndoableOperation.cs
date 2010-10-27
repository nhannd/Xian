#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An interface for an operation on an object
	/// that implements undo/redo using the Memento pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Memento design pattern allows client code to perform undo/redo operations on
	/// an object without understanding any of the internals of how the operation is undone.
	/// All that is needed is a 'memento' from before and after any modifications are made
	/// to the object in question.  These mementos can then be used to construct a 
	/// single <see cref="MemorableUndoableCommand"/> to be entered into the 
	/// <see cref="CommandHistory"/> and the changes to the object's state
	/// can then be 'undone' and 'redone', simply be passing the memento back to the object.
	/// </para>
	/// <para>
	/// This interface is merely an abstraction that allows client code to apply the Memento
	/// pattern to an arbitrary object without any understanding of the operation being applied
	/// or the participating objects.
	/// </para>
	/// </remarks>
	public interface IUndoableOperation<T> where T : class
	{
		/// <summary>
		/// In the memento pattern, the 'originator' is the object whose state is being
		/// captured and restored via a memento.
		/// </summary>
		/// <remarks>
		/// In this interface definition, the originator is purposely not of <typeparamref name="T">type T</typeparamref>
		/// because you may actually want to perform the operation on an object that is not itself
		/// <see cref="IMemorable">memorable</see>, but rather on some <see cref="IMemorable">memorable</see> property.
		/// </remarks>
		IMemorable GetOriginator(T item);

		/// <summary>
		/// Gets whether or not this operation applies to the given item.
		/// </summary>
		bool AppliesTo(T item);

		/// <summary>
		/// Applies the operation to the given item.
		/// </summary>
		/// <param name="item"></param>
		void Apply(T item);
	}
}