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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Web.Services
{
	public abstract partial class Application
	{
		private class Cache
		{
		    private const int CheckIntervalInSeconds = 30;
            private const int ApplicationShutdownDelayInSeconds = 10;

			public static readonly Cache Instance;

			static Cache()
			{
				Instance = new Cache();
			}

			private readonly object _syncLock = new object();
			private readonly Dictionary<Guid, Application> _applications = new Dictionary<Guid, Application>();
            private Dictionary<Guid, DateTime> _appsToBeRemoved = new Dictionary<Guid, DateTime>();
		    
            private Timer _cleanupTimer;


            private Cache()
            {
                // TODO: Cleanup ther timer?
                _cleanupTimer = new Timer(OnCleanupTimerCallback, null, TimeSpan.FromSeconds(CheckIntervalInSeconds),
                                          TimeSpan.FromSeconds(CheckIntervalInSeconds));
                
            }

            private void OnCleanupTimerCallback(object ignore)
            {
                try
                {
                    if (_appsToBeRemoved.Count > 0)
                    {
                        
                        List<Guid> removalList = new List<Guid>();
                        foreach (Guid appId in _appsToBeRemoved.Keys)
                        {
                            if (DateTime.Now - _appsToBeRemoved[appId] > TimeSpan.FromSeconds(ApplicationShutdownDelayInSeconds))
                            {
                                removalList.Add(appId);
                            }
                        }

                        if (removalList.Count>0)
                        {
                            foreach(Guid appId in removalList)
                            {
                                Application app = null;
                                try
                                {
                                    lock (_syncLock)
                                    {
                                        app = _applications[appId];
                                        _applications.Remove(appId);
                                    }
                                    _appsToBeRemoved.Remove(appId);
                                }
                                finally
                                {
                                    if (app != null)
                                    {
                                        app.DisposeMembers();
                                    }
                                    Platform.Log(LogLevel.Info, "Application {0} removed from cache.", appId);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

		    public void Add(Application application)
			{
				lock (_syncLock)
				{
					_applications.Add(application.Identifier, application);
                    application.Stopped += delegate { Remove(application.Identifier); };
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

					// NOTE: For non-duplex binding we can't remove the app right away
					// because the client still hasn't received the last event. If the app 
					// is removed from the cache here, the client won't be able to poll
					// the remaining messages beause the app id is no longer valid.
					// 
					// App shutdown must be delayed to give the client some time to poll the remaining events.
					//
                    _appsToBeRemoved.Add(applicationId, DateTime.Now);
                    
					// _applications.Remove(applicationId);
					// Platform.Log(LogLevel.Debug, "Application {0} removed from cache.", applicationId);
				}
			}

			public void StopAndClearAll(string message)
			{
				lock (_syncLock)
				{
					foreach(Application app in _applications.Values)
					{
                        app.Stop(message);
					    app.DisposeMembers();
					}

					_applications.Clear();
				}
			}
		}
	}
}
