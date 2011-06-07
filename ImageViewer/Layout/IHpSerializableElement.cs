#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Defines the interface to an object that is serializable as part of a hanging protocol.
	/// </summary>
	public interface IHpSerializableElement
	{
		/// <summary>
		/// Gets the class of the data-contract used to persist this element's state.
		/// </summary>
		Type StateDataContract { get; }

		/// <summary>
		/// Gets the state to be saved in the HP.
		/// The returned object is of the type returned by the <see cref="StateDataContract"/> property.
		/// </summary>
		/// <returns></returns>
		object GetState();

		/// <summary>
		/// Populates this element's state from a saved HP.  The <paramref name="state"/> object will be an
		/// instance of the type returned by the <see cref="StateDataContract"/> property.
		/// </summary>
		/// <param name="state"></param>
		void SetState(object state);
	}
}
