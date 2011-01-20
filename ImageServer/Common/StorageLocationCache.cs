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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// A cache of <see cref="StudyStorageLocation"/> objects.
	/// </summary>
	public class StorageLocationCache : IDisposable
	{
		#region Private Members
		private readonly Dictionary<ServerEntityKey, ServerCache<string, StudyStorageLocation>> _caches = new Dictionary<ServerEntityKey, ServerCache<string, StudyStorageLocation>>();
		private readonly object _lock = new object();
		#endregion

		#region Public Methods
		public StudyStorageLocation GetCachedStudy(ServerEntityKey partitionKey, string studyUid)
		{
			ServerCache<string,StudyStorageLocation> partitionCache;
			lock (_lock)
			{
				if (!_caches.TryGetValue(partitionKey, out partitionCache))
					return null;
			}

			return partitionCache.GetValue(studyUid);
		}

		public void AddCachedStudy(StudyStorageLocation theLocation)
		{
			ServerCache<string, StudyStorageLocation> partitionCache;
			lock (_lock)
			{
				if (!_caches.TryGetValue(theLocation.ServerPartitionKey, out partitionCache))
				{
					partitionCache = new ServerCache<string, StudyStorageLocation>(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
					_caches.Add(theLocation.ServerPartitionKey,partitionCache);
				}
			}

			partitionCache.Add(theLocation.StudyInstanceUid, theLocation);
		}
		#endregion

		#region IDisposable Implementation
		public void Dispose()
		{
			foreach (ServerCache<string, StudyStorageLocation> partitionCache in _caches.Values)
				partitionCache.Dispose();
		}
		#endregion
	}
}
