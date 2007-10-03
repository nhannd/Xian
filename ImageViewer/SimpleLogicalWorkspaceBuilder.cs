using System;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A simple algorithm for building the <see cref="LogicalWorkspace"/>.
	/// </summary>
	/// <remarks>
	/// Adds <see cref="IImageSet"/>s, <see cref="IDisplaySet"/>s and 
	/// <see cref="IPresentationImage"/>s to the <see cref="ILogicalWorkspace"/>.
	/// In this simple implementation, an <see cref="IImageSet"/> corresponds to
	/// a <see cref="Study"/>, an <see cref="IDisplaySet"/> to a <see cref="Series"/>
	/// and an <see cref="ImageSop"/> to an <see cref="IPresentationImage"/>.
	/// </remarks>
	public static class SimpleLogicalWorkspaceBuilder
	{
		#region Public methods

		/// <summary>
		/// Builds the <see cref="ILogicalWorkspace"/> based on the entire
		/// contents of the <see cref="StudyTree"/>.
		/// </summary>
		/// <param name="imageViewer"></param>
		public static void Build(IImageViewer imageViewer)
		{
			foreach (Patient patient in imageViewer.StudyTree.Patients.Values)
			{
				foreach (Study study in patient.Studies.Values)
				{
					BuildFromStudy(imageViewer, study);
				}
			}
		}

		#endregion

		#region Private methods

		private static void BuildFromStudy(IImageViewer imageViewer, string studyInstanceUID)
		{
			Study study = imageViewer.StudyTree.GetStudy(studyInstanceUID);
			BuildFromStudy(imageViewer, study);
		}

		private static void BuildFromStudy(IImageViewer imageViewer, Study study)
		{
			// Abort if study hasn't been loaded
			if (study == null)
				throw new ApplicationException(string.Format("Study {0} has not been loaded", study.StudyInstanceUID));

			IImageSet imageSet = GetImageSet(imageViewer.LogicalWorkspace, study.StudyInstanceUID);

			// Abort if image set has already been added
			if (imageSet != null)
				return;

			imageSet = AddImageSet(imageViewer.LogicalWorkspace, study);
			imageViewer.LogicalWorkspace.ImageSets.Sort(new StudyDateComparer());

			AddDisplaySets(imageSet, study);
		}

		private static void BuildFromSeries(IImageViewer imageViewer, string seriesInstanceUID)
		{
			Series series = imageViewer.StudyTree.GetSeries(seriesInstanceUID);
			BuildFromSeries(imageViewer, series);			
		}

		private static void BuildFromSeries(IImageViewer imageViewer, Series series)
		{
			if (series == null)
				throw new ApplicationException(string.Format("Series {0} has not been loaded", series.SeriesInstanceUID));

			string studyInstanceUID = series.ParentStudy.StudyInstanceUID;
			IImageSet imageSet = GetImageSet(imageViewer.LogicalWorkspace, studyInstanceUID);

			// If image set hasn't been added, add it
			if (imageSet == null)
				imageSet = AddImageSet(imageViewer.LogicalWorkspace, series.ParentStudy);

			IDisplaySet displaySet = AddDisplaySet(imageSet, series);
			AddImages(displaySet, series);
		}

		private static void BuildFromImage(IImageViewer imageViewer, string sopInstanceUID)
		{
			ImageSop image = imageViewer.StudyTree.GetSop(sopInstanceUID) as ImageSop;
			BuildFromImage(imageViewer, image);
		}

		private static void BuildFromImage(IImageViewer imageViewer, ImageSop image)
		{
			bool sortImageSets = false;
			bool sortDisplaySets = false;

			if (image == null)
				throw new ApplicationException(string.Format("Image {0} has not been loaded", image.SopInstanceUID));

			string studyInstanceUID = image.ParentSeries.ParentStudy.StudyInstanceUID;
			IImageSet imageSet = GetImageSet(imageViewer.LogicalWorkspace, studyInstanceUID);

			if (imageSet == null)
			{
				imageSet = AddImageSet(imageViewer.LogicalWorkspace, image.ParentSeries.ParentStudy);
				sortImageSets = true;
			}

			string seriesInstanceUID = image.ParentSeries.SeriesInstanceUID;
			IDisplaySet displaySet = GetDisplaySet(imageSet, seriesInstanceUID);

			if (displaySet == null)
			{
				displaySet = AddDisplaySet(imageSet, image.ParentSeries);
				sortDisplaySets = true;
			}

			AddImage(displaySet, image);

			// Yes, all this sorting as each image is loaded is terribly inefficient,
			// but seeing that this is not that common an operation, we'll live with
			// it for now.
			if (sortImageSets)
				imageViewer.LogicalWorkspace.ImageSets.Sort(new StudyDateComparer());

			if (sortDisplaySets)
				imageSet.DisplaySets.Sort(new SeriesNumberComparer());

			SortImages(displaySet);
		}

		private static IImageSet AddImageSet(ILogicalWorkspace logicalWorkspace, Study study)
		{
			IImageSet imageSet = new ImageSet();

			DateTime studyDate;
			DateParser.Parse(study.StudyDate, out studyDate);

			imageSet.Name = String.Format("{0} · {1}",
				studyDate.ToString(Format.DateFormat),
				study.StudyDescription);

			imageSet.PatientInfo = String.Format("{0} · {1}",
				study.ParentPatient.PatientsName.FormattedName,
				study.ParentPatient.PatientId);

			imageSet.Uid = study.StudyInstanceUID;

			logicalWorkspace.ImageSets.Add(imageSet);

			return imageSet;
		}

		private static void AddDisplaySets(IImageSet imageSet, Study study)
		{
			foreach (Series series in study.Series.Values)
			{
				IDisplaySet displaySet = AddDisplaySet(imageSet, series);
				AddImages(displaySet, series);
			}

			imageSet.DisplaySets.Sort(new SeriesNumberComparer());
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
			foreach (ImageSop image in series.Sops.Values)
				AddImage(displaySet, image);

			SortImages(displaySet);
		}

		private static void SortImages(IDisplaySet displaySet)
		{
			// This has been added so that the initial presentation of each display set has a reasonable 
			// sort order.  When proper sorting support is added, the sorters will be extensions.
			displaySet.PresentationImages.Sort(new InstanceNumberComparer());
		}

		private static IPresentationImage AddImage(IDisplaySet displaySet, ImageSop image)
		{
			IPresentationImage presentationImage = PresentationImageFactory.Create(image);
			presentationImage.Uid = image.SopInstanceUID;
			displaySet.PresentationImages.Add(presentationImage);

			return presentationImage;
		}

		private static IImageSet GetImageSet(ILogicalWorkspace logicalWorkspace, string studyInstanceUID)
		{
			foreach (IImageSet imageSet in logicalWorkspace.ImageSets)
			{
				if (imageSet.Uid == studyInstanceUID)
					return imageSet;
			}

			return null;
		}

		private static IDisplaySet GetDisplaySet(IImageSet imageSet, string seriesInstanceUID)
		{
			foreach (IDisplaySet displaySet in imageSet.DisplaySets)
			{
				if (displaySet.Uid == seriesInstanceUID)
					return displaySet;
			}

			return null;
		}

		#endregion
	}
}
