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

using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	// Hack to expose LayoutManager
	internal class MprImageViewerComponent : ImageViewerComponent
	{
		private readonly ILayoutManager _layoutManager;

		public MprImageViewerComponent(ILayoutManager layoutManager) : base(layoutManager, null)
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
		public static ImageViewerComponent CreateMprLayoutAndComponent(Volume volume)
		{
			MprLayoutManager layoutManager = new MprLayoutManager(volume);

			MprImageViewerComponent imageViewer = new MprImageViewerComponent(layoutManager);

			return imageViewer;
		}

		#endregion

		#region Private fields

		private Volume _volume;
		private MprDisplaySet _identityDisplaySet;
		private MprDisplaySet _orthoXDisplaySet;
		private MprDisplaySet _orthoYDisplaySet;
		private MprDisplaySet _obliqueDisplaySet;

		#endregion

		#region Simple MPR layout

		public MprLayoutManager(Volume volume)
		{
			_volume = volume;
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

			ImageViewer.PhysicalWorkspace.Locked = true;
		}

		protected override void FillPhysicalWorkspace()
		{
			_identityDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.Identity, _volume);
			_orthoXDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.OrthoX, _volume);
			_orthoYDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.OrthoY, _volume);
			_obliqueDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.Oblique, _volume);

			// Hey, I said it was a hack!
			//Vector3D sliceThroughPatient = _volume.CenterPointPatient;
			//sliceThroughPatient.Y -= 100;
			//sliceThroughPatient.Z += 100;
			//_obliqueSlicer.SliceThroughPointPatient = sliceThroughPatient;
			//_obliqueSlicer.SliceExtentMillimeters = 150;

			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
			physicalWorkspace.ImageBoxes[0].DisplaySet = _identityDisplaySet;
			// Let's start out in the middle of each stack
			physicalWorkspace.ImageBoxes[0].TopLeftPresentationImageIndex = _identityDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[1].DisplaySet = _orthoYDisplaySet;
			physicalWorkspace.ImageBoxes[1].TopLeftPresentationImageIndex = _orthoYDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[2].DisplaySet = _orthoXDisplaySet;
			physicalWorkspace.ImageBoxes[2].TopLeftPresentationImageIndex = _orthoXDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[3].DisplaySet = _obliqueDisplaySet;
			physicalWorkspace.ImageBoxes[3].TopLeftPresentationImageIndex = _obliqueDisplaySet.PresentationImages.Count / 2;
		}

		#endregion

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_identityDisplaySet != null)
				{
					_identityDisplaySet.Dispose();
					_identityDisplaySet = null;
				}

				if (_orthoXDisplaySet != null)
				{
					_orthoXDisplaySet.Dispose();
					_orthoXDisplaySet = null;
				}
				
				if (_orthoYDisplaySet != null)
				{
					_orthoYDisplaySet.Dispose();
					_orthoYDisplaySet = null;
				}
				
				if (_obliqueDisplaySet != null)
				{
					_obliqueDisplaySet.Dispose();
					_obliqueDisplaySet = null;
				}

				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}
			}

			base.Dispose(disposing);
		}

		#endregion
	}
}