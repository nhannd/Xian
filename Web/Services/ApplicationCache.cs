#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Web.Services
{
	public abstract partial class Application
	{
		private class Cache
		{
			public static readonly Cache Instance;

			static Cache()
			{
				Instance = new Cache();
			}

			private readonly object _syncLock = new object();
			private readonly Dictionary<Guid, Application> _applications = new Dictionary<Guid, Application>();

			public void Add(Application application)
			{
				lock (_syncLock)
				{
					_applications.Add(application.Identifier, application);
				}
			}

			public Application Find(Guid applicationId)
			{
				lock (_syncLock)
				{
					Application application;
					return _applications.TryGetValue(applicationId, out application) ? application : null;
				}
			}

			public void Remove(Guid applicationId)
			{
				lock (_syncLock)
				{
					if (!_applications.ContainsKey(applicationId))
						return;

					_applications.Remove(applicationId);
					Platform.Log(LogLevel.Debug, "Application {0} removed from cache.", applicationId);
				}
			}

			public void Clear(out List<Application> applications)
			{
				lock (_syncLock)
				{
					applications = new List<Application>(_applications.Values);
					_applications.Clear();
				}
			}
		}
	}
}
