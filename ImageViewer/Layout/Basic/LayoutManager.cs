using System;
using System.Collections;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.LayoutManagerExtensionPoint))]
	public class LayoutManager : ILayoutManager
	{
		// Private attributes

		// Constructor
		public LayoutManager()
		{	
		}

		#region ILayoutManager methods

		public void ApplyLayout(
			ILogicalWorkspace logicalWorkspace, 
			IPhysicalWorkspace physicalWorkspace,
			string studyInstanceUID)
		{
			CreateLogicalWorkspaceTree(logicalWorkspace, studyInstanceUID);
			CreatePhysicalWorkspaceTree(physicalWorkspace);
			FillPhysicalWorkspace(physicalWorkspace, logicalWorkspace);
			physicalWorkspace.Draw();
		}

		#endregion

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


		// Private methods
		private void CreateLogicalWorkspaceTree(
			ILogicalWorkspace logicalWorkspace,
			string studyInstanceUID)
		{
            Study study = ImageViewerComponent.StudyManager.StudyTree.GetStudy(studyInstanceUID);
			AddDisplaySets(logicalWorkspace, study);
		}

		private void AddDisplaySets(ILogicalWorkspace logicalWorkspace, Study study)
		{
			if (study == null)
				return;

			foreach (Series series in study.Series.Values)
			{
				//DisplaySet displaySet = new DisplaySet(series.SeriesDescription);
				DisplaySet displaySet = new DisplaySet();
				displaySet.Name = series.SeriesDescription;
				AddImages(displaySet, series);
				logicalWorkspace.DisplaySets.Add(displaySet);
			}
		}

		private void AddImages(IDisplaySet displaySet, Series series)
		{
			if (series == null)
				return;

			foreach (ImageSop image in series.Sops.Values)
			{
				DicomPresentationImage presentationImage = new DicomPresentationImage(image);
				displaySet.PresentationImages.Add(presentationImage);
			}

			// This has been added so that the initial presentation of each display set has a reasonable 
			// sort order.  When proper sorting support is added, the sorters will be extensions.
			displaySet.PresentationImages.Sort((IComparer<IPresentationImage>)new DicomPresentationImageSortByInstanceNumber());
		}

		private void CreatePhysicalWorkspaceTree(IPhysicalWorkspace physicalWorkspace)
		{
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
		}


		static public void FillPhysicalWorkspace(IPhysicalWorkspace physicalWorkspace, ILogicalWorkspace logicalWorkspace)
		{
			int displaySetIndex = 0;

			foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
			{
				if (displaySetIndex == logicalWorkspace.DisplaySets.Count)
					break;

				imageBox.DisplaySet = logicalWorkspace.DisplaySets[displaySetIndex];
				displaySetIndex++;
			}
		}
	}
}
