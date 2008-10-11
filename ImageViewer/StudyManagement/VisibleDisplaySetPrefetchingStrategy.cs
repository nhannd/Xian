using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;

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

		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		public string Name
		{
			//TODO: Rename the resource.
			get { return SR.PrefetchingStrategyNameVisibleDisplaySet; }
		}

		/// <summary>
		/// Gets the friendly description of the prefetching strategy
		/// </summary>
		public string Description
		{
			//TODO: Rename the resource.

			get { return SR.PrefetchingStrategyDescriptionVisibleDisplaySet; }
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
			if (_threadPool == null)
			{
				const int numberOfThreads = 10;
				_threadPool = new SimpleBlockingThreadPool(numberOfThreads);
			}

			_threadPool.Start();

			if (_imageViewer == null)
			{
				_imageViewer = imageViewer;
				_imageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
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
				//TODO: put whole display set in or presentation images?
				//TODO: check if same display set is already loading.
				//TODO: load image by image?
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
