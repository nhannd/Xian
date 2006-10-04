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
			LogicalWorkspace logicalWorkspace, 
			PhysicalWorkspace physicalWorkspace,
			string studyInstanceUID)
		{
			CreateLogicalWorkspaceTree(logicalWorkspace, studyInstanceUID);
			CreatePhysicalWorkspaceTree(physicalWorkspace);
			FillPhysicalWorkspace(physicalWorkspace, logicalWorkspace);
			physicalWorkspace.Draw(false);
		}

		#endregion

		// Private methods
		private void CreateLogicalWorkspaceTree(
			LogicalWorkspace logicalWorkspace,
			string studyInstanceUID)
		{
            Study study = ImageViewerComponent.StudyManager.StudyTree.GetStudy(studyInstanceUID);
			AddDisplaySets(logicalWorkspace, study);
		}

		private void AddDisplaySets(LogicalWorkspace logicalWorkspace, Study study)
		{
			if (study == null)
				return;

			foreach (Series series in study.Series.Values)
			{
				DisplaySet displaySet = new DisplaySet(series.SeriesDescription);
				AddImages(displaySet, series);
				logicalWorkspace.DisplaySets.Add(displaySet);
			}
		}

		private void AddImages(DisplaySet displaySet, Series series)
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
			displaySet.PresentationImages.Sort((IComparer<PresentationImage>)new DicomPresentationImageSortByInstanceNumber());
		}

		private void CreatePhysicalWorkspaceTree(PhysicalWorkspace physicalWorkspace)
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


		static public void FillPhysicalWorkspace(PhysicalWorkspace physicalWorkspace, LogicalWorkspace logicalWorkspace)
		{
			int displaySetIndex = 0;

			foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
			{
				if (displaySetIndex == logicalWorkspace.DisplaySets.Count)
					break;

				// Select the first image of the first display set
				//if (displaySetIndex == 0)
				//	logicalWorkspace[0][0].SetSelected(true, SelectionType.Single);

				imageBox.DisplaySet = logicalWorkspace.DisplaySets[displaySetIndex];
				displaySetIndex++;
			}
		}
	}
}
