using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class ImageOriginatorMemento
	{
		private IPresentationImage _presentationImage;
		private IMemorable _originator;
		private IMemento _memento;

		public ImageOriginatorMemento(
			IPresentationImage presentationImage, 
			IMemorable originator,
			IMemento memento)
		{
			_presentationImage = presentationImage;
			_originator = originator;
			_memento = memento;
		}

		public IPresentationImage PresentationImage
		{
			get { return _presentationImage; }
		}

		public IMemorable Originator
		{
			get { return _originator; }
		}

		public IMemento Memento
		{
			get { return _memento; }
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
	/// example, when an image is zoomed, it is expected that all linked images 
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
	/// <see cref="ClearCanvas.ImageViewer.Imaging.VoiLutOperationApplicator"/>
	/// and <see cref="ClearCanvas.ImageViewer.Graphics.SpatialTransformApplicator"/> 
	/// which allow Voi Lut installation/manipulation and 
	/// zoom/pan/etc. respectively to be applied across all linked images.  If you wish
	/// to write your own subclass of <see cref="ImageOperationApplicator"/>, it is
	/// recommended that you model it after those two subclasses.
	/// </para>
	/// </remarks>
	public abstract class ImageOperationApplicator : IMemorable
	{
		/// <summary>
		/// Delegate used by <see cref="ImageOperationApplicator"/> to apply an operation to presentation images (<see cref="IPresentationImage"/>).
		/// </summary>
		/// <param name="image"></param>
		public delegate void Apply(IPresentationImage image);

		private IPresentationImage _presentationImage;

		protected ImageOperationApplicator(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			_presentationImage = presentationImage;
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of <see cref="ImageOperationApplicator"/>.
		/// </summary>
		/// <returns>An <see cref="IMemento"/>.</returns>
		public IMemento CreateMemento()
		{
			List<ImageOriginatorMemento> imageOriginatorMementos = new List<ImageOriginatorMemento>();

			ApplyToAllImages(
				delegate(IPresentationImage image)
				{
					IMemorable originator = GetOriginator(image);

					if (originator != null)
					{
						ImageOriginatorMemento obj = new ImageOriginatorMemento(
							image,
							originator,
							originator.CreateMemento());

						imageOriginatorMementos.Add(obj);
					}
				}, false);

			IMemento applicatorMemento = new ImageOperationApplicatorMemento(imageOriginatorMementos);
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
			foreach (ImageOriginatorMemento imageOriginatorMemento in applicatorMemento.ImageOriginatorMementos)
			{
				if (imageOriginatorMemento.Originator != null)
				{
					imageOriginatorMemento.Originator.SetMemento(imageOriginatorMemento.Memento);
					imageOriginatorMemento.PresentationImage.Draw();
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
		/// <see cref="ClearCanvas.ImageViewer.Imaging.VoiLutOperationApplicator"/>
		/// and <see cref="ClearCanvas.ImageViewer.Graphics.SpatialTransformApplicator"/> 
		/// which allow Voi Lut installation/manipulation and 
		/// zoom/pan/etc. respectively to be applied across all linked images.  If you wish
		/// to write your own subclass of <see cref="ImageOperationApplicator"/>, it is
		/// recommended that you model it after those two subclasses.
		/// </para>
		/// </remarks>
		protected abstract IMemorable GetOriginator(IPresentationImage image);

		/// <summary>
		/// Applies the same operation to the current image as well as all its linked images.
		/// Each image is drawn automatically by this method.
		/// </summary>
		/// <param name="apply">The operation to perform on each image.</param>
		public void ApplyToAllImages(Apply apply)
		{
			ApplyToLinkedImages(apply, false, true);
		}

		/// <summary>
		/// Applies the same operation to all images linked to the current image, but not the current image itself.
		/// Each image is drawn automatically by this method.
		/// </summary>
		/// <param name="apply">The operation to perform on each image.</param>
		public void ApplyToLinkedImages(Apply apply)
		{
			ApplyToLinkedImages(apply, true, true);
		}

		private void ApplyToAllImages(Apply apply, bool draw)
		{
			ApplyToLinkedImages(apply, false, draw);
		}

		private void ApplyToLinkedImages(Apply apply, bool skipThisImage, bool draw)
		{
			IDisplaySet displaySet = _presentationImage.ParentDisplaySet;
			IImageSet imageSet = displaySet.ParentImageSet;

			// If display set is linked and selected, then iterate through all the linked images
			// from the other linked display sets
			if (displaySet.Linked)
			{
				foreach (IDisplaySet currentDisplaySet in imageSet.LinkedDisplaySets)
					ApplyToLinkedImages(currentDisplaySet, apply, skipThisImage, draw);
			}
			// If display set is just selected, then iterate through all the linked images
			// in that display set
			else
			{
				ApplyToLinkedImages(displaySet, apply, skipThisImage, draw);
			}
		}

		private void ApplyToLinkedImages(IDisplaySet displaySet, Apply apply, bool skipThisImage, bool draw)
		{
			foreach (IPresentationImage image in displaySet.LinkedPresentationImages)
			{
				if (image == _presentationImage && skipThisImage)
					continue;

				apply(image);
				if (draw)
					image.Draw();
			}
		}
	}
}
