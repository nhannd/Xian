#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal abstract class WeightedWindowPrefetchingStrategy : PrefetchingStrategy
	{
		private ViewerFrameEnumerator _imageBoxEnumerator;
		private BlockingThreadPool<Frame> _retrieveThreadPool;
		private SimpleBlockingThreadPool _decompressThreadPool;

		private int _imageWindow = 20;
		private int _selectedWeighting = 3;
		private int _unselectedWeighting = 2;
		private int _retrieveConcurrency = 5;
		private int _decompressConcurrency = 1;

		protected WeightedWindowPrefetchingStrategy(string name, string description)
			: base(name, description)
		{
		}

		public int RetrieveConcurrency
		{
			get { return _retrieveConcurrency; }
			set { _retrieveConcurrency = value; }
		}
		public int DecompressConcurrency
		{
			get { return _decompressConcurrency; }
			set
			{
				if (value < 0)
					value = 1;
				_decompressConcurrency = value;
			}
		}

		public int ImageWindow
		{
			get { return _imageWindow; }
			set { _imageWindow = value; }
		}

		public int SelectedWeighting
		{
			get { return _selectedWeighting; }
			set
			{
				if (value < 1)
					value = 1;

				_selectedWeighting = value;
			}
		}

		public int UnselectedWeighting
		{
			get { return _unselectedWeighting; }
			set { _unselectedWeighting = value; }
		}


		protected override void Start()
		{
			InternalStart();
		}

		private void InternalStart()
		{
			if (RetrieveConcurrency == 0)
				return;

			_imageBoxEnumerator = new ViewerFrameEnumerator(base.ImageViewer, 
				SelectedWeighting, UnselectedWeighting, ImageWindow, CanRetrieveFrame);

			_retrieveThreadPool = new BlockingThreadPool<Frame>(_imageBoxEnumerator, RetrieveFrame);
			_retrieveThreadPool.ThreadPoolName = "Retrieve";
			_retrieveThreadPool.Concurrency = RetrieveConcurrency;
			_retrieveThreadPool.ThreadPriority = ThreadPriority.BelowNormal;
			_retrieveThreadPool.Start();

			_decompressThreadPool = new SimpleBlockingThreadPool(DecompressConcurrency);
			_decompressThreadPool.ThreadPoolName = "Decompress";
			_decompressThreadPool.ThreadPriority = ThreadPriority.Lowest;
			_decompressThreadPool.Start();
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
		}

		protected abstract bool CanRetrieveFrame(Frame frame);

		protected abstract void RetrieveFrame(Frame frame, out bool decompress);

		protected abstract void LoadFramePixelData(Frame frame);

		private void RetrieveFrame(Frame frame)
		{
			bool decompress;
			RetrieveFrame(frame, out decompress);
			if (decompress)
				_decompressThreadPool.Enqueue(() => LoadFramePixelData(frame));
		}
	}
}