
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
		/// Internally, this method calls <see cref="BuildLogicalWorkspace"/>, <see cref="LayoutPhysicalWorkspace"/>
		/// and <see cref="FillPhysicalWorkspace"/> in that order, followed by a call to <see cref="IPhysicalWorkspace.Draw"/>.
		/// You can override this method entirely, or you can override any of the 3 methods called by this method.
		/// </remarks>
		public virtual void Layout()
		{
			BuildLogicalWorkspace();
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
		
		#endregion

		#region Logical Workspace Building Methods

		private void BuildFromStudy(Study study)
		{
			Platform.CheckForNullReference(study, "study");

			IImageSet imageSet = GetImageSet(study.StudyInstanceUID);

			// Abort if image set has already been added
			if (imageSet != null)
				return;

			imageSet = AddImageSet(study);
			AddDisplaySets(imageSet, study);
		}

		private IImageSet AddImageSet(Study study)
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

			LogicalWorkspace.ImageSets.Add(imageSet);

			return imageSet;
		}

		#endregion

		#region Helper Methods

		private void SortDisplaySets()
		{
			//TODO: move to virtual OnSort
			foreach (IImageSet imageSet in LogicalWorkspace.ImageSets)
			{
				imageSet.DisplaySets.Sort(new SeriesNumberComparer());

				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
					displaySet.PresentationImages.Sort(new InstanceAndFrameNumberComparer());
			}
		}

		//TODO: use factory methods to get comparers.
		private void SortImageSets()
		{
			//TODO: move to virtual OnSort
			LogicalWorkspace.ImageSets.Sort(new StudyDateComparer());
		}

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

		#region Static Helpers

		private static void AddDisplaySets(IImageSet imageSet, Study study)
		{
			foreach (Series series in study.Series)
			{
				IDisplaySet displaySet = AddDisplaySet(imageSet, series);
				AddImages(displaySet, series);
			}
		}

		private static IDisplaySet AddDisplaySet(IImageSet imageSet, Series series)
		{
			string name = String.Format("{0}: {1}", series.SeriesNumber, series.SeriesDescription);
			DisplaySet displaySet = new DisplaySet(name, series.SeriesInstanceUID);

			imageSet.DisplaySets.Add(displaySet);

			return displaySet;
		}

		private static void AddImages(IDisplaySet displaySet, Series series)
		{
			foreach (ImageSop image in series.Sops)
				AddImages(displaySet, image);
		}

		private static void AddImages(IDisplaySet displaySet, ImageSop imageSop)
		{
			foreach (IPresentationImage image in PresentationImageFactory.Create(imageSop))
			{
				image.Uid = imageSop.SopInstanceUID;
				displaySet.PresentationImages.Add(image);
			}
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
