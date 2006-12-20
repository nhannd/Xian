using System;
using System.Collections;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;
		private bool _physicalWorkspaceCreated = false;

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
			CreatePhysicalWorkspaceTree(_imageViewer.PhysicalWorkspace);
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
			imageSet.Tag = study.StudyInstanceUID;

			//imageSet.Name = study.StudyDate;
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
		}

		private IDisplaySet AddDisplaySet(IImageSet imageSet, Series series)
		{
			DisplaySet displaySet = new DisplaySet();
			displaySet.Name = series.SeriesDescription;
			displaySet.Tag = series.SeriesInstanceUID;

			imageSet.DisplaySets.Add(displaySet);

			return displaySet;
		}

		private void AddImages(IDisplaySet displaySet, Series series)
		{
			foreach (ImageSop image in series.Sops.Values)
				AddImage(displaySet, image);
		}

		private IPresentationImage AddImage(IDisplaySet displaySet, ImageSop image)
		{
			DicomPresentationImage presentationImage = new DicomPresentationImage(image);
			presentationImage.Tag = image.SopInstanceUID;
			displaySet.PresentationImages.Add(presentationImage);

			// This has been added so that the initial presentation of each display set has a reasonable 
			// sort order.  When proper sorting support is added, the sorters will be extensions.
			displaySet.PresentationImages.Sort((IComparer<IPresentationImage>)new DicomPresentationImageSortByInstanceNumber());

			return presentationImage;
		}

		private IImageSet GetImageSet(ILogicalWorkspace logicalWorkspace, string studyInstanceUID)
		{
			foreach (IImageSet imageSet in logicalWorkspace.ImageSets)
			{
				if (imageSet.Tag.ToString() == studyInstanceUID)
					return imageSet;
			}

			return null;
		}

		private IDisplaySet GetDisplaySet(IImageSet imageSet, string seriesInstanceUID)
		{
			foreach (IDisplaySet displaySet in imageSet.DisplaySets)
			{
				if (displaySet.Tag.ToString() == seriesInstanceUID)
					return displaySet;
			}

			return null;
		}

		private void CreatePhysicalWorkspaceTree(IPhysicalWorkspace physicalWorkspace)
		{
			if (_physicalWorkspaceCreated)
				return;

			//physicalWorkspace.SetImageBoxGrid(1, 1);
			physicalWorkspace.SetImageBoxGrid(1, 2);

			//foreach (ImageBox imageBox in physicalWorkspace)
			//{
			//    CreateTileGrid(imageBox, 2, 2);
			//}

			//physicalWorkspace[0].SetTileGrid(2,2);
			physicalWorkspace.ImageBoxes[0].SetTileGrid(1,1);
			physicalWorkspace.ImageBoxes[1].SetTileGrid(1,1);
			//physicalWorkspace[2].SetTileGrid(3,4);
			
			_physicalWorkspaceCreated = true;
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
