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
		private readonly object _syncLock = new object();

		public StreamingPrefetchingStrategy(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;	
			_imageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.NewDisplaySet != null)
				ThreadPool.QueueUserWorkItem(PrefetchPixelData, e.NewDisplaySet);
		}

		private void PrefetchPixelData(object obj)
		{
			IDisplaySet displaySet = obj as IDisplaySet;

			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				//lock (_syncLock)
				//{
				//    Bitmap bitmap = image.DrawToBitmap(2, 2);
				//    bitmap.Dispose();
				//}

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
		}
	}
}
