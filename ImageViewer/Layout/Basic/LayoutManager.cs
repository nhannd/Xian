using System;
using System.Collections;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;
		private bool _physicalWorkspaceLayoutSet = false;

		// Constructor
		public LayoutManager()
		{	
		}

		#region IDiagnosticLayoutManager Members

		public void SetImageViewer(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
			_imageViewer.EventBroker.StudyLoaded += new EventHandler<StudyEventArgs>(OnStudyLoaded);
			_imageViewer.EventBroker.ImageLoaded += new EventHandler<SopEventArgs>(OnImageLoaded);
		}

		public void Layout()
		{
			LayoutPhysicalWorkspace(_imageViewer.PhysicalWorkspace);
			FillPhysicalWorkspace(_imageViewer.PhysicalWorkspace, _imageViewer.LogicalWorkspace);
			_imageViewer.PhysicalWorkspace.Draw();
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		#endregion

		private void OnStudyLoaded(object sender, StudyEventArgs e)
		{
			AddStudy(e.Study.StudyInstanceUID);
		}

		void OnImageLoaded(object sender, SopEventArgs e)
		{
			AddImage(e.Sop.SopInstanceUID);
		}

		private void AddStudy(string studyInstanceUID)
		{
			Study study = _imageViewer.StudyTree.GetStudy(studyInstanceUID);

			// Abort if study hasn't been loaded
			if (study == null)
				throw new ApplicationException(String.Format(SR.ExceptionStudyNotLoaded, studyInstanceUID));

			IImageSet imageSet = GetImageSet(_imageViewer.LogicalWorkspace, studyInstanceUID);

			// Abort if image set has already been added
			if (imageSet != null)
				return;

			imageSet = AddImageSet(_imageViewer.LogicalWorkspace, study);
			_imageViewer.LogicalWorkspace.ImageSets.Sort(new ImageSetSortByStudyDate());

			AddDisplaySets(imageSet, study);
		}

		private void AddSeries(string seriesInstanceUID)
		{
			Series series = _imageViewer.StudyTree.GetSeries(seriesInstanceUID);

			if (series == null)
				throw new ApplicationException(String.Format(SR.ExceptionSeriesNotLoaded, seriesInstanceUID));

			string studyInstanceUID = series.ParentStudy.StudyInstanceUID;
			IImageSet imageSet = GetImageSet(_imageViewer.LogicalWorkspace, studyInstanceUID);

			// If image set hasn't been added, add it
			if (imageSet == null)
				imageSet = AddImageSet(_imageViewer.LogicalWorkspace, series.ParentStudy);

			IDisplaySet displaySet = AddDisplaySet(imageSet, series);
			AddImages(displaySet, series);
		}

		private void AddImage(string sopInstanceUID)
		{
			ImageSop image = _imageViewer.StudyTree.GetSop(sopInstanceUID) as ImageSop;

			if (image == null)
				throw new ApplicationException(String.Format(SR.ExceptionImageNotLoaded, sopInstanceUID));

			string studyInstanceUID = image.ParentSeries.ParentStudy.StudyInstanceUID;
			IImageSet imageSet = GetImageSet(_imageViewer.LogicalWorkspace, studyInstanceUID);

			if (imageSet == null)
				imageSet = AddImageSet(_imageViewer.LogicalWorkspace, image.ParentSeries.ParentStudy);

			string seriesInstanceUID = image.ParentSeries.SeriesInstanceUID;
			IDisplaySet displaySet = GetDisplaySet(imageSet, seriesInstanceUID);

			if (displaySet == null)
				displaySet = AddDisplaySet(imageSet, image.ParentSeries);

			AddImage(displaySet, image);
		}

		private IImageSet AddImageSet(ILogicalWorkspace logicalWorkspace, Study study)
		{
			IImageSet imageSet = new ImageSet();

			DateTime studyDate;
			DateParser.Parse(study.StudyDate, out studyDate);

			imageSet.Name = String.Format("{0} · {1}", 
				studyDate.ToString(Format.DateFormat), 
				study.StudyDescription);

			imageSet.PatientInfo = String.Format("{0} · {1}", 
				study.ParentPatient.PatientsName,
				study.ParentPatient.PatientId);

			imageSet.Uid = study.StudyInstanceUID;

			logicalWorkspace.ImageSets.Add(imageSet);

			return imageSet;
		}

		private void AddDisplaySets(IImageSet imageSet, Study study)
		{
			foreach (Series series in study.Series.Values)
			{
				IDisplaySet displaySet = AddDisplaySet(imageSet, series);
				AddImages(displaySet, series);
			}

			imageSet.DisplaySets.Sort(new DisplaySetSortBySeriesNumber());
		}

		private IDisplaySet AddDisplaySet(IImageSet imageSet, Series series)
		{
			DisplaySet displaySet = new DisplaySet();
			displaySet.Name = String.Format("{0}: {1}", series.SeriesNumber, series.SeriesDescription);
			displaySet.Uid = series.SeriesInstanceUID;

			imageSet.DisplaySets.Add(displaySet);

			return displaySet;
		}

		private void AddImages(IDisplaySet displaySet, Series series)
		{
			foreach (ImageSop image in series.Sops.Values)
				AddImage(displaySet, image);

			// This has been added so that the initial presentation of each display set has a reasonable 
			// sort order.  When proper sorting support is added, the sorters will be extensions.
			displaySet.PresentationImages.Sort(new PresentationImageSortByInstanceNumber());
		}

		private IPresentationImage AddImage(IDisplaySet displaySet, ImageSop image)
		{
			StandardPresentationImage presentationImage = new StandardPresentationImage(image);
			presentationImage.Uid = image.SopInstanceUID;
			displaySet.PresentationImages.Add(presentationImage);

			return presentationImage;
		}

		private IImageSet GetImageSet(ILogicalWorkspace logicalWorkspace, string studyInstanceUID)
		{
			foreach (IImageSet imageSet in logicalWorkspace.ImageSets)
			{
				if (imageSet.Uid == studyInstanceUID)
					return imageSet;
			}

			return null;
		}

		private IDisplaySet GetDisplaySet(IImageSet imageSet, string seriesInstanceUID)
		{
			foreach (IDisplaySet displaySet in imageSet.DisplaySets)
			{
				if (displaySet.Uid == seriesInstanceUID)
					return displaySet;
			}

			return null;
		}

		private void LayoutPhysicalWorkspace(IPhysicalWorkspace physicalWorkspace)
		{
			if (_physicalWorkspaceLayoutSet)
				return;

			_physicalWorkspaceLayoutSet = true;

			StoredLayoutConfiguration configuration = null;

			//take the first opened study, enumerate the modalities and compute the union of the layout configuration (in case there are multiple modalities).
			if (physicalWorkspace.LogicalWorkspace.ImageSets.Count > 0)
			{
				IImageSet firstImageSet = physicalWorkspace.LogicalWorkspace.ImageSets[0];
				foreach(IDisplaySet displaySet in firstImageSet.DisplaySets)
				{
					if (displaySet.PresentationImages.Count <= 0)
						continue;

					if (configuration == null)
						configuration = new StoredLayoutConfiguration();

					StoredLayoutConfiguration storedConfiguration = LayoutConfigurationSettings.Default.GetLayoutConfiguration(displaySet.PresentationImages[0] as IImageSopProvider);
					configuration.ImageBoxRows = Math.Max(configuration.ImageBoxRows, storedConfiguration.ImageBoxRows);
					configuration.ImageBoxColumns = Math.Max(configuration.ImageBoxColumns, storedConfiguration.ImageBoxColumns);
					configuration.TileRows = Math.Max(configuration.TileRows, storedConfiguration.TileRows);
					configuration.TileColumns = Math.Max(configuration.TileColumns, storedConfiguration.TileColumns);
				}
			}

			if (configuration == null)
				configuration = LayoutConfigurationSettings.Default.DefaultConfiguration;

			physicalWorkspace.SetImageBoxGrid(configuration.ImageBoxRows, configuration.ImageBoxColumns);
			for (int i = 0; i < physicalWorkspace.ImageBoxes.Count; ++i)
				physicalWorkspace.ImageBoxes[i].SetTileGrid(configuration.TileRows, configuration.TileColumns);
		}

		static public void FillPhysicalWorkspace(IPhysicalWorkspace physicalWorkspace, ILogicalWorkspace logicalWorkspace)
		{
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

				imageBox.DisplaySet = logicalWorkspace.ImageSets[imageSetIndex].DisplaySets[displaySetIndex];
				displaySetIndex++;
			}
		}
	}
}
