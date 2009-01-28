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

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class MprLayoutManager : LayoutManager
	{
		private DisplaySet _sagittalDisplaySet;
		private DisplaySet _coronalDisplaySet;
		private DisplaySet _axialDisplaySet;

		//TODO: Lump into a constructor?
		public void LoadSagittalDisplaySet(DisplaySet displaySet)
		{
			_sagittalDisplaySet = displaySet;
		}

		public void LoadCoronalDisplaySet(DisplaySet displaySet)
		{
			_coronalDisplaySet = displaySet;
		}

		public void LoadAxialDisplaySet(DisplaySet displaySet)
		{
			_axialDisplaySet = displaySet;
		}

		#region ILayoutManager Members

		public override void Layout()
		{
			LayoutPhysicalWorkspace();

			FillPhysicalWorkspace();

			ImageViewer.PhysicalWorkspace.Draw();
		}

		#endregion

		#region Protected Virtual Methods

		protected override void LayoutPhysicalWorkspace()
		{
			ImageViewer.PhysicalWorkspace.SetImageBoxGrid(1, 3);

			foreach (IImageBox imageBox in ImageViewer.PhysicalWorkspace.ImageBoxes)
				imageBox.SetTileGrid(1, 1);
		}

		protected override void FillPhysicalWorkspace()
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
			physicalWorkspace.ImageBoxes[0].DisplaySet = _sagittalDisplaySet;
			// Let's start out in the middle of each stack
			physicalWorkspace.ImageBoxes[0].TopLeftPresentationImageIndex = _sagittalDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[1].DisplaySet = _coronalDisplaySet;
			physicalWorkspace.ImageBoxes[1].TopLeftPresentationImageIndex = _coronalDisplaySet.PresentationImages.Count / 2;
			physicalWorkspace.ImageBoxes[2].DisplaySet = _axialDisplaySet;
			physicalWorkspace.ImageBoxes[2].TopLeftPresentationImageIndex = _axialDisplaySet.PresentationImages.Count / 2;

			//TODO: Add this property and use it to disable the Layout Components (in Layout.Basic).
			//physicalWorkspace.IsReadOnly = true;
		}

		#endregion

	}
}