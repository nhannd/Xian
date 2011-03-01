#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using FrameBlockingThreadPool = ClearCanvas.ImageViewer.Common.BlockingThreadPool<ClearCanvas.ImageViewer.StudyManagement.Frame>;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A base implementation for an <see cref="IPrefetchingStrategy"/> based on a window of frames around a visible frame,
	/// with preference given to selected and unselected <see cref="IImageBox"/>es depending on a weighting factor.
	/// </summary>
	public abstract class WeightedWindowPrefetchingStrategy : PrefetchingStrategy
	{
		private ViewerFrameEnumerator _imageBoxEnumerator;
		private FrameBlockingThreadPool _retrieveThreadPool;
		private SimpleBlockingThreadPool _decompressThreadPool;
		private bool _isStarted = false;
		private bool _enabled = true;

		private ThreadPriority _retrievalThreadPriority = ThreadPriority.BelowNormal;
		private ThreadPriority _decompressionThreadPriority = ThreadPriority.Lowest;
		private int _retrievalThreadConcurrency = 5;
		private int _decompressionThreadConcurrency = 1;
		private int? _frameLookAheadSize = 20;
		private int _selectedFrameWeight = 3;
		private int _unselectedFrameWeight = 2;

		/// <summary>
		/// Initializes a new instance of a <see cref="WeightedWindowPrefetchingStrategy"/>.
		/// </summary>
		/// <param name="name">The name of the <see cref="IPrefetchingStrategy"/>.</param>
		/// <param name="description">The description of the <see cref="IPrefetchingStrategy"/>.</param>
		protected WeightedWindowPrefetchingStrategy(string name, string description)
			: base(name, description) {}

		/// <summary>
		/// Gets or sets a value indicating whether or not the strategy is enabled.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");
				_enabled = value;
			}
		}

		/// <summary>
		/// Gets or sets the priority of the threads to be used for frame pixel data retrieval.
		/// </summary>
		/// <remarks>It is strongly recommended that <see cref="ThreadPriority.AboveNormal"/> and <see cref="ThreadPriority.Highest"/> not be used.</remarks>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public ThreadPriority RetrievalThreadPriority
		{
			get { return _retrievalThreadPriority; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");
				_retrievalThreadPriority = value;
			}
		}

		/// <summary>
		/// Gets or sets the priority of the threads to be used for frame pixel data compression.
		/// </summary>
		/// <remarks>It is strongly recommended that <see cref="ThreadPriority.AboveNormal"/> and <see cref="ThreadPriority.Highest"/> not be used.</remarks>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public ThreadPriority DecompressionThreadPriority
		{
			get { return _decompressionThreadPriority; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");
				_decompressionThreadPriority = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of concurrent threads to be used for frame pixel data retrieval. Valid range is 1 or more.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int RetrievalThreadConcurrency
		{
			get { return _retrievalThreadConcurrency; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");

				Platform.CheckPositive(value, "RetrievalThreadConcurrency");
				_retrievalThreadConcurrency = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of concurrent threads to be used for frame pixel data decompression. Valid range is 1 or more.
		/// </summary>
		/// <remarks>Setting the thread concurrency to 0 effectively means that decompression will not be done ahead of time.</remarks>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int DecompressionThreadConcurrency
		{
			get { return _decompressionThreadConcurrency; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");

				Platform.CheckPositive(value, "DecompressionThreadConcurrency");
				_decompressionThreadConcurrency = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of frames to process in advance. Valid range is 0 or more.
		/// </summary>
		/// <remarks>
		/// <para>The look-ahead is performed in both directions from the currently selected frame. The maximum possible window size would thus be 2 times the look-ahead size.</para>
		/// <para>Setting the look-ahead size to null specifies an infinite look-ahead scope.</para>
		/// </remarks>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int? FrameLookAheadSize
		{
			get { return _frameLookAheadSize; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");

				if (value.HasValue)
					Platform.CheckNonNegative(value.Value, "FrameLookAheadSize");
				_frameLookAheadSize = value;
			}
		}

		//TODO (CR February 2011) - Low: Name - SelectedImageBoxWeight?
		/// <summary>
		/// Gets or sets the relative weighting given to loading frames in the selected <see cref="IImageBox"/>. Valid range is 1 or more.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int SelectedFrameWeight
		{
			get { return _selectedFrameWeight; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");

				Platform.CheckPositive(value, "SelectedFrameWeight");
				_selectedFrameWeight = value;
			}
		}

		//TODO (CR February 2011) - Low: Name - UnselectedImageBoxWeight?
		/// <summary>
		/// Gets or sets the relative weighting given to loading frames in unselected <see cref="IImageBox"/>. Valid range is 0 or more.
		/// </summary>
		/// <remarks>Setting the relative weight to 0 effectively means that only frames in the selected <see cref="IImageBox"/> will be preloaded.</remarks>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int UnselectedFrameWeight
		{
			get { return _unselectedFrameWeight; }
			protected set
			{
				if (_isStarted)
					throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");

				Platform.CheckNonNegative(value, "UnselectedFrameWeight");
				_unselectedFrameWeight = value;
			}
		}

		protected override void Start()
		{
			_isStarted = true;

			if (_enabled)
			{
				_imageBoxEnumerator = new ViewerFrameEnumerator(ImageViewer, SelectedFrameWeight, UnselectedFrameWeight, FrameLookAheadSize.GetValueOrDefault(-1), CanRetrieveFrame);

				_retrieveThreadPool = new FrameBlockingThreadPool(_imageBoxEnumerator, DoRetrieveFrame);
				_retrieveThreadPool.ThreadPoolName = GetThreadPoolName("Retrieve");
				_retrieveThreadPool.Concurrency = RetrievalThreadConcurrency;
				_retrieveThreadPool.ThreadPriority = _retrievalThreadPriority;
				_retrieveThreadPool.Start();

				_decompressThreadPool = new SimpleBlockingThreadPool(DecompressionThreadConcurrency);
				_decompressThreadPool.ThreadPoolName = GetThreadPoolName("Decompress");
				_decompressThreadPool.ThreadPriority = _decompressionThreadPriority;
				_decompressThreadPool.Start();
			}
		}

		protected override void Stop()
		{
			if (_retrieveThreadPool != null)
			{
				_retrieveThreadPool.Stop(false);
				_retrieveThreadPool = null;
			}

			if (_decompressThreadPool != null)
			{
				_decompressThreadPool.Stop(false);
				_decompressThreadPool = null;
			}

			if (_imageBoxEnumerator != null)
			{
				_imageBoxEnumerator.Dispose();
				_imageBoxEnumerator = null;
			}

			_isStarted = false;
		}

		protected abstract bool CanRetrieveFrame(Frame frame);

		protected abstract void RetrieveFrame(Frame frame);

		protected abstract bool CanDecompressFrame(Frame frame);

		protected abstract void DecompressFrame(Frame frame);

		private void DoRetrieveFrame(Frame frame)
		{
			try
			{
				RetrieveFrame(frame);

				if (CanDecompressFrame(frame))
					_decompressThreadPool.Enqueue(() => DoDecompressFrame(frame));
			}
			catch (Exception ex)
			{
				// don't let an uncaught exception crash the entire preloader
				Platform.Log(LogLevel.Warn, ex, "An error occured while trying to preload a frame.");
			}
		}

		private void DoDecompressFrame(Frame frame)
		{
			try
			{
				DecompressFrame(frame);
			}
			catch (Exception ex)
			{
				// don't let an uncaught exception crash the entire preloader
				Platform.Log(LogLevel.Warn, ex, "An error occured while trying to preload a frame.");
			}
		}

		private string GetThreadPoolName(string operationName)
		{
			var strategyName = Name;
			if (string.IsNullOrEmpty(strategyName))
				strategyName = GetType().FullName;
			return string.Format("{0}:{1}", strategyName, operationName);
		}
	}
}