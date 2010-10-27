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
	/// Defines the interface to an update context.  An update context allows the application read
	/// data from a persistent store, and to synchronize the persistent store with changes made to that data.
	/// </summary>
	public interface IUpdateContext : IPersistenceContext
	{
		/// <summary>
		/// Gets or sets the change-set recorder that the context will use to create
		/// a record of the changes that were made.
		/// </summary>
		/// <remarks>
		/// Setting this property to null will effectively disable this auditing.
		/// </remarks>
		IEntityChangeSetRecorder ChangeSetRecorder { get; set; }

		/// <summary>
		/// Attempts to flush and commit all changes made within this update context to the persistent store.
		/// </summary>
		/// <remarks>
		/// If this operation succeeds, the state of the persistent store will be syncrhonized with the state
		/// of all domain objects that are attached to this context, and the context can continue to be used
		/// for read operations only. If the operation fails, an exception will be thrown.
		/// </remarks>
		void Commit();
	}
}
