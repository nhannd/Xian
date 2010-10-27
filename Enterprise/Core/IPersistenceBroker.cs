#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Base interface for all persistence broker interfaces.  This interface is not implemented directly.
	/// </summary>
	public interface IPersistenceBroker
	{
		/// <summary>
		/// Used by the framework to establish the <see cref="IPersistenceContext"/> in which an instance of
		/// this broker will act.
		/// </summary>
		/// <param name="context"></param>
		void SetContext(IPersistenceContext context);
	}
}
