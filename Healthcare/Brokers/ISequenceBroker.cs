#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Base broker interface for brokers that work with sequences.
	/// </summary>
	public interface ISequenceBroker : IPersistenceBroker
	{
		/// <summary>
		/// Peeks at the next number in the sequence, but does not advance the sequence.
		/// </summary>
		/// <returns></returns>
		string PeekNext();

		/// <summary>
		/// Gets the next number in the sequence, advancing the sequence by 1.
		/// </summary>
		/// <returns></returns>
		string GetNext();
	}
}
