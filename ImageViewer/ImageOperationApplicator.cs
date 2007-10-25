#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{

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
	/// </remarks>
	public class ImageOperationApplicator : IMemorable
	{
		private delegate IEnumerable<IPresentationImage> GetImagesDelegate();

		private readonly IPresentationImage _presentationImage;
		private readonly IImageOperation _operation;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presentationImage">The 'current' <see cref="IPresentationImage"/>.</param>
		/// <param name="operation">The operation to be performed on the current <see cref="IPresentationImage"/> and/or its linked images.</param>
		public ImageOperationApplicator(IPresentationImage presentationImage, IImageOperation operation)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			Platform.CheckForNullReference(operation, "operation");

			_presentationImage = presentationImage;
			_operation = operation;
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of all image originators that will be affected by the <see cref="IImageOperation"/>.
		/// </summary>
		/// <remarks>
		/// Only those originators for which <see cref="IImageOperation.AppliesTo"/> returns true <b>and</b>
		/// <see cref="IImageOperation.GetOriginator"/> returns a non-null value will have their states 
		/// captured.
		/// </remarks>
		public IMemento CreateMemento()
		{
			List<ImageOriginatorMemento> imageOriginatorMementos = new List<ImageOriginatorMemento>();
			foreach (IPresentationImage image in GetAllImages())
			{
				IMemorable originator = GetOriginator(image);
				if (originator != null)
				{
					IMemento memento = originator.CreateMemento();
					imageOriginatorMementos.Add(new ImageOriginatorMemento(image, originator, memento));
				}
			}

			return new ImageOperationApplicatorMemento(imageOriginatorMementos);
		}

		/// <summary>
		/// Restores the state of all image originators that were affected by the <see cref="IImageOperation"/>.
		/// </summary>
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			ImageOperationApplicatorMemento applicatorMemento = memento as ImageOperationApplicatorMemento;
			Platform.CheckForInvalidCast(applicatorMemento, "memento", typeof(ImageOperationApplicatorMemento).FullName);

			// Apply memento to all originators of linked images
			foreach (ImageOriginatorMemento imageOriginatorMemento in applicatorMemento.ImageOriginatorMementos)
			{
				imageOriginatorMemento.Originator.SetMemento(imageOriginatorMemento.Memento);
				imageOriginatorMemento.PresentationImage.Draw();
			}
		}

		#endregion

		/// <summary>
		/// Applies the same <see cref="IImageOperation"/> to the current image as well as all its linked images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IImageOperation.Apply"/> will be called only for images where 
		/// <see cref="IImageOperation.AppliesTo"/> has returned true <b>and</b> <see cref="IImageOperation.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// Each affected image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public void ApplyToAllImages()
		{
			Apply(GetAllImages);
		}

		/// <summary>
		/// Applies the same <see cref="IImageOperation"/> to all linked images, but not the current image itself.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IImageOperation.Apply"/> will be called only for images where 
		/// <see cref="IImageOperation.AppliesTo"/> has returned true <b>and</b> <see cref="IImageOperation.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// Each affected image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public void ApplyToLinkedImages()
		{
			Apply(GetAllLinkedImages);
		}

		private void Apply(GetImagesDelegate getImages)
		{
			foreach (IPresentationImage image in getImages())
			{
				if (AppliesTo(image))
				{
					_operation.Apply(image);
					image.Draw();
				}
			}
		}

		private IEnumerable<IPresentationImage> GetAllImages()
		{
			yield return _presentationImage;

			foreach (IPresentationImage image in this.GetAllLinkedImages())
				yield return image;
		}

		private IEnumerable<IPresentationImage> GetAllLinkedImages()
		{
			IDisplaySet parentDisplaySet = _presentationImage.ParentDisplaySet;
			IImageSet parentImageSet = parentDisplaySet.ParentImageSet;

			// If display set is linked and selected, then iterate through all the linked images
			// from the other linked display sets
			if (parentDisplaySet.Linked)
			{
				foreach (IDisplaySet currentDisplaySet in parentImageSet.LinkedDisplaySets)
				{
					foreach (IPresentationImage image in GetAllLinkedImages(currentDisplaySet))
						yield return image;
				}
			}
			// If display set is just selected, then iterate through all the linked images
			// in that display set.
			else
			{
				foreach (IPresentationImage image in GetAllLinkedImages(parentDisplaySet))
					yield return image;
			}
		}

		private IEnumerable<IPresentationImage> GetAllLinkedImages(IDisplaySet displaySet)
		{
			foreach (IPresentationImage image in displaySet.LinkedPresentationImages)
			{
				if (image != _presentationImage)
					yield return image;
			}
		}

		private bool AppliesTo(IPresentationImage image)
		{
			return GetOriginator(image) != null;
		}

		private IMemorable GetOriginator(IPresentationImage image)
		{
			IMemorable originator = _operation.GetOriginator(image);
			bool applies = _operation.AppliesTo(image);
			return applies ? originator : null;
		}
	}
}
