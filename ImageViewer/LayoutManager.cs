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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A base implementation of <see cref="ILayoutManager"/>.
	/// </summary>
	public class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;
		private bool _layoutCompleted;
		private bool _allowEmptyViewer;

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

		protected bool AllowEmptyViewer
		{
			get { return _allowEmptyViewer; }
			set { _allowEmptyViewer = value; }
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
		/// <see cref="FillPhysicalWorkspace"/> and <see cref="SortImageSets()"/> in that order, followed by a call to <see cref="IDrawable.Draw">IPhysicalWorkspace.Draw</see>.
		/// You can override this method entirely, or you can override any of the 5 methods called by this method.
		/// </remarks>
		public virtual void Layout()
		{
			if (_layoutCompleted)
				throw new InvalidOperationException("Layout has already been called.");

			BuildLogicalWorkspace();
			ValidateLogicalWorkspace();
			LayoutPhysicalWorkspace();
			FillPhysicalWorkspace();
			
			// Now, only after showing the "primary study", sort the image sets according to study date.
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

			if (!_allowEmptyViewer)
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

		protected virtual DicomImageSetDescriptor CreateImageSetDescriptor(IStudyRootStudyIdentifier studyData)
		{
			return new DicomImageSetDescriptor(studyData);
		}

		protected virtual IImageSet CreateImageSet(IStudyRootStudyIdentifier studyData)
		{
			return new ImageSet(CreateImageSetDescriptor(studyData));
		}

		protected virtual void UpdateImageSet(IImageSet imageSet, Series series)
		{
			foreach (IDisplaySet displaySet in BasicDisplaySetFactory.CreateSeriesDisplaySets(series, StudyTree))
				imageSet.DisplaySets.Add(displaySet);
		}

		protected virtual void SortSops(SopCollection sops)
		{
			sops.Sort(GetSopComparer());
		}

		protected virtual IComparer<Sop> GetSopComparer()
		{
			return new InstanceNumberComparer();
		}

		protected virtual void SortSeries(SeriesCollection series)
		{
			series.Sort(GetSeriesComparer());
		}

		protected virtual IComparer<Series> GetSeriesComparer()
		{
			return new SeriesNumberComparer();
		}

		protected virtual void SortStudies(StudyCollection studies)
		{
			studies.Sort(GetStudyComparer());
		}

		protected virtual IComparer<Study> GetStudyComparer()
		{
			return new StudyDateComparer();
		}

		#endregion

		#endregion

		#region Logical Workspace Building Methods

		private void BuildFromStudy(Study study)
		{
			IImageSet imageSet = GetImageSet(study.StudyInstanceUid);
			// Abort if image set has already been added
			if (imageSet != null)
				return;

			imageSet = CreateImageSet(study.GetIdentifier());
			if (imageSet.Uid != study.StudyInstanceUid)
				throw new InvalidOperationException("ImageSet Uid must be the same as Study Instance Uid.");

			SortSeries(study.Series);

			foreach (Series series in study.Series)
			{
				SortSops(series.Sops);
				UpdateImageSet(imageSet, series);
			}

			if (imageSet.DisplaySets.Count == 0)
				imageSet.Dispose();
			else
				AddImageSet(imageSet);
		}

		private void AddImageSet(IImageSet imageSet)
		{
			int insertIndex = LogicalWorkspace.ImageSets.Count;
			if (_layoutCompleted)
			{
				//A bit cheap, but once the initial layout is done, we need to keep everything sorted.
				ObservableList<IImageSet> sortedImageSets = new ObservableList<IImageSet>();
				foreach(IImageSet set in LogicalWorkspace.ImageSets)
					sortedImageSets.Add(set);

				sortedImageSets.Add(imageSet);
				SortImageSets(sortedImageSets);
				insertIndex = sortedImageSets.IndexOf(imageSet);
			}

			LogicalWorkspace.ImageSets.Insert(insertIndex, imageSet);
		}

		private void SortImageSets()
		{
			SortImageSets(LogicalWorkspace.ImageSets);
		}

		private void SortImageSets(ObservableList<IImageSet> imageSets)
		{
			SortImageSets(imageSets, GetAllStudiesSorted());
		}

		internal static void SortImageSets(ObservableList<IImageSet> imageSets, IList<Study> studies)
		{
			imageSets.Sort(new ImageSetComparer(studies));
		}

		#endregion

		#region Helper Methods

		private IImageSet GetImageSet(string studyInstanceUid)
		{
			foreach (ImageSet imageSet in LogicalWorkspace.ImageSets)
			{
				if (imageSet.Uid == studyInstanceUid)
					return imageSet;
			}

			return null;
		}

		private StudyCollection GetAllStudiesSorted()
		{
			StudyCollection studies = new StudyCollection();

			foreach (Patient patient in StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
					studies.Add(study);
			}

			SortStudies(studies);
			return studies;
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

		#region Disposal

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _imageViewer != null)
			{
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

		private class ImageSetComparer : IComparer<IImageSet>
		{
			private readonly IList<Study> _studies;

			public ImageSetComparer(IList<Study> studies)
			{
				_studies = studies;
			}

			#region IComparer<IImageSet> Members

			public int Compare(IImageSet x, IImageSet y)
			{
				int index1 = IndexOfStudy(x.Uid);
				int index2 = IndexOfStudy(y.Uid);

				if (index1 < index2)
					return -1;
				
				if (index1 > index2)
					return 1;

				return 0;
			}

			private int IndexOfStudy(string studyInstanceUid)
			{
				int i = 0;
				foreach (Study study in _studies)
				{
					if (study.StudyInstanceUid == studyInstanceUid)
						return i;

					++i;
				}

				return -1;
			}

			#endregion
		}
	}
}
