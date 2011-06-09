#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Defines the interface to an object that is serializable as part of a hanging protocol.
	/// </summary>
	public interface IHpSerializableElement
	{
		/// <summary>
		/// Gets the state to be saved in the HP.
		/// </summary>
		/// <returns>An instance of a data-contract class - see <see cref="HpDataContractAttribute"/></returns>
		object GetState();

		/// <summary>
		/// Populates this element's state from a saved HP.
		/// </summary>
		/// <param name="state">An instance of a data-contract class - see <see cref="HpDataContractAttribute"/></param>
		void SetState(object state);
	}
}
