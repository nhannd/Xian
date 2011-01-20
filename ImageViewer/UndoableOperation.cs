#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
