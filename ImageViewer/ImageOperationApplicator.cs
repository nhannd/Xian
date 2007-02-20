using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class ImageAndOriginator
	{
		private IPresentationImage _presentationImage;
		private IMemorable _originator;

		public ImageAndOriginator(IPresentationImage presentationImage, IMemorable originator)
		{
			_presentationImage = presentationImage;
			_originator = originator;
		}

		public IPresentationImage PresentationImage
		{
			get { return _presentationImage; }
		}

		public IMemorable Originator
		{
			get { return _originator; }
		}
	}

	public abstract class ImageOperationApplicator : IMemorable
	{
		private IPresentationImage _presentationImage;

		protected ImageOperationApplicator(IPresentationImage selectedPresentationImage)
		{
			// If this fails, the cast in the subclass' constructor may have failed
			Platform.CheckForNullReference(selectedPresentationImage, "selectedPresentationImage");

			_presentationImage = selectedPresentationImage;
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			IList<ImageAndOriginator> imagesAndOriginators = GetImagesAndOriginatorsFromLinkedImages();
			IMemento innerMemento = GetOriginator(_presentationImage).CreateMemento();
			IMemento applicatorMemento = new ImageOperationApplicatorMemento(imagesAndOriginators, innerMemento);
			return applicatorMemento;
		}
		
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			ImageOperationApplicatorMemento applicatorMemento = memento as ImageOperationApplicatorMemento;
			Platform.CheckForInvalidCast(applicatorMemento, "memento", "ImageOperationApplicatorMemento");

			// Apply memento to all originators of linked images
			foreach (ImageAndOriginator imageAndOriginator in applicatorMemento.LinkedImagesAndOriginators)
			{
				if (imageAndOriginator.Originator != null)
				{
					imageAndOriginator.Originator.SetMemento(applicatorMemento.Memento);

					if (imageAndOriginator.PresentationImage.Visible)
						imageAndOriginator.PresentationImage.Draw();
				}
			}
		}

		#endregion

		protected abstract IMemorable GetOriginator(IPresentationImage image);

		private IList<ImageAndOriginator> GetImagesAndOriginatorsFromLinkedImages()
		{
			IDisplaySet displaySet = _presentationImage.ParentDisplaySet;
			IImageSet imageSet = displaySet.ParentImageSet;

			List<ImageAndOriginator> imagesAndOrginators = new List<ImageAndOriginator>();

			// If display set is linked and selected, then remember all the linked images
			// from the other linked display sets
			if (displaySet.Linked)
			{
				foreach (IDisplaySet currentDisplaySet in imageSet.LinkedDisplaySets)
				{
					foreach (IPresentationImage image in currentDisplaySet.LinkedPresentationImages)
					{
						IMemorable originator = GetOriginator(image);
						ImageAndOriginator imageAndOriginator = new ImageAndOriginator(image, originator); 
						imagesAndOrginators.Add(imageAndOriginator);
					}
				}
			}
			// If display set is just selected, then remember all the linked images
			// in that display set
			else
			{
				foreach (IPresentationImage image in displaySet.LinkedPresentationImages)
				{
					IMemorable originator = GetOriginator(image);
					ImageAndOriginator imageAndOriginator = new ImageAndOriginator(image, originator); 
					imagesAndOrginators.Add(imageAndOriginator);
				}
			}

			return imagesAndOrginators;
		}
	}
}
