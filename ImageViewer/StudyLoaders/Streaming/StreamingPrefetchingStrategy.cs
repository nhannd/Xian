using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{

	class StreamingPrefetchingStrategy
	{
		private IImageViewer _imageViewer;
		private bool _stopped = false;
		private int _threadCount = 0;

		public StreamingPrefetchingStrategy(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;	
			_imageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		public void Stop()
		{
			_stopped = true;

			// Wait until all threads are done
			while (_threadCount > 0)
				Thread.Sleep(5);

			_imageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.NewDisplaySet != null)
			{
				Interlocked.Increment(ref _threadCount);
				ThreadPool.QueueUserWorkItem(PrefetchPixelData, e.NewDisplaySet);
			}
		}

		private void PrefetchPixelData(object obj)
		{
			IDisplaySet displaySet = obj as IDisplaySet;

			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				if (_stopped)
				{
					Interlocked.Decrement(ref _threadCount);
					return;
				}

				IImageSopProvider provider = image as IImageSopProvider;

				if (provider != null)
					provider.Frame.GetNormalizedPixelData();

				IAnnotationLayoutProvider annotations = image as IAnnotationLayoutProvider;

				if (annotations != null)
				{
					IAnnotationLayout layout = annotations.AnnotationLayout;
				}

				//IImageGraphicProvider graphicProvider = image as IImageGraphicProvider;

				//if (graphicProvider != null)
				//{
				//    GrayscaleImageGraphic grayscaleGraphic = graphicProvider.ImageGraphic as GrayscaleImageGraphic;

				//    if (grayscaleGraphic != null)
				//    {
				//        IComposedLut lut = grayscaleGraphic.OutputLut;
				//    }
				//}
			}

			Interlocked.Decrement(ref _threadCount);
		}
	}
}
