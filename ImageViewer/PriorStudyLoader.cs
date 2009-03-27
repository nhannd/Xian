using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public class PriorStudyLoaderException : Exception
	{
		internal PriorStudyLoaderException(bool findFailed, int noStudyLoaderFailures, int completeFailures, int partialFailures, int totalQueryResults)
			: base("A failure has occurred while loading prior studies.")
		{
			this.FindFailed = findFailed;
			this.NoStudyLoaderFailures = noStudyLoaderFailures;
			this.CompleteFailures = completeFailures;
			this.PartialFailures = partialFailures;
			this.TotalQueryResults = totalQueryResults;
		}

		public readonly bool FindFailed;
		public readonly int NoStudyLoaderFailures;
		public readonly int CompleteFailures;
		public readonly int PartialFailures;
		public readonly int TotalQueryResults;
	}

	public partial class ImageViewerComponent
	{
		private class PriorStudyLoader
		{
			private readonly ImageViewerComponent _imageViewer;
			private readonly StudyLoaderMap _studyLoaders;
			private readonly List<SingleStudyLoader> _singleStudyLoaders;

			private volatile bool _isActive = false;
			private volatile bool _stop = false;

			private Thread _thread;
			private SynchronizationContext _synchronizationContext;

			private readonly IPriorStudyFinder _priorStudyFinder;
			private volatile StudyItemList _queryResults;
			private volatile bool _findFailed = false;
			private volatile int _noStudyLoaderFailures = 0;

			public PriorStudyLoader(ImageViewerComponent imageViewer, IPriorStudyFinder priorStudyFinder)
			{
				_imageViewer = imageViewer;
				_studyLoaders = new StudyLoaderMap();
				_singleStudyLoaders = new List<SingleStudyLoader>();
				_priorStudyFinder = priorStudyFinder;
				_priorStudyFinder.SetImageViewer(_imageViewer);
			}

			public bool IsActive
			{
				get { return _isActive; }	
			}

			public void Start()
			{
				if (_thread != null)
					return;

				if (_priorStudyFinder == PriorStudyFinder.Null)
					return;

				_stop = false;
				_isActive = true;
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
				_thread.Join();
				_thread = null;
			}

			private void Run()
			{
				try
				{
					FindAndAddPriors();
				}
				catch(Exception e)
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
				catch(Exception e)
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
						LoadStudyArgs loadStudyArgs = new LoadStudyArgs(result);
						SingleStudyLoader loader = new SingleStudyLoader(loadStudyArgs);
						_singleStudyLoaders.Add(loader);
						loader.LoadSops(_studyLoaders);
						_synchronizationContext.Post(AddSops, loader);
					}
					catch(StudyLoaderNotFoundException ex)
					{
						++_noStudyLoaderFailures;

						Platform.Log(LogLevel.Error, ex, "Failed to load prior study '{0}'; study loader '{1}' does not exist.",
							result.StudyInstanceUID, result.StudyLoaderName);
					}
					catch(Exception e)
					{
						Platform.Log(LogLevel.Error, e, "Failed to load prior study '{0}' from study loader '{1}'.",
							result.StudyInstanceUID, result.StudyLoaderName);
					}
				}
			}

			private void AddSops(object theLoader)
			{
				SingleStudyLoader loader = (SingleStudyLoader)theLoader;
				if (!_stop)
				{
					loader.AddSops(_imageViewer);
				}
			}

			private void OnComplete(object nothing)
			{
				_isActive = false;
				int completeFailures = GetCompleteFailures();
				int partialFailures = GetPartialFailures();

				DisposeLoaders();

				if (_stop)
					return;

				try
				{
					VerifyLoadPriors(completeFailures, partialFailures);
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
				}
			}

			private void VerifyLoadPriors(int completeFailures, int partialFailures)
			{
				if (_findFailed || _noStudyLoaderFailures > 0 || completeFailures > 0 || partialFailures > 0)
					throw new PriorStudyLoaderException(_findFailed, _noStudyLoaderFailures, completeFailures, partialFailures, _queryResults.Count);
			}

			private int GetCompleteFailures()
			{
				int completeFailures = 0;
				foreach (SingleStudyLoader loader in _singleStudyLoaders)
				{
					if (loader.Total == 0 || loader.Failed >= loader.Total)
						++completeFailures;
				}

				return completeFailures - _noStudyLoaderFailures;
			}

			private int GetPartialFailures()
			{
				int partialFailures = 0;
				foreach (SingleStudyLoader loader in _singleStudyLoaders)
				{
					if (loader.Failed > 0 && loader.Failed < loader.Total)
						++partialFailures;
				}

				return partialFailures;
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