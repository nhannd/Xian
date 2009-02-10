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

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	// Hack to expose LayoutManager
	internal class MprImageViewerComponent : ImageViewerComponent
	{
		private readonly ILayoutManager _layoutManager;
		public MprImageViewerComponent(ILayoutManager layoutManager) : base(layoutManager)
		{
			_layoutManager = layoutManager;
		}

		public new ILayoutManager LayoutManager
		{
			get { return _layoutManager; }
		}
	}

	internal class MprLayoutManager : LayoutManager
	{
		#region ImageViewerComponent, LayoutManager creation method

		//ggerade ToRef: We're thinking we may introduce an MprImageViewerComponent that will own the volume
		//	and take care of the layout creation and such. For now I just needed a place to put this
		//	utility method which bootstraps the Mpr Component
		public static ImageViewerComponent CreateMprLayoutAndComponent(Volume volume)
		{
			MprLayoutManager layoutManager = new MprLayoutManager(volume);

			MprImageViewerComponent imageViewer = new MprImageViewerComponent(layoutManager);

			//ggerade ToRes: I almost got rid of the displayset members... hacked this in so that
			//	the workspace title would get set
			VolumeSlicer slicer = new VolumeSlicer(volume);
			DisplaySet displaySet = slicer.CreateAxialDisplaySet();
			imageViewer.StudyTree.AddSop(((DicomGrayscalePresentationImage)displaySet.PresentationImages[0]).ImageSop);

			return imageViewer;
		}

		#endregion

		#region Private fields

		private readonly Volume _volume;
		private readonly VolumeSlicer _sagittalSlicer;
		private readonly VolumeSlicer _coronalSlicer;
		private readonly VolumeSlicer _axialSlicer;
		private readonly VolumeSlicer _obliqueSlicer;

		#endregion

		#region Simple MPR layout

		public MprLayoutManager(Volume volume)
		{
			_volume = volume;
			_sagittalSlicer = new VolumeSlicer(volume);
			_coronalSlicer = new VolumeSlicer(volume);
			_axialSlicer = new VolumeSlicer(volume); 
			_obliqueSlicer = new VolumeSlicer(volume); 
		}

		public override void Layout()
		{
			LayoutPhysicalWorkspace();

			FillPhysicalWorkspace();

			ImageViewer.PhysicalWorkspace.Draw();
		}

		protected override void LayoutPhysicalWorkspace()
		{
			ImageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 2);

			foreach (IImageBox imageBox in ImageViewer.PhysicalWorkspace.ImageBoxes)
				imageBox.SetTileGrid(1, 1);
		}

		protected override void FillPhysicalWorkspace()
		{
			//ggerade ToRes: It might be better to create these on demand - this could be a relatively expensive
			//	ctor. Consider doing in FillPhysicalWorkspace maybe? 
			// Recall that the Sops need to go into the IVC's StudyTree, so we have to manage that somehow.
			DisplaySet sagittalDisplaySet = _sagittalSlicer.CreateSagittalDisplaySet();
			DisplaySet coronalDisplaySet = _coronalSlicer.CreateCoronalDisplaySet();
			DisplaySet axialDisplaySet = _axialSlicer.CreateAxialDisplaySet();
			// Hey, I said it was a hack!
			DisplaySet obliqueDisplaySet = _obliqueSlicer.CreateObliqueDisplaySet(45, 0, 45);
			rotateXs[3] = 45;
			rotateYs[3] = 0;
			rotateZs[3] = 45;

			// Here we add the Mpr DisplaySets to the IVC's StudyTree, this keeps the framework happy
			AddAllSopsToStudyTree(ImageViewer.StudyTree, sagittalDisplaySet);
			AddAllSopsToStudyTree(ImageViewer.StudyTree, coronalDisplaySet);
			AddAllSopsToStudyTree(ImageViewer.StudyTree, axialDisplaySet);
			AddAllSopsToStudyTree(ImageViewer.StudyTree, obliqueDisplaySet);

			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
			physicalWorkspace.ImageBoxes[0].DisplaySet = sagittalDisplaySet;
			// Let's start out in the middle of each stack
			physicalWorkspace.ImageBoxes[0].TopLeftPresentationImageIndex = sagittalDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[1].DisplaySet = coronalDisplaySet;
			physicalWorkspace.ImageBoxes[1].TopLeftPresentationImageIndex = coronalDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[2].DisplaySet = axialDisplaySet;
			physicalWorkspace.ImageBoxes[2].TopLeftPresentationImageIndex = axialDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[3].DisplaySet = obliqueDisplaySet;
			physicalWorkspace.ImageBoxes[3].TopLeftPresentationImageIndex = obliqueDisplaySet.PresentationImages.Count / 2;

			//TODO: Add this property and use it to disable the Layout Components (in Layout.Basic).
			//physicalWorkspace.IsReadOnly = true;
		}

		#endregion

		#region Really Hacked Up MPR "rotations"

		public void RotatePresentationImage(IPresentationImage presImage, int rotateX, int rotateY, int rotateZ)
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;

			int viewIndex;
			IDisplaySet displaySet = presImage.ParentDisplaySet;

			for (viewIndex = 0; viewIndex < 4; viewIndex++ )
				if (presImage == physicalWorkspace.ImageBoxes[viewIndex].TopLeftPresentationImage)
					break;

			// Short circuit if nothing changed
			if (rotateXs[viewIndex] == rotateX && rotateYs[viewIndex] == rotateY && rotateZs[viewIndex] == rotateZ)
				return;
			
			rotateXs[viewIndex] = rotateX;
			rotateYs[viewIndex] = rotateY;
			rotateZs[viewIndex] = rotateZ;

			// Hang on to the current index, we'll keep it the same with the new DisplaySet
			int currentIndex = physicalWorkspace.ImageBoxes[viewIndex].TopLeftPresentationImageIndex;

			//ggerade ToRes: What about other settings like Window/level? It is lost when the DisplaySets
			// are swapped out.

			// Clear out the old DisplaySet
			physicalWorkspace.ImageBoxes[viewIndex].DisplaySet = null;
			RemoveAllSopsFromStudyTree(ImageViewer.StudyTree, displaySet);

			switch (viewIndex)
			{
				case 0:
					displaySet = _sagittalSlicer.CreateSagittalDisplaySet(rotateX, rotateY);
					break;
				case 1:
					displaySet = _coronalSlicer.CreateCoronalDisplaySet(rotateX, rotateY);
					break;
				case 2:
					displaySet = _axialSlicer.CreateAxialDisplaySet(rotateX, rotateY);
					break;
				case 3:
					displaySet = _obliqueSlicer.CreateObliqueDisplaySet(rotateX, rotateY, rotateZ);
					break;
			}

			AddAllSopsToStudyTree(ImageViewer.StudyTree, displaySet);

			// Hacked this in so that the Imagebox wouldn't jump to first image all the time
			ImageBox box = (ImageBox)physicalWorkspace.ImageBoxes[viewIndex];
			box.PresetTopLeftPresentationImageIndex = currentIndex;

			physicalWorkspace.ImageBoxes[viewIndex].DisplaySet = displaySet;
			// Taken care of by hack above
			//physicalWorkspace.ImageBoxes[viewIndex].TopLeftPresentationImageIndex = currentIndex;

			physicalWorkspace.ImageBoxes[viewIndex].Draw();
		}

		private int[] rotateXs = new int[4];
		private int[] rotateYs = new int[4];
		private int[] rotateZs = new int[4];

		public int GetPresentationImageRotationX(IPresentationImage presImage)
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;

			int viewIndex;
			for (viewIndex = 0; viewIndex < physicalWorkspace.ImageBoxes.Count; viewIndex++)
				if (presImage == physicalWorkspace.ImageBoxes[viewIndex].TopLeftPresentationImage)
					break;

			return rotateXs[viewIndex];
		}

		public int GetPresentationImageRotationY(IPresentationImage presImage)
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;

			int viewIndex;
			for (viewIndex = 0; viewIndex < physicalWorkspace.ImageBoxes.Count; viewIndex++)
				if (presImage == physicalWorkspace.ImageBoxes[viewIndex].TopLeftPresentationImage)
					break;

			return rotateYs[viewIndex];
		}

		public int GetPresentationImageRotationZ(IPresentationImage presImage)
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;

			int viewIndex;
			for (viewIndex = 0; viewIndex < physicalWorkspace.ImageBoxes.Count; viewIndex++)
				if (presImage == physicalWorkspace.ImageBoxes[viewIndex].TopLeftPresentationImage)
					break;

			return rotateZs[viewIndex];
		}

		#endregion

		#region StudyTree helpers

		public void AddDisplaySetsToStudyTree(StudyTree tree)
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;

			foreach (IImageBox box in physicalWorkspace.ImageBoxes)
			{
				AddAllSopsToStudyTree(tree, box.TopLeftPresentationImage.ParentDisplaySet);
			}
		}

		// Note: The overlays expect that a Sop is parented by a Series, so this was the easiest way
		//	to keep the IVC happy.
		private static void AddAllSopsToStudyTree(StudyTree tree, IDisplaySet displaySet)
		{
			// Now load the generated images into the viewer
			foreach (PresentationImage presentationImage in displaySet.PresentationImages)
			{
				DicomGrayscalePresentationImage dicomGrayscalePresentationImage =
					(DicomGrayscalePresentationImage) presentationImage;

				ImageSop sop = dicomGrayscalePresentationImage.ImageSop;
				tree.AddSop(sop);
			}
		}

		private static void RemoveAllSopsFromStudyTree(StudyTree tree, IDisplaySet displaySet)
		{
			// Now load the generated images into the viewer
			foreach (PresentationImage presentationImage in displaySet.PresentationImages)
			{
				DicomGrayscalePresentationImage dicomGrayscalePresentationImage =
					(DicomGrayscalePresentationImage) presentationImage;

				ImageSop sop = dicomGrayscalePresentationImage.ImageSop;
				tree.RemoveSop(sop);
			}
		}

		#endregion

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_volume != null)
					_volume.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

	}
}