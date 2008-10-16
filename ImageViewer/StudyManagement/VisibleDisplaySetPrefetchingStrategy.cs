using ClearCanvas.Common.Utilities;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A prefetching strategy triggered when a display set is made visible.
	/// </summary>
	public class VisibleDisplaySetPrefetchingStrategy : IPrefetchingStrategy
	{
		private IImageViewer _imageViewer;
		private volatile bool _stopped = false;
		private SimpleBlockingThreadPool _threadPool;
		private readonly int _numberOfThreads = 5;

		/// <summary>
		/// Constructor.
		/// </summary>
		public VisibleDisplaySetPrefetchingStrategy()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public VisibleDisplaySetPrefetchingStrategy(int numberOfThreads)
		{
			Platform.CheckArgumentRange(numberOfThreads, 0, 50, "numberOfThreads");

			_numberOfThreads = numberOfThreads;
		}

		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		public string Name
		{
			get { return SR.NamePrefetchingStrategyVisibleDisplaySet; }
		}

		/// <summary>
		/// Gets the friendly description of the prefetching strategy
		/// </summary>
		public string Description
		{
			get { return SR.DescriptionPrefetchingStrategyVisibleDisplaySet; }
		}

		/// <summary>
		/// Starts prefetching pixel data in the background.
		/// </summary>
		/// <param name="imageViewer"></param>
		/// <remarks>
		/// Use <paramref name="imageViewer"/> to determine how prefetching is done.
		/// </remarks>
		public void Start(IImageViewer imageViewer)
		{
			if (_numberOfThreads > 0)
			{
				if (_threadPool == null)
				{
					_threadPool = new SimpleBlockingThreadPool(_numberOfThreads);
					_threadPool.ThreadPriority = ThreadPriority.Lowest;
				}

				_threadPool.Start();

				if (_imageViewer == null)
				{
					_imageViewer = imageViewer;
					_imageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
				}
			}
		}

		/// <summary>
		/// Stops prefetching of pixel data in the background.
		/// </summary>
		/// <remarks>
		/// Implementers should ensure that all background threads have terminated
		/// before this method returns.
		/// </remarks>
		public void Stop()
		{
			_stopped = true;

			if (_threadPool != null)
			{
				_threadPool.Stop(false);
				_imageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			}
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.NewDisplaySet != null)
			{
				// TODO: Later, we may need to revise the strategy to be more predictive,
				// and to load images rather than display sets.
				_threadPool.Enqueue(delegate()
				                    	{
				                    		PrefetchPixelData(e.NewDisplaySet);
				                    	});
			}
		}

		private void PrefetchPixelData(IDisplaySet displaySet)
		{
			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				// Quit when we've received a signal to stop, or when the available
				// memory has dropped below a certain threshold
				if (_stopped || SystemResources.GetAvailableMemory(SizeUnits.Megabytes) < 100)
				{
					return;
				}

				IImageSopProvider provider = image as IImageSopProvider;

				if (provider != null)
					provider.Frame.GetNormalizedPixelData();
			}
		}
	}
}
