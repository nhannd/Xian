#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	public class EntityChangeSetPreCommitArgs
	{
		public EntityChangeSetPreCommitArgs(EntityChangeSet changeSet, IPersistenceContext context)
		{
			ChangeSet = changeSet;
			PersistenceContext = context;
		}

		/// <summary>
		/// Gets the change-set that is about to be committed.
		/// </summary>
		public EntityChangeSet ChangeSet { get; private set; }

		/// <summary>
		/// Gets the persistence context in which the change-set is about to be committed.
		/// Changes made still be made to entities in this context prior to commit.
		/// </summary>
		public IPersistenceContext PersistenceContext { get; private set; }
	}

	public class EntityChangeSetPostCommitArgs
	{
		public EntityChangeSetPostCommitArgs(EntityChangeSet changeSet)
		{
			ChangeSet = changeSet;
		}

		/// <summary>
		/// Gets the change-set that was committed.
		/// </summary>
		public EntityChangeSet ChangeSet { get; private set; }
	}


	/// <summary>
	/// Defines an interface to an object that subscribes to entity-change set notifications.
	/// </summary>
	public interface IEntityChangeSetListener
	{
		/// <summary>
		/// Occurs immediately prior to committing a change-set.
		/// </summary>
		/// <param name="args"></param>
		void PreCommit(EntityChangeSetPreCommitArgs args);

		/// <summary>
		/// Occurs immediately after committing a change-set.
		/// </summary>
		/// <param name="args"></param>
		void PostCommit(EntityChangeSetPostCommitArgs args);
	}
}
