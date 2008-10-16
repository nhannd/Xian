#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	//TODO: remove this and use CompositeCommand.

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
		#region LinkedImageEnumerator class

		private class LinkedImageEnumerator : IEnumerable<IPresentationImage>
		{
			private readonly IPresentationImage _referenceImage;
			private bool _applyToAllImageSets;
			private bool _excludeReferenceImage;

			public LinkedImageEnumerator(IPresentationImage referenceImage)
			{
				_referenceImage = referenceImage;
				_applyToAllImageSets = false;
				_excludeReferenceImage = false;
			}

			public bool ApplyToAllImageSets
			{
				get { return _applyToAllImageSets; }
				set { _applyToAllImageSets = value; }
			}

			public bool ExcludeReferenceImage
			{
				get { return _excludeReferenceImage; }
				set { _excludeReferenceImage = value; }
			}

			private IEnumerable<IPresentationImage> GetAllLinkedImages()
			{
				IDisplaySet parentDisplaySet = _referenceImage.ParentDisplaySet;
				IImageSet parentImageSet = parentDisplaySet.ParentImageSet;

				// If display set is linked and selected, then iterate through all the linked images
				// from the other linked display sets
				if (parentDisplaySet.Linked)
				{
					if (_applyToAllImageSets)
					{
						foreach (IImageSet imageSet in parentImageSet.ParentLogicalWorkspace.ImageSets)
						{
							foreach (IDisplaySet displaySet in imageSet.LinkedDisplaySets)
							{
								foreach (IPresentationImage image in GetAllLinkedImages(displaySet))
									yield return image;
							}
						}
					}
					else
					{
						foreach (IDisplaySet currentDisplaySet in parentImageSet.LinkedDisplaySets)
						{
							foreach (IPresentationImage image in GetAllLinkedImages(currentDisplaySet))
								yield return image;
						}
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
					if (image != _referenceImage)
						yield return image;
				}
			}

			private IEnumerable<IPresentationImage> GetImages()
			{
				if (!_excludeReferenceImage)
					yield return _referenceImage;

				foreach (IPresentationImage image in this.GetAllLinkedImages())
					yield return image;
			}

			#region IEnumerable<IPresentationImage> Members

			public IEnumerator<IPresentationImage> GetEnumerator()
			{
				return GetImages().GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetImages().GetEnumerator();
			}

			#endregion
		}

		#endregion

		private readonly UndoableOperationApplicator<IPresentationImage> _applicator;
		private readonly LinkedImageEnumerator _imageEnumerator;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceImage">The 'current' (or reference) <see cref="IPresentationImage"/>.</param>
		/// <param name="operation">The operation to be performed on the current <see cref="IPresentationImage"/> and/or its linked images.</param>
		public ImageOperationApplicator(IPresentationImage referenceImage, IUndoableOperation<IPresentationImage> operation)
		{
			Platform.CheckForNullReference(referenceImage, "referenceImage");
			Platform.CheckForNullReference(operation, "operation");

			_imageEnumerator = new LinkedImageEnumerator(referenceImage);

			_applicator = new UndoableOperationApplicator<IPresentationImage>(operation, _imageEnumerator);
			_applicator.AppliedOperation += RedrawImage;
			_applicator.ItemMementoSet += RedrawImage;
		}

		private static void RedrawImage(object sender, ItemEventArgs<IPresentationImage> e)
		{
			e.Item.Draw();
		}

		/// <summary>
		/// Gets or sets whether the operation should be applied to all <see cref="IImageSet"/>s.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When this value is true, the operation will be applied to all <see cref="IImageSet"/>s with linked <see cref="IDisplaySet"/>s.  
		/// When false, the operation will only be applied to the current <see cref="IImageSet"/>'s linked <see cref="IDisplaySet"/>s 
		/// (determined from the current <see cref="IPresentationImage"/>).
		/// </para>
		/// <para>
		/// When the current <see cref="IDisplaySet"/> is not linked, the operation is only applied to the current <see cref="IDisplaySet"/>.
		/// </para>
		/// <para>
		/// The default value is false.
		/// </para>
		/// </remarks>
		public bool ApplyToAllImageSets
		{
			get { return _imageEnumerator.ApplyToAllImageSets; }
			set { _imageEnumerator.ApplyToAllImageSets = value; }
		}

		/// <summary>
		/// Applies the same <see cref="IUndoableOperation{T}"/> to the current image as well as all its linked images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IUndoableOperation{T}.Apply"/> will be called only for images where 
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> has returned true <b>and</b> <see cref="IUndoableOperation{T}.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// Each affected image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public void ApplyToAllImages()
		{
			_imageEnumerator.ExcludeReferenceImage = false;
			_applicator.Apply();
		}

		/// <summary>
		/// Applies the same <see cref="IUndoableOperation{T}"/> to all linked images, but not the current image itself.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IUndoableOperation{T}.Apply"/> will be called only for images where 
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> has returned true <b>and</b> <see cref="IUndoableOperation{T}.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// Each affected image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public void ApplyToLinkedImages()
		{
			_imageEnumerator.ExcludeReferenceImage = true;
			_applicator.Apply();
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			_imageEnumerator.ExcludeReferenceImage = false;
			return _applicator.CreateMemento();
		}

		public void SetMemento(object memento)
		{
			_applicator.SetMemento(memento);
		}

		#endregion
	}
}
