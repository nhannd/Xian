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
	/// Defines the interface to a read-context.  A read-context allows the application to perform read-only
	/// operations on a persistent store.
	/// </summary>
	public interface IReadContext : IPersistenceContext
	{
	}
}
