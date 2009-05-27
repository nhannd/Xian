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

using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class LoadPriorStudiesException : LoadMultipleStudiesException
	{
		internal LoadPriorStudiesException(ICollection<Exception> exceptions, int totalStudies)
			: base(FormatMessage(exceptions, totalStudies), exceptions, totalStudies)
		{
			FindFailed = false;
		}

		internal LoadPriorStudiesException()
			: base("The query for prior studies has failed.", new List<Exception>(), 0)
		{
			FindFailed = true;
		}

		public readonly bool FindFailed;

		private static string FormatMessage(ICollection<Exception> exceptions, int totalStudies)
		{
			return String.Format("{0} of {1} prior studies produced one or more errors while loading.", exceptions.Count, totalStudies);
		}
	}

	public class LoadPriorStudyFailedEventArgs : EventArgs
	{
		internal LoadPriorStudyFailedEventArgs(StudyItem study, Exception error)
		{
			this.Study = study;
			this.Error = error;
		}

		public readonly StudyItem Study;
		public readonly Exception Error;
	}

	public interface IPriorStudyLoader
	{
		bool IsActive { get; }
		event EventHandler IsActiveChanged;

		//TODO (CR May09): resolve events with EventBroker; combine 'failed' and 'success' events into one.
		event EventHandler<LoadPriorStudyFailedEventArgs> LoadPriorStudyFailed;
	}

	public partial class ImageViewerComponent
	{
		internal class AsyncPriorStudyLoader : IPriorStudyLoader
		{
			private readonly ImageViewerComponent _imageViewer;
			private readonly List<SingleStudyLoader> _singleStudyLoaders;
			private readonly List<Exception> _loadStudyExceptions;

			private volatile bool _isActive = false;
			private event EventHandler _isActiveChanged;
			event EventHandler<LoadPriorStudyFailedEventArgs> _loadPriorStudyFailed;

			private volatile bool _stop = false;

			private Thread _thread;
			private SynchronizationContext _synchronizationContext;

			private readonly IPriorStudyFinder _priorStudyFinder;
			private volatile StudyItemList _queryResults;
			private volatile int _actualPriorCount = 0;
			private volatile bool _findFailed = false;

			public AsyncPriorStudyLoader(ImageViewerComponent imageViewer, IPriorStudyFinder priorStudyFinder)
			{
				_imageViewer = imageViewer;
				_singleStudyLoaders = new List<SingleStudyLoader>();
				_loadStudyExceptions = new List<Exception>();
				_priorStudyFinder = priorStudyFinder;
				_priorStudyFinder.SetImageViewer(_imageViewer);
			}

			public bool IsActive
			{
				get { return _isActive; }
				private set
				{
					if (_isActive == value)
						return;

					_isActive = value;
					EventsHelper.Fire(_isActiveChanged, this, EventArgs.Empty);
				}
			}

			public event EventHandler IsActiveChanged
			{
				add { _isActiveChanged += value; }
				remove { _isActiveChanged -= value; }
			}

			public event EventHandler<LoadPriorStudyFailedEventArgs> LoadPriorStudyFailed
			{
				add { _loadPriorStudyFailed += value; }
				remove { _loadPriorStudyFailed -= value; }
			}

			public void Start()
			{
				if (_thread != null)
					return;

				if (_priorStudyFinder == PriorStudyFinder.Null)
					return;

				_stop = false;
				IsActive = true;
				_synchronizationContext = SynchronizationContext.Current;
				_thread = new Thread(Run);
				_thread.IsBackground = false;
				_thread.Start();
			}

			public void Stop()
			{
				if (_stop || _thread == null)
					return;

				_stop = true;
				_priorStudyFinder.Cancel();
				_thread.Join();
				_thread = null;
			}

			private void Run()
			{
				try
				{
					FindAndAddPriors();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "An unexpected error has occurred while finding/adding prior studies.");
				}
				finally
				{
					_synchronizationContext.Post(OnComplete, null);
				}
			}

			private void FindAndAddPriors()
			{
				try
				{
					_queryResults = _priorStudyFinder.FindPriorStudies() ?? new StudyItemList();
					if (_queryResults == null || _queryResults.Count == 0)
						return;
				}
				catch (Exception e)
				{
					_queryResults = new StudyItemList();
					_findFailed = true;
					Platform.Log(LogLevel.Error, e, "The search for prior studies has failed.");
					return;
				}

				foreach (StudyItem result in _queryResults)
				{
					if (_stop)
						break;

					try
					{
						SingleStudyLoader loader = new SingleStudyLoader(_imageViewer, result);
						_singleStudyLoaders.Add(loader);
						loader.LoadSops();
						OnSopsLoaded(loader);
					}
					catch(LoadStudyException e)
					{
						Platform.Log(LogLevel.Error, e, "Failed to load prior study '{0}' from study loader '{1}'.",
									 result.StudyInstanceUID, result.StudyLoaderName);

						OnLoadPriorStudyFailed(result, e);
					}
					catch (StudyLoaderNotFoundException e)
					{
						Platform.Log(LogLevel.Error, e, "Failed to load prior study '{0}' from study loader '{1}'; study loader doesn't exist.",
									 result.StudyInstanceUID, result.StudyLoaderName);

						OnLoadPriorStudyFailed(result, e);
					}
				}
			}

			private void OnSopsLoaded(SingleStudyLoader loader)
			{
				_synchronizationContext.Post(AddSops, loader);
			}

			private void OnLoadPriorStudyFailed(StudyItem result, Exception error)
			{
				_synchronizationContext.Post(OnLoadPriorStudyFailed, new LoadPriorStudyFailedEventArgs(result, error));
			}

			private void OnLoadPriorStudyFailed(object loadPriorStudyFailedEventArgs)
			{
				if (!_stop)
				{
					LoadPriorStudyFailedEventArgs args = (LoadPriorStudyFailedEventArgs) loadPriorStudyFailedEventArgs;
					//don't re-report for an existing study.
					if (null == _imageViewer.StudyTree.GetStudy(args.Study.StudyInstanceUID))
					{
						++_actualPriorCount;
						_loadStudyExceptions.Add(args.Error);
						EventsHelper.Fire(_loadPriorStudyFailed, this, args);
					}
				}
			}

			private void AddSops(object theLoader)
			{
				SingleStudyLoader loader = (SingleStudyLoader)theLoader;
				if (!_stop)
				{
					try
					{
						if (null == _imageViewer.StudyTree.GetStudy(loader.StudyInstanceUid))
						{
							loader.AddSops();
							++_actualPriorCount;
						}
					}
					catch(LoadStudyException e)
					{
						Platform.Log(LogLevel.Error, e, "An error occurred while loading sops for prior study '{0}' from study loader '{1}'.",
									 loader.StudyInstanceUid, loader.StudyLoaderName);

						OnLoadPriorStudyFailed(new LoadPriorStudyFailedEventArgs(loader.StudyItem, e));
					}
				}
			}

			private void OnComplete(object nothing)
			{
				IsActive = false;
				DisposeLoaders();

				if (_stop)
					return;

				try
				{
					VerifyLoadPriors();
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
				}
			}

			private void VerifyLoadPriors()
			{
				if (_findFailed)
					throw new LoadPriorStudiesException();
				else if (_loadStudyExceptions.Count > 0)
					throw new LoadPriorStudiesException(_loadStudyExceptions, _actualPriorCount);
			}

			private void DisposeLoaders()
			{
				foreach (SingleStudyLoader loader in _singleStudyLoaders)
					loader.Dispose();

				_singleStudyLoaders.Clear();
			}
		}
	}
}