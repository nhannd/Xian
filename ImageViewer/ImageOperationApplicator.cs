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
	
	/// <summary>
	/// Encapsulates the creating and restoring of mementos across all
	/// linked <see cref="IPresentationImage"/> objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is often desirable to apply an operation across all linked 
	/// <see cref="IPresentationImage"/> objects.  For
	/// example, when an image is zoomed, it is expected that all linked images to 
	/// will zoom as well.  When that operation is undone, it is expected that
	/// it is undone on all of those images.  This class encapsulates that functionality
	/// so that the plugin developer doesn't have to deal with such details.
	/// </para>
	/// <para>
	/// This class is abstract, so it is up to the developer to subclass it and implement
	/// the <see cref="GetOriginator"/> method.
	/// </para>
	/// <para>
	/// By default, the Framework provides two subclasses of 
	/// <see cref="ImageOperationApplicator"/> called 
	/// <see cref="ClearCanvas.ImageViewer.Imaging.WindowLevelApplicator"/>
	/// and <see cref="ClearCanvas.ImageViewer.Graphics.SpatialTransformApplicator"/> 
	/// which allow window/level and
	/// zoom/pan/etc. respectively to be applied across all linked images.  If you wish
	/// to write your own subclass of <see cref="ImageOperationApplicator"/>, it is
	/// recommended that you model it after those two subclasses.
	/// </para>
	/// </remarks>
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

		/// <summary>
		/// Captures the state of <see cref="ImageOperationApplicator"/>.
		/// </summary>
		/// <returns>An <see cref="IMemento"/>.</returns>
		public IMemento CreateMemento()
		{
			IList<ImageAndOriginator> imagesAndOriginators = GetImagesAndOriginatorsFromLinkedImages();
			IMemento innerMemento = GetOriginator(_presentationImage).CreateMemento();
			IMemento applicatorMemento = new ImageOperationApplicatorMemento(imagesAndOriginators, innerMemento);
			return applicatorMemento;
		}

		/// <summary>
		/// Restores the state of <see cref="ImageOperationApplicator"/>.
		/// </summary>
		/// <param name="memento"></param>
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
					imageAndOriginator.PresentationImage.Draw();
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the object whose state is to be captured or restored.
		/// </summary>
		/// <param name="image">A <see cref="IPresentationImage"/> that contains
		/// the object whose state is to be captured or restored.</param>
		/// <returns>The object whose state is to be captured or restored.</returns>
		/// <remarks>
		/// <para>
		/// Typically, operations are applied to some aspect of the presentation image,
		/// such as zoom, pan, window/level, etc. That aspect will usually be 
		/// encapsulated as an object that is owned by the
		/// by <see cref="PresentationImage"/>.  <see cref="GetOriginator"/> allows
		/// the plugin developer to define what that object is.
		/// </para>
		/// <para>
		/// By default, the Framework provides two subclasses of 
		/// <see cref="ImageOperationApplicator"/> called 
		/// <see cref="ClearCanvas.ImageViewer.Imaging.WindowLevelApplicator"/>
		/// and <see cref="ClearCanvas.ImageViewer.Graphics.SpatialTransformApplicator"/> 
		/// which allow window/level and
		/// zoom/pan/etc. respectively to be applied across all linked images.  If you wish
		/// to write your own subclass of <see cref="ImageOperationApplicator"/>, it is
		/// recommended that you model it after those two subclasses.
		/// </para>
		/// </remarks>
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
