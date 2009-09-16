#region License

// Copyright (c) 2009, ClearCanvas Inc.
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


using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A base implementation of <see cref="ILayoutManager"/>.
	/// </summary>
	public class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;
		private bool _layoutCompleted;

		/// <summary>
		/// Constructor.
		/// </summary>
		public LayoutManager()
		{
		}

		#region Protected Properties

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer"/>.
		/// </summary>
		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		protected StudyTree StudyTree
		{
			get 
			{
				if (_imageViewer != null)
					return _imageViewer.StudyTree;
				else
					return null;
			}	
		}

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer.PhysicalWorkspace"/> property.
		/// </summary>
		protected IPhysicalWorkspace PhysicalWorkspace
		{
			get
			{
				if (_imageViewer != null)
					return _imageViewer.PhysicalWorkspace;
				else
					return null;
			}	
		}

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer.LogicalWorkspace"/> property.
		/// </summary>
		protected ILogicalWorkspace LogicalWorkspace
		{
			get
			{
				if (_imageViewer != null)
					return _imageViewer.LogicalWorkspace;
				else
					return null;
			}
		}

		#endregion

		#region ILayoutManager Members

		/// <summary>
		/// Sets the owning <see cref="IImageViewer"/>.
		/// </summary>
		public virtual void SetImageViewer(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		/// <summary>
		/// Builds the <see cref="ILogicalWorkspace"/>, lays out and fills the <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// Internally, this method calls <see cref="BuildLogicalWorkspace"/>, <see cref="LayoutPhysicalWorkspace"/>, <see cref="SortDisplaySets()"/>,
		/// <see cref="FillPhysicalWorkspace"/> and <see cref="SortImageSets"/> in that order, followed by a call to <see cref="IDrawable.Draw">IPhysicalWorkspace.Draw</see>.
		/// You can override this method entirely, or you can override any of the 5 methods called by this method.
		/// </remarks>
		public virtual void Layout()
		{
			if (_layoutCompleted)
				throw new InvalidOperationException("Layout has already been called.");

			BuildLogicalWorkspace();
			ValidateLogicalWorkspace();
			LayoutPhysicalWorkspace();
			
			// Sort the display sets before filling the physical workspace, so that
			// the order in which the display sets are laid out matches the order
			// of the studies in the study tree.

			SortDisplaySets();
			FillPhysicalWorkspace();
			
			// Now, sort the image sets according to study date.
			SortImageSets();

			ImageViewer.PhysicalWorkspace.Draw();
			OnLayoutCompleted();
		}

		/// <summary>
		/// Called from <see cref="Layout"/> to signal that the layout is complete.
		/// </summary>
		protected virtual void OnLayoutCompleted()
		{
			_layoutCompleted = true;
			ImageViewer.EventBroker.StudyLoaded += OnPriorStudyLoaded;
			//NOTE: this event doesn't actually get fired right now, but we're doing this for completeness.
			ImageViewer.EventBroker.ImageLoaded += OnImageLoaded;
		}

		#endregion

		#region Protected Methods

		#region Virtual
		/// <summary>
		/// Builds the <see cref="ILogicalWorkspace"/>, creating and populating <see cref="ILogicalWorkspace.ImageSets"/>
		/// from the contents of <see cref="IImageViewer.StudyTree"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// By default, this method simply creates an <see cref="IDisplaySet"/> for each <see cref="Series"/>
		/// in <see cref="IImageViewer.StudyTree"/>.
		/// </para>
		/// <para>
		/// Override this method to change how the <see cref="IDisplaySet"/>s are constructed.
		/// </para>
		/// </remarks>
		protected virtual void BuildLogicalWorkspace()
		{
			foreach (Patient patient in ImageViewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					BuildFromStudy(study);
				}
			}
		}

		protected virtual DicomImageSetDescriptor CreateImageSetDescriptor(IStudyRootStudyIdentifier studyData)
		{
			return new DicomImageSetDescriptor(studyData);
		}

		protected virtual void UpdateImageSet(IImageSet imageSet, Series series)
		{
			foreach (IDisplaySet displaySet in BasicDisplaySetFactory.CreateSeriesDisplaySets(series, StudyTree))
				imageSet.DisplaySets.Add(displaySet);
		}

		protected virtual void UpdateDisplaySet(IDisplaySet displaySet, Sop sop)
		{
			if (displaySet.Descriptor is IDicomDisplaySetDescriptor)
			{
				if (((IDicomDisplaySetDescriptor)displaySet.Descriptor).Update(sop))
					displaySet.Draw();
			}
		}

		/// <summary>
		/// Validates the <see cref="ILogicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// A <see cref="NoVisibleDisplaySetsException"/> is thrown if no
		/// <see cref="IDisplaySet"/>s exist in the <see cref="ILogicalWorkspace"/>.
		/// </remarks>
		protected virtual void ValidateLogicalWorkspace()
		{
			foreach (IImageSet imageSet in _imageViewer.LogicalWorkspace.ImageSets)
			{
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
				{
					foreach (IPresentationImage image in displaySet.PresentationImages)
						return;
				}
			}

			throw new NoVisibleDisplaySetsException("The Layout operation has resulted in no images to be displayed.");
		}

		/// <summary>
		/// Lays out the physical workspace, adding and setting up the <see cref="IPhysicalWorkspace.ImageBoxes"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// By default, this method determines a simple layout based on the number of <see cref="IDisplaySet"/>s
		/// available (e.g. it tries to set the layout so all <see cref="IDisplaySet"/>s are visible).
		/// </para>
		/// <para>
		/// Typically, subclasses would override this method, at the very least.
		/// </para>
		/// </remarks>
		protected virtual void LayoutPhysicalWorkspace()
		{
			int numDisplaySets = GetNumberOfDisplaySets();

			if (numDisplaySets == 1)
				ImageViewer.PhysicalWorkspace.SetImageBoxGrid(1, 1);
			else if (numDisplaySets == 2)
				ImageViewer.PhysicalWorkspace.SetImageBoxGrid(1, 2);
			else if (numDisplaySets <= 4)
				ImageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 2);
			else if (numDisplaySets <= 8)
				ImageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 4);
			else if (numDisplaySets <= 12)
				ImageViewer.PhysicalWorkspace.SetImageBoxGrid(3, 4);
			else
				ImageViewer.PhysicalWorkspace.SetImageBoxGrid(4, 4);

			foreach (IImageBox imageBox in ImageViewer.PhysicalWorkspace.ImageBoxes)
				imageBox.SetTileGrid(1, 1);
		}

		/// <summary>
		/// Fills <see cref="IPhysicalWorkspace.ImageBoxes"/> with <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <remarks>
		/// <para>
		/// By default, <see cref="IPhysicalWorkspace.ImageBoxes"/> is filled starting with the first 
		/// <see cref="IImageSet"/>'s <see cref="IDisplaySet"/>, and continuing until there
		/// are no empty <see cref="IImageBox"/>es or all <see cref="IDisplaySet"/>s have been assigned to an <see cref="IImageBox"/>.
		/// </para>
		/// <para>
		/// Override this method to change how <see cref="IDisplaySet"/>s are assigned to <see cref="IPhysicalWorkspace.ImageBoxes"/>.
		/// </para>
		/// </remarks>
		protected virtual void FillPhysicalWorkspace()
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
			ILogicalWorkspace logicalWorkspace = ImageViewer.LogicalWorkspace;

			if (logicalWorkspace.ImageSets.Count == 0)
				return;

			int imageSetIndex = 0;
			int displaySetIndex = 0;

			foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
			{
				if (displaySetIndex == logicalWorkspace.ImageSets[imageSetIndex].DisplaySets.Count)
				{
					imageSetIndex++;
					displaySetIndex = 0;

					if (imageSetIndex == logicalWorkspace.ImageSets.Count)
						break;
				}

				imageBox.DisplaySet = logicalWorkspace.ImageSets[imageSetIndex].DisplaySets[displaySetIndex].CreateFreshCopy();
				displaySetIndex++;
			}
		}

		/// <summary>
		/// Called to sort the display sets.
		/// </summary>
		/// <remarks>
		/// <para>The base implmentation of this method sorts the display sets by the <see cref="IComparer{T}"/> supplied by <see cref="GetDisplaySetComparer"/>,
		/// and sorts each display set internally by the <see cref="IComparer{T}"/> supplied by <see cref="GetPresentationImageComparer"/>.</para>
		/// <para>Subclasses may choose to override just the supplied <see cref="IComparer{T}"/>s, or to override this method entirely.</para>
		/// <para>This method is called by the base implementation of <see cref="Layout"/>.</para>
		/// </remarks>
		/// <seealso cref="Layout"/>
		protected virtual void SortDisplaySets()
		{
			foreach (IImageSet imageSet in LogicalWorkspace.ImageSets)
				SortDisplaySets(imageSet.DisplaySets);
		}

		/// <summary>
		/// Called to sort the image sets.
		/// </summary>
		/// <remarks>
		/// <para>The base implementation of this method sorts the image sets by the <see cref="IComparer{T}"/> supplied by <see cref="GetImageSetComparer"/>.</para>
		/// <para>Subclasses may choose to override just the supplied <see cref="IComparer{T}"/>, or to override this method entirely.</para>
		/// <para>This method is called by the base implementation of <see cref="Layout"/>.</para>
		/// </remarks>
		/// <seealso cref="Layout"/>
		protected virtual void SortImageSets()
		{
			LogicalWorkspace.ImageSets.Sort(GetImageSetComparer());
		}

		#endregion

		/// <summary>
		/// Sorts the given <see cref="DisplaySetCollection"/>.
		/// </summary>
		/// <remarks>
		/// <para>By default, sorts the <see cref="IDisplaySet"/>s and their <see cref="IPresentationImage"/>s using
		/// the <see cref="IComparer{T}"/>s from <see cref="GetDisplaySetComparer"/> and <see cref="GetPresentationImageComparer"/>.</para>
		/// <para>Subclasses may choose to override just the supplied <see cref="IComparer{T}"/>, or to override this method entirely.</para>
		/// <para>This method is called by the base implementation of <see cref="Layout"/>.</para>
		/// </remarks>
		protected void SortDisplaySets(DisplaySetCollection displaySets)
		{
			displaySets.Sort(GetDisplaySetComparer());

			foreach (IDisplaySet displaySet in displaySets)
				SortImages(displaySet.PresentationImages);
		}

		/// <summary>
		/// Sorts the given <see cref="PresentationImageCollection"/>.
		/// </summary>
		/// <remarks>
		/// <para>By default, sorts the <see cref="IPresentationImage"/>s using the <see cref="IComparer{T}"/> from <see cref="GetPresentationImageComparer"/>.</para>
		/// <para>Subclasses may choose to override just the supplied <see cref="IComparer{T}"/>, or to override this method entirely.</para>
		/// <para>This method is called by the base implementation of <see cref="Layout"/>.</para>
		/// </remarks>
		protected void SortImages(PresentationImageCollection images)
		{
			images.Sort(GetPresentationImageComparer());
		}

		#region Comparer Factory Methods

		/// <summary>
		/// Gets an <see cref="IComparer{T}"/> with which to sort <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <remarks>
		/// <para>The base implementation of this method returns an ascending <see cref="SeriesNumberComparer"/>.</para>
		/// <para>Subclasses may choose to override this method to provide any <see cref="IComparer{T}"/> of <see cref="IDisplaySet"/>s.</para>
		/// <para>This method is called by the base implementation of <see cref="SortDisplaySets()"/>.</para>
		/// </remarks>
		/// <returns>The <see cref="IComparer{T}"/> with which to sort <see cref="IDisplaySet"/>s.</returns>
		/// <seealso cref="SortDisplaySets()"/>
		protected virtual IComparer<IDisplaySet> GetDisplaySetComparer()
		{
			return DisplaySetCollection.GetDefaultComparer();
		}

		/// <summary>
		/// Gets an <see cref="IComparer{T}"/> with which to sort <see cref="IPresentationImage"/>s.
		/// </summary>
		/// <remarks>
		/// <para>The base implementation of this method returns an ascending <see cref="InstanceAndFrameNumberComparer"/>.</para>
		/// <para>Subclasses may choose to override this method to provide any <see cref="IComparer{T}"/> of <see cref="IPresentationImage"/>s.</para>
		/// <para>This method is called by the base implementation of <see cref="SortDisplaySets()"/>.</para>
		/// </remarks>
		/// <returns>The <see cref="IComparer{T}"/> with which to sort <see cref="IPresentationImage"/>s.</returns>
		/// <seealso cref="SortDisplaySets()"/>
		protected virtual IComparer<IPresentationImage> GetPresentationImageComparer()
		{
			return PresentationImageCollection.GetDefaultComparer();
		}

		/// <summary>
		/// Gets an <see cref="IComparer{T}"/> with which to sort <see cref="IImageSet"/>s.
		/// </summary>
		/// <remarks>
		/// <para>The base implementation of this method returns an asecending <see cref="StudyDateComparer"/>.</para>
		/// <para>Subclasses may choose to override this method to provide any <see cref="IComparer{T}"/> of <see cref="IImageSet"/>s.</para>
		/// <para>This method is called by the base implementation of <see cref="SortImageSets"/>.</para>
		/// </remarks>
		/// <returns>The <see cref="IComparer{T}"/> with which to sort <see cref="IImageSet"/>s.</returns>
		/// <seealso cref="SortImageSets"/>
		protected virtual IComparer<IImageSet> GetImageSetComparer()
		{
			return ImageSetCollection.GetDefaultComparer();
		}

		#endregion
		#endregion

		#region Logical Workspace Building Methods

		private void BuildFromStudy(Study study)
		{
			ImageSet imageSet = GetImageSet(study.StudyInstanceUid);

			// Abort if image set has already been added
			if (imageSet != null)
				return;

			imageSet = CreateImageSet(study);

			if (imageSet != null)
				AddImageSet(imageSet);
		}

		private ImageSet CreateImageSet(Study study)
		{
			ImageSetDescriptor descriptor = CreateImageSetDescriptor(study.GetIdentifier());
			ImageSet imageSet = new ImageSet(descriptor);

			foreach (Series series in study.Series)
				UpdateImageSet(imageSet, series);

			if (imageSet.DisplaySets.Count == 0)
			{
				imageSet.Dispose();
				imageSet = null;
			}

			return imageSet;
		}

		private void AddImageSet(IImageSet imageSet)
		{
			int insertIndex = LogicalWorkspace.ImageSets.Count;
			if (_layoutCompleted)
			{
				//A bit cheap, but once the initial layout is done, we need to keep everything sorted.
				List<IImageSet> sorted = new List<IImageSet>(LogicalWorkspace.ImageSets);
				sorted.Add(imageSet);
				sorted.Sort(LogicalWorkspace.ImageSets.Comparer);
				insertIndex = sorted.IndexOf(imageSet);
			}

			SortDisplaySets(imageSet.DisplaySets);

			LogicalWorkspace.ImageSets.Insert(insertIndex, imageSet);
		}

		#endregion

		#region Helper Methods

		private ImageSet GetImageSet(string studyInstanceUID)
		{
			foreach (ImageSet imageSet in LogicalWorkspace.ImageSets)
			{
				if (imageSet.Uid == studyInstanceUID)
					return imageSet;
			}

			return null;
		}

		private int GetNumberOfDisplaySets()
		{
			int count = 0;

			foreach (IImageSet imageSet in ImageViewer.LogicalWorkspace.ImageSets)
				count += imageSet.DisplaySets.Count;

			return count;
		}

		#endregion

		private void OnPriorStudyLoaded(object sender, StudyLoadedEventArgs args)
		{
			OnPriorStudyLoaded(args.Study);
		}

		private void OnPriorStudyLoaded(Study study)
		{
			BuildFromStudy(study);
		}

		private void OnImageLoaded(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Sop> e)
		{
			OnNewSopLoaded(e.Item);
		}

		private void OnNewSopLoaded(Sop sop)
		{
			ImageSet imageSet = GetImageSet(sop.StudyInstanceUid);
			if (imageSet == null)
			{
				imageSet = CreateImageSet(sop.ParentSeries.ParentStudy);
				if (imageSet != null)
					AddImageSet(imageSet);
			}
			else
			{
				//Update the originals.
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
					UpdateDisplaySet(displaySet, sop);

				//Update the copies.
				foreach (IDisplaySet displaySet in imageSet.GetCopies())
					UpdateDisplaySet(displaySet, sop);
			}
		}

		#region Disposal

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _imageViewer != null)
			{
				_imageViewer.EventBroker.ImageLoaded -= OnImageLoaded;
				_imageViewer.EventBroker.StudyLoaded -= OnPriorStudyLoaded;
				_imageViewer = null;
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		#endregion
		#endregion
	}
}
