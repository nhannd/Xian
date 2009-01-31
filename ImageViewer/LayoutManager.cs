
using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Abstract base class for <see cref="ILayoutManager"/>s.
	/// </summary>
	public class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;
		private PresentationImageFactory _presentationImageFactory;

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

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer.PhysicalWorkspace"/> property.
		/// </summary>
		protected IPhysicalWorkspace PhysicalWorkspace
		{
			get { return _imageViewer.PhysicalWorkspace; }	
		}

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer.LogicalWorkspace"/> property.
		/// </summary>
		protected ILogicalWorkspace LogicalWorkspace
		{
			get { return _imageViewer.LogicalWorkspace; }
		}

		protected PresentationImageFactory PresentationImageFactory
		{
			get
			{
				if (_presentationImageFactory == null)
					_presentationImageFactory = new PresentationImageFactory(ImageViewer.StudyTree);

				return _presentationImageFactory;
			}
			set { _presentationImageFactory = value; }
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
		/// Internally, this method calls <see cref="BuildLogicalWorkspace"/>, <see cref="LayoutPhysicalWorkspace"/>, <see cref="SortDisplaySets"/>,
		/// <see cref="FillPhysicalWorkspace"/> and <see cref="SortImageSets"/> in that order, followed by a call to <see cref="IDrawable.Draw">IPhysicalWorkspace.Draw</see>.
		/// You can override this method entirely, or you can override any of the 5 methods called by this method.
		/// </remarks>
		public virtual void Layout()
		{
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
		}

		#endregion

		#region Protected Virtual Methods

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
			{
				imageSet.DisplaySets.Sort(GetDisplaySetComparer());

				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
					displaySet.PresentationImages.Sort(GetPresentationImageComparer());
			}
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

		#region Comparer Factory Methods

		/// <summary>
		/// Gets an <see cref="IComparer{T}"/> with which to sort <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <remarks>
		/// <para>The base implementation of this method returns an ascending <see cref="SeriesNumberComparer"/>.</para>
		/// <para>Subclasses may choose to override this method to provide any <see cref="IComparer{T}"/> of <see cref="IDisplaySet"/>s.</para>
		/// <para>This method is called by the base implementation of <see cref="SortDisplaySets"/>.</para>
		/// </remarks>
		/// <returns>The <see cref="IComparer{T}"/> with which to sort <see cref="IDisplaySet"/>s.</returns>
		/// <seealso cref="SortDisplaySets"/>
		protected virtual IComparer<IDisplaySet> GetDisplaySetComparer()
		{
			return new DefaultDisplaySetComparer();
		}

		/// <summary>
		/// Gets an <see cref="IComparer{T}"/> with which to sort <see cref="IPresentationImage"/>s.
		/// </summary>
		/// <remarks>
		/// <para>The base implementation of this method returns an ascending <see cref="InstanceAndFrameNumberComparer"/>.</para>
		/// <para>Subclasses may choose to override this method to provide any <see cref="IComparer{T}"/> of <see cref="IPresentationImage"/>s.</para>
		/// <para>This method is called by the base implementation of <see cref="SortDisplaySets"/>.</para>
		/// </remarks>
		/// <returns>The <see cref="IComparer{T}"/> with which to sort <see cref="IPresentationImage"/>s.</returns>
		/// <seealso cref="SortDisplaySets"/>
		protected virtual IComparer<IPresentationImage> GetPresentationImageComparer()
		{
			return new InstanceAndFrameNumberComparer();
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
			return new StudyDateComparer();
		}

		#endregion
		#endregion

		#region Logical Workspace Building Methods

		private void BuildFromStudy(Study study)
		{
			Platform.CheckForNullReference(study, "study");

			IImageSet imageSet = GetImageSet(study.StudyInstanceUID);

			// Abort if image set has already been added
			if (imageSet != null)
				return;

			AddImageSet(study);
		}

		private void AddImageSet(Study study)
		{
			IImageSet imageSet = null;
			foreach (Series series in study.Series)
			{
				IDisplaySet displaySet = null;
				foreach (Sop sop in series.Sops)
				{
					List<IPresentationImage> images = PresentationImageFactory.CreateImages(sop);
					if (images.Count > 0)
					{
						if (imageSet == null)
						{
							imageSet = CreateImageSet(study);
							LogicalWorkspace.ImageSets.Add(imageSet);
						}

						if (displaySet == null)
						{
							displaySet = CreateDisplaySet(series);
							imageSet.DisplaySets.Add(displaySet);
						}

						foreach (IPresentationImage image in images)
						{
							image.Uid = sop.SopInstanceUID;
							displaySet.PresentationImages.Add(image);
						}
					}
				}
			}
		}

		private IImageSet CreateImageSet(Study study)
		{
			ImageSet imageSet = new ImageSet();

			DateTime studyDate;
			DateParser.Parse(study.StudyDate, out studyDate);

			string modalitiesInStudy = StringUtilities.Combine(GetModalitiesInStudy(study), ", ");

			imageSet.Name = String.Format("{0} [{1}] {2}",
				studyDate.ToString(Format.DateFormat),
				modalitiesInStudy ?? "",
				study.StudyDescription);

			imageSet.PatientInfo = String.Format("{0} · {1}",
				study.ParentPatient.PatientsName.FormattedName,
				study.ParentPatient.PatientId);

			imageSet.Uid = study.StudyInstanceUID;
			return imageSet;
		}

		private IDisplaySet CreateDisplaySet(Series series)
		{
			string name = String.Format("{0}: {1}", series.SeriesNumber, series.SeriesDescription);
			DisplaySet displaySet = new DisplaySet(name, series.SeriesInstanceUID, series.SeriesDescription);
			displaySet.DefaultSortNumber = series.SeriesNumber;
			return displaySet;
		}

		#endregion

		#region Helper Methods

		private IImageSet GetImageSet(string studyInstanceUID)
		{
			foreach (IImageSet imageSet in LogicalWorkspace.ImageSets)
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

		private static List<string> GetModalitiesInStudy(Study study)
		{
			List<string> modalities = new List<string>();
			foreach (Series series in study.Series)
			{
				string modality = series.Modality;
				if (!modalities.Contains(modality))
					modalities.Add(modality);
			}

			return modalities;
		}

		#endregion

		#region Disposal

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
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
