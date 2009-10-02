#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
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
			private readonly List<Sop> _sops;
			private readonly LoadStudyArgs _args;
			private readonly StudyItem _studyItem;

			private readonly object _waitLock = new object();
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
				_sops = new List<Sop>();
			}

			private bool IsRunningOnUiThread
			{
				get { return _uiThreadContext == SynchronizationContext.Current; }	
			}

			private bool IsStudyInStudyTree
			{
				get { return _viewer.StudyTree.GetStudy(StudyInstanceUid) != null; }
			}

			public StudyItem StudyItem
			{
				get { return _studyItem; }	
			}

			public string StudyLoaderName
			{
				get
				{
					if (_args != null)
						return _args.StudyLoaderName;
					else
						return _studyItem.StudyLoaderName;
				}	
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

			public object Server
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
					LoadSops();
					OnSopsLoaded();
				}
				catch (Exception e)
				{
					OnLoadPriorStudyFailed(e);
				}
			}

			private void LoadSops()
			{
				//Use a new loader in case this call is asynchronous, we don't want to use the viewer's instance.
				IStudyLoader studyLoader = new StudyLoaderMap()[StudyLoaderName];

				StudyLoaderArgs args = new StudyLoaderArgs(StudyInstanceUid, Server);
				int total;

				try
				{
					total = studyLoader.Start(args);
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
						Sop sop = studyLoader.LoadNextSop();
						if (sop == null)
							break;

						_sops.Add(sop);
					}

					if (_sops.Count == 0)
						throw new LoadStudyException(args.StudyInstanceUid, total, total);
				}
				catch (Exception e)
				{
					foreach (Sop sop in _sops)
					{
						try
						{
							sop.Dispose();
						}
						catch(Exception ex)
						{
							Platform.Log(LogLevel.Error, ex);
						}
					}

					_sops.Clear();

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

				List<Sop> sops = new List<Sop>(_sops);
				_sops.Clear();
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

					IPrefetchingStrategy prefetchingStrategy = _viewer.StudyLoaders[StudyLoaderName].PrefetchingStrategy;
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
						"Failed to load prior study '{0}' from study loader '{1}'.",
						StudyInstanceUid, StudyLoaderName);

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
					lock (_waitLock)
					{
						if (!_disposed)
						{
							try
							{
								AddSops();
								Monitor.Pulse(_waitLock);
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
					lock (_waitLock)
					{
						if (!_disposed)
						{
							_uiThreadContext.Post(delegate { OnSopsLoaded(); }, null);
							Monitor.Wait(_waitLock);
						}
					}
				}
			}
			
			private void OnLoadPriorStudyFailed(object error)
			{
				if (IsRunningOnUiThread)
				{
					lock (_waitLock)
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
							Monitor.Pulse(_waitLock);
						}
					}
				}
				else
				{
					lock (_waitLock)
					{
						if (!_disposed)
						{
							_uiThreadContext.Post(OnLoadPriorStudyFailed, error);
							Monitor.Wait(_waitLock);
						}
					}
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					lock(_waitLock)
					{
						_disposed = true;
						Monitor.Pulse(_waitLock);
					}

					foreach (Sop sop in _sops)
						sop.Dispose();

					_sops.Clear();

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