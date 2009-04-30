#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTePresentationImage 
		: DicomGrayscalePresentationImage, 
		IDynamicTeProvider
	{
		#region Private fields

		private Frame _frame;
		private DynamicTe _dynamicTe;
		private ColorImageGraphic _probabilityOverlay;

		#endregion

		public DynamicTePresentationImage(
			Frame frame, 
			byte[] protonDensityMap,
			byte[] t2Map,
			byte[] probabilityMap)
			: base(frame)
		{
			Platform.CheckForNullReference(frame, "imageSop");

			_frame = frame;

			// TODO (Norman): DicomFilteredAnnotationLayoutProvider was made internal.  Either need to derive
			// this class from DicomGrayscalePresentationImage or create a layout provider.
			//this.AnnotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);

			AddProbabilityOverlay();
			_dynamicTe = new DynamicTe(
				this.ImageGraphic as GrayscaleImageGraphic, 
				protonDensityMap, 
				t2Map,
				_probabilityOverlay,
				probabilityMap);
		}

		public DynamicTe DynamicTe
		{
			get { return _dynamicTe; }
		}

		public bool ProbabilityOverlayVisible
		{
			get { return _probabilityOverlay.Visible; }
			set { _probabilityOverlay.Visible = value; }
		}

		public override IPresentationImage CreateFreshCopy()
		{
 			 return new DynamicTePresentationImage(
				 _frame, 
				 this.DynamicTe.ProtonDensityMap, 
				 this.DynamicTe.T2Map,
				 this.DynamicTe.ProbabilityMap);
		}


		private void AddProbabilityOverlay()
		{
			_probabilityOverlay = new ColorImageGraphic(_frame.Rows, _frame.Columns);
			this.OverlayGraphics.Add(_probabilityOverlay);
		}
	}
}
