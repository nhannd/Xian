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
	/// <summary>
	/// Exception class for when loading prior studies has failed.
	/// </summary>
	/// <remarks>
	/// Because loading of priors is handled automatically by the framework, this class is both
	/// thrown an caught by the framework, but is handled by the <see cref="ExceptionHandler"/>.
	/// This is done to allow for custom exception handling.
	/// </remarks>
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

		/// <summary>
		/// Gets whether or not it was the find operation that failed (e.g. <see cref="IPriorStudyFinder"/>).
		/// </summary>
		public readonly bool FindFailed;

		private static string FormatMessage(ICollection<Exception> exceptions, int totalStudies)
		{
			return String.Format("{0} of {1} prior studies produced one or more errors while loading.", exceptions.Count, totalStudies);
		}
	}

	/// <summary>
	/// Defines the interface for automatic loading of related (or 'prior') studies.
	/// </summary>
	/// <remarks>
	/// The <see cref="ImageViewerComponent"/> automatically loads prior studies
	/// asynchronously in the background so that developers don't have to worry about such details.
	/// The only part of loading priors that can be customized is the search algorithm, by
	/// implementing <see cref="IPriorStudyFinder"/>.
	/// </remarks>
	public interface IPriorStudyLoader
	{
		/// <summary>
		/// Gets whether or not the <see cref="IPriorStudyLoader"></see> is actively searching for and/or loading priors.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Occurs when <see cref="IsActive"/> has changed.
		/// </summary>
		event EventHandler IsActiveChanged;
	}

	public partial class ImageViewerComponent
	{
		//NOTE: Async because otherwise the name conflicts with the ImageViewer.PriorStudyLoader property.
		internal class AsynchronousPriorStudyLoader : IPriorStudyLoader
		{
			private readonly ImageViewerComponent _imageViewer;
			private readonly List<SingleStudyLoader> _singleStudyLoaders;

			private volatile bool _isActive = false;
			private event EventHandler _isActiveChanged;

			private volatile bool _stop = false;

			private Thread _thread;
			private SynchronizationContext _synchronizationContext;

			private readonly IPriorStudyFinder _priorStudyFinder;
			private volatile StudyItemList _queryResults;
			private volatile bool _findFailed = false;

			public AsynchronousPriorStudyLoader(ImageViewerComponent imageViewer, IPriorStudyFinder priorStudyFinder)
			{
				_imageViewer = imageViewer;
				_singleStudyLoaders = new List<SingleStudyLoader>();
				_priorStudyFinder = priorStudyFinder;
				_priorStudyFinder.SetImageViewer(_imageViewer);
			}

			#region IPriorStudyLoader

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

			#endregion

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
				_thread.Priority = ThreadPriority.BelowNormal;
				_thread.IsBackground = false;
				_thread.Start();
			}

			public void Stop()
			{
				if (_stop || _thread == null)
					return;

				_stop = true;
				_priorStudyFinder.Cancel();
				DisposeLoaders();
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
					if (_queryResults.Count == 0)
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

					SingleStudyLoader loader = new SingleStudyLoader(_synchronizationContext, _imageViewer, result);
					_singleStudyLoaders.Add(loader);
					loader.LoadStudy();
				}
			}


			private void OnComplete(object nothing)
			{
				IsActive = false;
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

			private int GetValidPriorCount()
			{
				int count = 0;
				foreach (SingleStudyLoader loader in _singleStudyLoaders)
				{
					if (loader.IsValidPrior)
						++count;
				}

				return count;
			}

			private List<Exception> GetLoadErrors()
			{
				List<Exception> errors = new List<Exception>();

				foreach (SingleStudyLoader loader in _singleStudyLoaders)
				{
					if (loader.IsValidPrior && loader.Error != null)
						errors.Add(loader.Error);
				}

				return errors;
			}

			private void VerifyLoadPriors()
			{
				if (_findFailed)
				{
					throw new LoadPriorStudiesException();
				}
				else
				{
					List<Exception> errors = GetLoadErrors();
					if (errors.Count > 0)
						throw new LoadPriorStudiesException(errors, GetValidPriorCount());
				}
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