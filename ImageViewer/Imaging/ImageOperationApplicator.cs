using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class ImageAndOriginator
	{
		private PresentationImage _presentationImage;
		private IMemorable _originator;

		public ImageAndOriginator(PresentationImage presentationImage, IMemorable originator)
		{
			_presentationImage = presentationImage;
			_originator = originator;
		}

		public PresentationImage PresentationImage
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
		private PresentationImage _presentationImage;

		public ImageOperationApplicator(PresentationImage selectedPresentationImage)
		{
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
				imageAndOriginator.Originator.SetMemento(applicatorMemento.Memento);
				imageAndOriginator.PresentationImage.Draw(false);
			}
		}

		#endregion

		protected abstract IMemorable GetOriginator(PresentationImage image);

		private IList<ImageAndOriginator> GetImagesAndOriginatorsFromLinkedImages()
		{
			DisplaySet displaySet = _presentationImage.ParentDisplaySet;
			LogicalWorkspace logicalWorkspace = displaySet.ParentLogicalWorkspace;

			List<ImageAndOriginator> imagesAndOrginators = new List<ImageAndOriginator>();

			// If display set is linked and selected, then remember all the linked images
			// from the other linked display sets
			if (displaySet.Linked)
			{
				foreach (DisplaySet currentDisplaySet in logicalWorkspace.LinkedDisplaySets)
				{
					foreach (PresentationImage image in currentDisplaySet.LinkedPresentationImages)
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
				foreach (PresentationImage image in displaySet.LinkedPresentationImages)
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
