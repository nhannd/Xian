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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Threading;

namespace ClearCanvas.ImageViewer
{
	public partial class ImageViewerComponent
	{
		internal class SingleStudyLoader : IDisposable
		{
			private readonly SynchronizationContext _uiThreadContext;
			private readonly ImageViewerComponent _viewer;
			private readonly LoadStudyArgs _args;
			private readonly StudyItem _studyItem;

		    private IStudyLoader _studyLoader;

			private readonly object _syncLock = new object();
			private List<Sop> _sops;
			private bool _disposed;

			public SingleStudyLoader(SynchronizationContext uiThreadContext, ImageViewerComponent viewer, LoadStudyArgs args)
				: this(uiThreadContext, viewer)
			{
				_args = args;
			}

			public SingleStudyLoader(SynchronizationContext uiThreadContext, ImageViewerComponent viewer, StudyItem studyItem)
				: this(uiThreadContext, viewer)
			{
				_studyItem = studyItem;
			}

			private SingleStudyLoader(SynchronizationContext uiThreadContext, ImageViewerComponent viewer)
			{
				Platform.CheckForNullReference(uiThreadContext, "uiThreadContext");

				IsValidPrior = false;
				_uiThreadContext = uiThreadContext;
				_viewer = viewer;
				LoadOnlineOnly = false;
			}

			private bool IsRunningOnUiThread
			{
				get { return _uiThreadContext == SynchronizationContext.Current; }
			}

			private bool IsStudyInStudyTree
			{
				get { return _viewer.StudyTree.GetStudy(StudyInstanceUid) != null; }
			}

			public bool LoadOnlineOnly { get; set; }

			public StudyItem StudyItem
			{
				get { return _studyItem; }	
			}

			public string StudyInstanceUid
			{
				get
				{
					if (_args != null)
						return _args.StudyInstanceUid;
					else
						return _studyItem.StudyInstanceUid;
				}
			}

			public IDicomServiceNode Server
			{
				get
				{
					if (_args != null)
						return _args.Server;
					else
						return _studyItem.Server;
				}
			}

			public bool IsValidPrior { get; private set; }

			public Exception Error { get; private set; }

			public void LoadStudy()
			{
				try
				{
					List<Sop> sops = LoadSops();
					lock (_syncLock)
					{
						if (_disposed)
						{
							DisposeSops(sops);
							return;
						}

						_sops = sops;
					}

					OnSopsLoaded();
				}
				catch (Exception e)
				{
					OnLoadPriorStudyFailed(e);
				}
			}

			private List<Sop> LoadSops()
			{
                _studyLoader = Server.GetService<IStudyLoader>();
                
                var args = new StudyLoaderArgs(StudyInstanceUid, Server);
				int total;
				var sops = new List<Sop>();
				
				try
				{
					if (LoadOnlineOnly && StudyItem != null)
					{
					    // TODO (CR Apr 2012): try to get rid of this.
						//This stinks, but we pre-emptively throw the offline/nearline exception
						//to avoid trying to load a prior when we know it's not online.
						switch (StudyItem.InstanceAvailability)
						{
							case "OFFLINE":
								throw new OfflineLoadStudyException(StudyInstanceUid);
							case "NEARLINE":
								throw new NearlineLoadStudyException(StudyInstanceUid);
							default:
								break;
						}
					}

                    total = _studyLoader.Start(args);
					if (total <= 0)
						throw new NotFoundLoadStudyException(args.StudyInstanceUid);
				}
				catch (LoadStudyException)
				{
					throw;
				}
				catch (Exception e)
				{
					throw new LoadStudyException(args.StudyInstanceUid, e);
				}

				try
				{
					while (true)
					{
                        Sop sop = _studyLoader.LoadNextSop();
						if (sop == null)
							break;

						sops.Add(sop);
					}

					if (sops.Count == 0)
						throw new LoadStudyException(args.StudyInstanceUid, total, total);

					return sops;
				}
				catch (Exception e)
				{
					DisposeSops(sops);
					throw new LoadStudyException(args.StudyInstanceUid, total, total, e);
				}
			}

			private void AddSops()
			{
				int total = 0;
				int failed = 0;

				//don't try to load something that's already there.
				if (IsStudyInStudyTree)
				{
					Platform.Log(LogLevel.Debug, "Study '{0} already exists in study tree.", StudyInstanceUid);
					return;
				}

				IsValidPrior = true;

				List<Sop> sops = _sops;
				_sops = null;

				foreach (Sop sop in sops)
				{
					try
					{
						_viewer.StudyTree.AddSop(sop);
					}
					catch (SopValidationException e)
					{
						++failed;
						Platform.Log(LogLevel.Error, e);
						sop.Dispose();
					}
					catch (Exception e)
					{
						++failed;
						Platform.Log(LogLevel.Error, e);
					}

					++total;
				}

				Study study = _viewer.StudyTree.GetStudy(StudyInstanceUid);

				LoadStudyException error = null;
				if (failed > 0 || study == null)
					error = new LoadStudyException(StudyInstanceUid, total, failed);

				if (study != null)
				{
					_viewer.EventBroker.OnStudyLoaded(new StudyLoadedEventArgs(study, error));

                    IPrefetchingStrategy prefetchingStrategy = _studyLoader.PrefetchingStrategy;
					if (prefetchingStrategy != null)
						prefetchingStrategy.Start(_viewer);
				}

				if (error != null)
					throw error;
			}

			private void OnLoadPriorStudyFailed()
			{
				//don't report for an existing study or one that was partially loaded.
				if (!IsStudyInStudyTree)
				{
					Platform.Log(LogLevel.Error, Error,
						"Failed to load prior study '{0}' from server '{1}'.",
						StudyInstanceUid, Server);

					IsValidPrior = true;
					if (_args != null)
						_viewer.EventBroker.OnStudyLoadFailed(new StudyLoadFailedEventArgs(_args, Error));
					else
						_viewer.EventBroker.OnStudyLoadFailed(new StudyLoadFailedEventArgs(_studyItem, Error));
				}
			}

			private void OnSopsLoaded()
			{
				if (IsRunningOnUiThread)
				{
					lock (_syncLock)
					{
						if (!_disposed)
						{
							try
							{
								AddSops();
								Monitor.Pulse(_syncLock);
							}
							catch (Exception e)
							{
								OnLoadPriorStudyFailed(e);
							}
						}
					}
				}
				else
				{
					lock (_syncLock)
					{
						if (!_disposed)
						{
							_uiThreadContext.Post(delegate { OnSopsLoaded(); }, null);
							Monitor.Wait(_syncLock);
						}
					}
				}
			}
			
			private void OnLoadPriorStudyFailed(object error)
			{
				if (IsRunningOnUiThread)
				{
					lock (_syncLock)
					{
						try
						{
							if (!_disposed)
							{
								Error = (Exception)error;
								OnLoadPriorStudyFailed();
							}
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Unexpected error occurred while firing StudyLoadFailed event.");
						}
						finally
						{
							Monitor.Pulse(_syncLock);
						}
					}
				}
				else
				{
					lock (_syncLock)
					{
						if (!_disposed)
						{
							_uiThreadContext.Post(OnLoadPriorStudyFailed, error);
							Monitor.Wait(_syncLock);
						}
					}
				}
			}

			private static void DisposeSops(List<Sop> sops)
			{
				foreach (Sop sop in sops)
				{
					try
					{
						sop.Dispose();
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex);
					}
				}

				sops.Clear();
			}

			private void DisposeSops()
			{
				List<Sop> sops;

				lock(_syncLock)
				{
					_disposed = true;
					sops = _sops;
					_sops = null;
					Monitor.Pulse(_syncLock);
				}

				if (sops != null)
					DisposeSops(sops);
			}

		    #region IDisposable Members

			public void Dispose()
			{
				try
				{
					DisposeSops();
                    GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}
	}
}