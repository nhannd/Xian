#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Defines an extension point for extensions that listen for entity change-sets.
	/// </summary>
	[ExtensionPoint]
	public class EntityChangeSetListenerExtensionPoint : ExtensionPoint<IEntityChangeSetListener>
	{
	}

	/// <summary>
	/// Publishes entity change-sets to listeners.
	/// </summary>
	public class EntityChangeSetPublisher
	{
		private readonly List<IEntityChangeSetListener> _listeners;

		/// <summary>
		/// Constructor
		/// </summary>
		public EntityChangeSetPublisher()
			: this(LoadExtensions())
		{
		}

		public EntityChangeSetPublisher(IEnumerable<IEntityChangeSetListener> listeners)
		{
			_listeners = new List<IEntityChangeSetListener>(listeners);
		}

		/// <summary>
		/// Publishes the pre-commit notification.
		/// </summary>
		/// <param name="args"></param>
		public void PreCommit(EntityChangeSetPreCommitArgs args)
		{
			foreach (var listener in _listeners)
			{
				// do not catch exceptions thrown by PreCommit listeners
				// if any listener throws an exception, the commit should be aborted
				listener.PreCommit(args);
			}
		}

		/// <summary>
		/// Publishes the post-commit notification.
		/// </summary>
		/// <param name="args"></param>
		public void PostCommit(EntityChangeSetPostCommitArgs args)
		{
			foreach (var listener in _listeners)
			{
				try
				{
					// might as well catch and log any exceptions thrown by PostCommit listeners,
					// because it is too late to abort the Commit
					listener.PostCommit(args);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		private static IEnumerable<IEntityChangeSetListener> LoadExtensions()
		{
			return new TypeSafeEnumerableWrapper<IEntityChangeSetListener>(
				new EntityChangeSetListenerExtensionPoint().CreateExtensions());
		}
	}
}
